using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace KarelazisBot.Modules
{
    public class CombobotServer
    {
        public CombobotServer(Objects.Client c, ushort port)
        {
            this.Client = c;
            this.ListeningPort = port;
            this.ConnectedClients = new List<CombobotClientDescriptor>();
        }

        public Objects.Client Client { get; private set; }
        /// <summary>
        /// Gets the listening port.
        /// </summary>
        public ushort ListeningPort { get; internal set; }
        /// <summary>
        /// Gets whether the server is currently running.
        /// </summary>
        public bool IsRunning { get; private set; }
        private ManualResetEvent SendComboResetEvent { get; set; }
        private List<CombobotClientDescriptor> ConnectedClients { get; set; }
        private uint Target { get; set; }
        private enum PacketType
        {
            PlayerInfo = 1,
            Combo = 2,
            Ping = 3
        }

        #region events
        public delegate void ClientConnectedHandler(CombobotClientDescriptor descriptor);
        /// <summary>
        /// An event that gets fired when a client's incoming connection request is succesfully handled.
        /// </summary>
        public event ClientConnectedHandler ClientConnected;
        public delegate void ClientDisconnectedHandler(CombobotClientDescriptor descriptor);
        /// <summary>
        /// An event that gets fired when a client has disconnected.
        /// </summary>
        public event ClientDisconnectedHandler ClientDisconnected;
        public delegate void ClientPingReceivedHandler(CombobotClientDescriptor descriptor);
        /// <summary>
        /// An event that gets fired when a descriptor's roundtrip time (aka ping) gets received and updated.
        /// </summary>
        public event ClientPingReceivedHandler ClientPingReceived;
        #endregion

        /// <summary>
        /// Starts listening for incoming connection requests and handles them.
        /// </summary>
        /// <param name="listeningPort"></param>
        public void Start(ushort listeningPort)
        {
            if (this.IsRunning) return;
            this.IsRunning = true;
            this.ListeningPort = listeningPort;
            Thread t = new Thread(new ThreadStart(this.Listen));
            t.Start();
            Thread t2 = new Thread(new ThreadStart(this.KeepAliveClients));
            t2.Start();
        }
        /// <summary>
        /// Stops listening for incoming connection requests and disconnects all connected clients.
        /// </summary>
        public void Stop()
        {
            if (!this.IsRunning) return;
            this.IsRunning = false;
            foreach (var descriptor in this.GetConnectedClients())
            {
                if (descriptor.TcpClient != null) descriptor.TcpClient.Close();
            }
        }
        /// <summary>
        /// Gets an array of connected clients.
        /// </summary>
        /// <returns></returns>
        public IEnumerable<CombobotClientDescriptor> GetConnectedClients()
        {
            return this.ConnectedClients;
        }
        /// <summary>
        /// Issues a combo command to all connected clients.
        /// </summary>
        /// <param name="id">The ID of the creature, player or NPC.</param>
        public void Combo(uint id)
        {
            if (id == 0) return;
            this.Target = id;
            this.SendComboResetEvent = new ManualResetEvent(false);
            foreach (CombobotClientDescriptor descriptor in this.ConnectedClients)
            {
                Thread t = new Thread(new ParameterizedThreadStart(this.SendComboThread));
                t.Start(descriptor.TcpClient);
            }
            this.SendComboResetEvent.Set();
        }

        /// <summary>
        /// Listens for incoming connection requests. Run this method on a seperate thread.
        /// </summary>
        private void Listen()
        {
            TcpListener listener = null;
            try
            {
                listener = new TcpListener(IPAddress.Any, this.ListeningPort);
                listener.Start();
                while (this.IsRunning)
                {
                    if (!listener.Pending()) { Thread.Sleep(50); continue; }
                    TcpClient tc = listener.AcceptTcpClient();
                    CombobotClientDescriptor descriptor = new CombobotClientDescriptor(tc, string.Empty);
                    this.ConnectedClients.Add(descriptor);
                    if (this.ClientConnected != null) this.ClientConnected(descriptor);
                    Thread t = new Thread(new ParameterizedThreadStart(this.HandleClient));
                    t.Start(descriptor);
                }
            }
            catch { }
            finally
            {
                if (listener != null) listener.Stop();
            }
        }
        /// <summary>
        /// Handles a connected client. Run this method on a seperate thread.
        /// </summary>
        /// <param name="combobotClientDescriptor">A CombobotClientDescriptor object.</param>
        private void HandleClient(object combobotClientDescriptor)
        {
            CombobotClientDescriptor descriptor = (CombobotClientDescriptor)combobotClientDescriptor;
            try
            {
                TcpClient tc = descriptor.TcpClient;
                NetworkStream nstream = tc.GetStream();
                while (this.IsRunning)
                {
                    Objects.Packet p = Objects.Packet.GetNextPacket(nstream);
                    if (p.Length == 0) break; // disconnected

                    p.GetUInt16(); // length, not needed
                    switch ((PacketType)p.GetByte())
                    {
                        case PacketType.Ping:
                            descriptor.SetRTT((ushort)descriptor.GetElapsedMilliseconds());
                            if (this.ClientPingReceived != null) this.ClientPingReceived(descriptor);
                            break;
                        case PacketType.PlayerInfo:
                            descriptor.CharacterName = p.GetString();
                            break;
                    }
                }
            }
            catch { }
            finally
            {
                if (descriptor.TcpClient != null) descriptor.TcpClient.Close();
                this.ConnectedClients.Remove(descriptor);
                if (this.ClientDisconnected != null) this.ClientDisconnected(descriptor);
            }
        }
        /// <summary>
        /// Sends a player information request packet and a ping packet every 5 seconds to all connected clients.
        /// Run this method on a seperate thread.
        /// </summary>
        private void KeepAliveClients()
        {
            while (this.IsRunning)
            {
                Thread.Sleep(5000);

                // create a ping packet
                Objects.Packet p = new Objects.Packet();
                p.AddByte((byte)PacketType.Ping);
                p.AddLength();
                // create a player info request packet
                Objects.Packet playerInfo = new Objects.Packet();
                playerInfo.AddByte((byte)PacketType.PlayerInfo);
                playerInfo.AddLength();
                foreach (CombobotClientDescriptor descriptor in this.GetConnectedClients())
                {
                    playerInfo.Send(descriptor.TcpClient);
                    descriptor.StartTimer();
                    p.Send(descriptor.TcpClient);
                }
            }
        }
        private void SendComboThread(object tcpclient)
        {
            try
            {
                TcpClient tc = (TcpClient)tcpclient;
                // wait for all threads to start
                this.SendComboResetEvent.WaitOne();
                Objects.Packet p = new Objects.Packet();
                p.AddByte((byte)PacketType.Combo);
                p.AddUInt32(this.Target);
                p.AddLength();
                p.Send(tc);
            }
            catch { }
        }
    }

    public class CombobotClient
    {
        public CombobotClient(Objects.Client c) : this(c, "127.0.0.1", 3000, ActionType.Say, string.Empty) { }
        public CombobotClient(Objects.Client c, string serverip, ushort serverport, ActionType action, string actionEx)
        {
            this.Client = c;
            this.ServerIP = serverip;
            this.ServerPort = serverport;
            this.Action = action;
            this.ActionEx = actionEx;
        }

        /// <summary>
        /// Gets the client associated with this object.
        /// </summary>
        public Objects.Client Client { get; private set; }
        /// <summary>
        /// Gets the server's IP.
        /// </summary>
        public string ServerIP { get; internal set; }
        /// <summary>
        /// Gets the server's port.
        /// </summary>
        public ushort ServerPort { get; internal set; }
        /// <summary>
        /// Gets whether this client is currently connected to a server.
        /// </summary>
        public bool IsConnected { get; private set; }
        /// <summary>
        /// Gets or sets the action to use when a combo is issued by the combo leader.
        /// </summary>
        public ActionType Action { get; set; }
        /// <summary>
        /// Gets or sets the extra action (i.e. item ID or spell) when a combo is issued by the combo leader.
        /// </summary>
        public string ActionEx { get; set; }
        public enum ActionType
        {
            Say,
            CustomRune,
            SuddenDeath,
            Explosion,
            HeavyMagicMissile,
            Paralyze
        }
        private Thread ThreadHandleServer { get; set; }
        private TcpClient TcpClient { get; set; }
        private enum PacketType
        {
            PlayerInfo = 1,
            Combo = 2,
            Ping = 3
        }

        #region events
        public delegate void ConnectedHandler();
        /// <summary>
        /// An event that gets fired when a connection is successfully established.
        /// </summary>
        public event ConnectedHandler Connected;
        public delegate void Disconnectedhandler();
        /// <summary>
        /// An event taht gets fired when a connection is either unsuccessful or the socket disconnects.
        /// </summary>
        public event Disconnectedhandler Disconnected;
        #endregion

        public void Connect(string ip, ushort port)
        {
            this.ServerIP = ip;
            this.ServerPort = port;
            while (this.ThreadHandleServer != null && this.ThreadHandleServer.IsAlive) this.ThreadHandleServer.Abort();
            this.ThreadHandleServer = new Thread(new ThreadStart(this.HandleServer));
            this.ThreadHandleServer.Start();
        }
        public void Disconnect()
        {
            if (this.TcpClient != null && this.TcpClient.Connected) this.TcpClient.Close();
            this.IsConnected = false;
        }

        private void HandleServer()
        {
            try
            {
                this.TcpClient = new TcpClient(this.ServerIP, this.ServerPort);
                this.IsConnected = true;
                if (this.Connected != null) this.Connected();
                Objects.Item item;
                Objects.Packet p = new Objects.Packet();
                p.AddByte((byte)PacketType.PlayerInfo);
                p.AddString(this.Client.Player.Name);
                p.AddLength();
                p.Send(this.TcpClient);
                while (true)
                {
                    p = Objects.Packet.GetNextPacket(this.TcpClient.GetStream());
                    if (p.Length == 0) break; // disconnected
                    p.GetUInt16(); // length, not needed
                    switch ((PacketType)p.GetByte())
                    {
                        case PacketType.Combo:
                            if (!this.Client.Player.Connected) continue;
                            uint targetid = p.GetUInt32();
                            Objects.Creature c = this.Client.BattleList.GetAny(targetid);
                            if (c == null || !this.Client.Player.Location.IsOnScreen(c.Location)) break;
                            switch (this.Action)
                            {
                                case ActionType.Say:
                                    if (string.IsNullOrEmpty(this.ActionEx)) break;
                                    this.Client.Packets.Say(this.ActionEx);
                                    break;
                                case ActionType.CustomRune:
                                    ushort runeid = 0;
                                    if (!ushort.TryParse(this.ActionEx, out runeid)) break;
                                    item = this.Client.Inventory.GetItem(runeid);
                                    if (item != null) item.UseOnCreature(c);
                                    break;
                                case ActionType.SuddenDeath:
                                    item = this.Client.Inventory.GetItem(this.Client.ItemList.Runes.SuddenDeath);
                                    if (item != null) item.UseOnCreature(c);
                                    break;
                                case ActionType.HeavyMagicMissile:
                                    item = this.Client.Inventory.GetItem(this.Client.ItemList.Runes.HeavyMagicMissile);
                                    if (item != null) item.UseOnCreature(c);
                                    break;
                                case ActionType.Explosion:
                                    item = this.Client.Inventory.GetItem(this.Client.ItemList.Runes.Explosion);
                                    if (item != null) item.UseOnCreature(c);
                                    break;
                                case ActionType.Paralyze:
                                    item = this.Client.Inventory.GetItem(this.Client.ItemList.Runes.Paralyze);
                                    if (item != null) item.UseOnCreature(c);
                                    break;
                            }
                            break;
                        case PacketType.Ping:
                            p = new Objects.Packet();
                            p.AddByte((byte)PacketType.Ping);
                            p.AddLength();
                            p.Send(this.TcpClient);
                            break;
                        case PacketType.PlayerInfo:
                            p = new Objects.Packet();
                            p.AddByte((byte)PacketType.PlayerInfo);
                            p.AddString(this.Client.Player.Name);
                            p.AddLength();
                            p.Send(this.TcpClient);
                            break;
                    }
                }
            }
            catch { }
            finally
            {
                if (this.TcpClient != null) this.TcpClient.Close();
                this.IsConnected = false;
                if (this.Disconnected != null) this.Disconnected();
            }
        }
    }

    /// <summary>
    /// A class to describe connected clients to the user through the UI.
    /// </summary>
    public class CombobotClientDescriptor
    {
        public CombobotClientDescriptor(TcpClient tc, string characterName)
        {
            this.TcpClient = tc;
            this.CharacterName = characterName;
            this.Stopwatch = new System.Diagnostics.Stopwatch();
        }

        /// <summary>
        /// Gets the TcpClient object this character is using.
        /// </summary>
        public TcpClient TcpClient { get; private set; }
        private string _CharacterName { get; set; }
        /// <summary>
        /// Gets or sets the character name.
        /// </summary>
        public string CharacterName
        {
            get { return this._CharacterName; }
            set
            {
                this._CharacterName = value;
                if (this.CharacterNameChanged != null) this.CharacterNameChanged(this, this._CharacterName);
            }
        }
        public delegate void CharacterNameChangedHandler(CombobotClientDescriptor descriptor, string newName);
        public event CharacterNameChangedHandler CharacterNameChanged;
        /// <summary>
        /// Gets or sets the roundtrip time (aka ping).
        /// </summary>
        private ushort RTT { get; set; }
        private System.Diagnostics.Stopwatch Stopwatch { get; set; }

        /// <summary>
        /// Gets the roundtrip time (aka ping).
        /// </summary>
        /// <returns></returns>
        public ushort GetRTT()
        {
            return this.RTT;
        }
        /// <summary>
        /// Sets the roundtrip time (aka ping), and stops and resets the timer.
        /// </summary>
        /// <param name="rtt"></param>
        public void SetRTT(ushort rtt)
        {
            this.RTT = rtt;
            this.Stopwatch.Reset();
        }
        /// <summary>
        /// Starts the timer, used to calculate RTT.
        /// </summary>
        public void StartTimer()
        {
            if (this.Stopwatch.IsRunning) return;
            this.Stopwatch.Start();
        }
        /// <summary>
        /// Gets the amount of milliseconds has passed since the timer started. Returns 0 if the timer is not running.
        /// </summary>
        /// <returns></returns>
        public long GetElapsedMilliseconds()
        {
            if (this.Stopwatch.IsRunning) return 0;
            return this.Stopwatch.ElapsedMilliseconds;
        }

        public override string ToString()
        {
            return this.CharacterName + " - " + this.RTT + "ms";
        }
    }
}
