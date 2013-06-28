// for this to work, you need to set up a SSH tunnel
// The following tutorial uses PuTTY

// Session -> Enter the host name and port of the SSH server
// Connection -> SSH -> Tunnels:
// For the login server:
// Source port: 7778
// Destination: realots.net:7778
// Local + Auto
// Press Add
// For the game server:
// Source port: 7444
// Destination: realots.net:7444
// Local + Auto
// Press Add

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Net;
using System.Net.Sockets;
using KarelazisBot; // base access to enumerators, Windows API and such
using KarelazisBot.Objects;

public class Test
{
    static BouncerClient currentBouncerClient = null;

    public static void Main(Client client)
    {
        currentBouncerClient = new BouncerClient(client, 7778, 7444);
        currentBouncerClient.Start();
        while (currentBouncerClient.IsRunning) Thread.Sleep(10);
    }
    public static void Cleanup()
    {
        currentBouncerClient.Stop();
    }

    public class BouncerClient
    {
        public BouncerClient(Client c, ushort sshPortLogin, ushort sshPortGame)
        {
            this.Client = c;
            this.SshListeningPortLogin = sshPortLogin;
            this.SshListeningPortGame = sshPortGame;
        }

        #region properties
        public Client Client { get; private set; }
        public ushort ListeningPort { get; private set; }
        public bool IsRunning { get; private set; }
        public ushort SshListeningPortLogin { get; private set; }
        public ushort SshListeningPortGame { get; private set; }

        private List<LoginServer> OldLoginServers { get; set; }
        private List<CharacterList.Character> OldCharacterList { get; set; }
        #endregion

        #region public methods
        public void Start()
        {
            if (this.IsRunning || this.Client.Memory.ReadByte(this.Client.Addresses.Misc.Connection) != 0) return;
			this.ListeningPort = this.FindFreeListeningPort();
			if (this.ListeningPort == 0) return;
            this.IsRunning = true;
            this.OldLoginServers = this.Client.Login.GetLoginServers().ToList();
            this.Client.Login.SetLoginServer(new LoginServer("127.0.0.1", this.ListeningPort));
            Thread t = new Thread(this.Listen);
            t.Start();
        }
        public void Stop()
        {
            if (!this.IsRunning) return;
            this.IsRunning = false;
            this.Client.Login.SetLoginServers(this.OldLoginServers);
            if (this.OldCharacterList != null && this.OldCharacterList.Count > 0)
            {
                this.Client.CharacterList.SetCharacters(this.OldCharacterList);
            }
        }
        #endregion

        #region private methods
		/// <summary>
        /// Attempts to get a free listening port. Returns 0 if unsuccessful.
        /// </summary>
        /// <param name="min"></param>
        /// <param name="max"></param>
        /// <returns></returns>
        private ushort FindFreeListeningPort(ushort min = 7000, ushort max = 7999)
        {
            for (ushort i = Math.Min(min, max); i < Math.Max(min, max); i++)
            {
                try
                {
                    TcpListener listener = new TcpListener(IPAddress.Loopback, i);
                    listener.Start();
                    listener.Stop();
                    return i;
                }
                catch { }
            }
            return 0;
        }
        private void Listen()
        {
            TcpListener listener = new TcpListener(IPAddress.Loopback, this.ListeningPort);
            try
            {
                listener.Start();
                while (this.IsRunning)
                {
                    Thread.Sleep(100);
                    if (!listener.Pending()) continue;

                    TcpClient tc = listener.AcceptTcpClient();
                    Thread t = new Thread(new ParameterizedThreadStart(this.HandleClient));
                    t.Start(tc);
                }
            }
            catch { }
            finally
            {
                this.IsRunning = false;
                if (listener != null) listener.Stop();
            }
        }
        private void HandleClient(object tcpclient)
        {
            TcpClientPair pair = new TcpClientPair((TcpClient)tcpclient, new TcpClient());
            try
            {
                // prepare to receive data from the bouncer server
                Thread t = null;
                switch ((Enums.Connection)this.Client.Memory.ReadByte(this.Client.Addresses.Misc.Connection))
                {
                    case Enums.Connection.ConnectingToLoginServer:
                        pair.Server = new TcpClient("127.0.0.1", this.SshListeningPortLogin);
                        t = new Thread(new ParameterizedThreadStart(this.HandleServer));
                        t.Start(pair);
                        break;
                    default:
                        pair.Server = new TcpClient("127.0.0.1", this.SshListeningPortGame);
                        t = new Thread(new ParameterizedThreadStart(this.HandleServer));
                        t.Start(pair);
                        break;
                }

                NetworkStream nstream = pair.Client.GetStream();
                while (true)
                {
                    Packet p = Packet.GetNextPacket(nstream);
                    if (p.Length == 0) break;

                    p.Send(pair.Server);
                }
            }
            catch { }

            if (pair != null)
            {
                if (pair.Client != null && pair.Client.Connected) pair.Client.Close();
                if (pair.Server != null && pair.Server.Connected) pair.Server.Close();
            }
        }
        private void HandleServer(object tcppair)
        {
            TcpClientPair pair = (TcpClientPair)tcppair;
            try
            {
                NetworkStream nstream = pair.Server.GetStream();
                bool changeCharacterList = false;
                byte connection = 0;

                while (true)
                {
                    Packet p = Packet.GetNextPacket(nstream);
                    if (p.Length == 0) break;

                    if (connection != (byte)Enums.Connection.Online)
                    {
                        connection = this.Client.Memory.ReadByte(this.Client.Addresses.Misc.Connection);
                        if ((Enums.Connection)connection == Enums.Connection.ConnectingToLoginServer)
                        {
                            changeCharacterList = true;
                        }
                    }

                    p.Send(pair.Client);

                    if (changeCharacterList)
                    {
                        for (int j = 0; j < 10; j++)
                        {
                            if (this.Client.Memory.ReadByte(this.Client.Addresses.Misc.ActionState) != 0)
                            {
                                int address = this.Client.Memory.ReadInt32(this.Client.Addresses.Dialog.Pointer);
                                address += this.Client.Addresses.DialogDistances.Title;
                                string title = this.Client.Memory.ReadString(address);
                                if (title == "Select Character")
                                {
                                    this.OldCharacterList = this.Client.CharacterList.GetCharacters().ToList();
                                    List<CharacterList.Character> temp = this.Client.CharacterList.GetCharacters().ToList();
                                    foreach (var c in temp)
                                    {
                                        c.ServerIP = "127.0.0.1";
                                        c.ServerPort = this.SshListeningPortGame;
                                    }
                                    this.Client.CharacterList.SetCharacters(temp);
                                    break;
                                }
                            }
                            Thread.Sleep(100);
                        }
                        changeCharacterList = false;
                    }
                }
            }
            catch { }

            if (pair != null)
            {
                if (pair.Client != null && pair.Client.Connected) pair.Client.Close();
                if (pair.Server != null && pair.Server.Connected) pair.Server.Close();
            }
        }
        #endregion

        class TcpClientPair
        {
            public TcpClientPair(TcpClient tcClient, TcpClient tcServer)
            {
                this.Client = tcClient;
                this.Server = tcServer;
            }

            public TcpClient Client { get; set; }
            public TcpClient Server { get; set; }
        }
    }
}
