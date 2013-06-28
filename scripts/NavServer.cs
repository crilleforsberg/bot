using System; // access to basic stuff in .NET
using System.Collections.Generic; // access to things like List<T> (T = value type) in .NET
using System.Threading;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using KarelazisBot; // base access to enumerators, Windows API and such
using KarelazisBot.Objects; // access to all the objects

public class Test
{
    public static void Main(Client client)
    {
        NavServer server = new NavServer(4315);
        server.Start();
        while (server.IsRunning)
        {
            Thread.Sleep(1000);
        }
    }

    public class NavServer
    {
        public NavServer(ushort listeningPort)
        {
            this.ListeningPort = listeningPort;
            this.ResetEvent = new ManualResetEventSlim();
            this.ConnectedClients = new List<ClientDescriptor>();
        }

        public ushort ListeningPort { get; private set; }
        public bool IsRunning { get; private set; }
        private ManualResetEventSlim ResetEvent { get; set; }
        private List<ClientDescriptor> ConnectedClients { get; set; }

        public void Start()
        {
            if (this.IsRunning) return;
            this.ResetEvent.Reset();
            this.IsRunning = true;
            Thread t = new Thread(this.Listen);
            t.Start();
        }
		public void Stop()
		{
			if (!this.IsRunning) return;
            this.ResetEvent.Set();
            this.IsRunning = false;
		}
        public IEnumerable<ClientDescriptor> GetConnectedClients()
        {
            return this.ConnectedClients.ToArray();
        }

        private void Listen()
        {
            try
            {
                var listener = new TcpListener(IPAddress.Any, this.ListeningPort);
                listener.Start();
                while (!this.ResetEvent.Wait(100))
                {
                    if (!listener.Pending()) continue;

                    TcpClient tc = listener.AcceptTcpClient();
                    Thread t = new Thread(new ParameterizedThreadStart(this.HandleClient));
                    t.Start(new ClientDescriptor(tc));
                }
            }
            catch { }
            finally
            {
                this.IsRunning = false;
            }
        }
        private void HandleClient(object clientDescriptor)
        {
            ClientDescriptor descriptor = null;
            try
            {
                descriptor = (ClientDescriptor)clientDescriptor;
                this.ConnectedClients.Add(descriptor);
                NetworkStream nstream = descriptor.TcpClient.GetStream();
                while (this.IsRunning)
                {
                    Packet p = Packet.GetNextPacket(nstream);
                    if (p.Length == 0) break;

                    p.GetUInt16(); // packet length
                    descriptor.PlayerName = p.GetString();
                    descriptor.PlayerID = p.GetUInt32();
                    descriptor.PlayerLevel = p.GetUInt16();
                    descriptor.Location = new Location(p.GetUInt16(), p.GetUInt16(), p.GetByte());
                }
            }
            catch { }
            finally
            {
                if (descriptor != null)
                {
                    descriptor.TcpClient.Close();
                    this.ConnectedClients.Remove(descriptor);
                }
            }
        }
        private void UpdateClients()
        {
            try
            {
                while (!this.ResetEvent.Wait(200))
                {
                    try
                    {
                        var descriptors = this.ConnectedClients.ToArray();
                        if (descriptors.Length == 0) continue;
                        Packet p = new Packet();
                        List<TcpClient> sockets = new List<TcpClient>();

                        p.AddUInt16((ushort)descriptors.Length);
                        foreach (ClientDescriptor descriptor in descriptors)
                        {
                            sockets.Add(descriptor.TcpClient);
                            p.AddString(descriptor.PlayerName);
                            p.AddUInt32(descriptor.PlayerID);
                            p.AddUInt16(descriptor.PlayerLevel);
                            p.AddUInt16((ushort)descriptor.Location.X);
                            p.AddUInt16((ushort)descriptor.Location.Y);
                            p.AddByte((byte)descriptor.Location.Z);
                        }
                        p.AddLength();
                        foreach (TcpClient tc in sockets)
                        {
                            if (!tc.Connected) continue;
                            p.Send(tc);
                        }
                    }
                    catch { }
                }
            }
            catch { }
        }
    }
    public class ClientDescriptor
    {
        public ClientDescriptor(TcpClient tc)
        {
            this.TcpClient = tc;
        }

        public TcpClient TcpClient { get; private set; }
        public string PlayerName { get; set; }
        public uint PlayerID { get; set;}
        public ushort PlayerLevel { get; set; }
        public Location Location { get; set; }
    }
}
