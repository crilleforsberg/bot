using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KarelazisBot.Objects
{
    public partial class Client
    {
        public partial class LoginHelper
        {
            public LoginHelper(Objects.Client parent)
            {
                this.Parent = parent;
            }

            public Objects.Client Parent { get; private set; }
            private const byte UndefinedIndex = byte.MaxValue;

            /// <summary>
            /// Gets this 
            /// </summary>
            /// <returns></returns>
            public IEnumerable<LoginServer> GetLoginServers()
            {
                for (int i = this.Parent.Addresses.LoginServer.Start;
                    i < this.Parent.Addresses.LoginServer.End;
                    i += this.Parent.Addresses.LoginServer.Step)
                {
                    yield return new LoginServer(
                        this.Parent.Memory.ReadString(i + this.Parent.Addresses.LoginServer.Distances.IP),
                        this.Parent.Memory.ReadUInt16(i + this.Parent.Addresses.LoginServer.Distances.Port));
                }
            }
            /// <summary>
            /// Sets the client's login servers.
            /// </summary>
            /// <param name="loginServer">The login server to set.</param>
            /// <param name="index">Set this to 0-5 if you want to set at a specific index.</param>
            public void SetLoginServer(LoginServer loginServer, byte index = UndefinedIndex)
            {
                if (index != byte.MaxValue)
                {
                    int address = this.Parent.Addresses.LoginServer.Start +
                        this.Parent.Addresses.LoginServer.Step * index;
                    this.Parent.Memory.WriteString(address + this.Parent.Addresses.LoginServer.Distances.IP,
                        loginServer.IP);
                    this.Parent.Memory.WriteUInt16(address + this.Parent.Addresses.LoginServer.Distances.Port,
                        loginServer.Port);
                }
                else
                {
                    for (int i = this.Parent.Addresses.LoginServer.Start;
                    i < this.Parent.Addresses.LoginServer.End;
                    i += this.Parent.Addresses.LoginServer.Step)
                    {
                        this.Parent.Memory.WriteString(i + this.Parent.Addresses.LoginServer.Distances.IP,
                            loginServer.IP);
                        this.Parent.Memory.WriteUInt16(i + this.Parent.Addresses.LoginServer.Distances.Port,
                            loginServer.Port);
                    }
                }
            }
            /// <summary>
            /// Sets the client's login servers.
            /// </summary>
            /// <param name="loginServers">The login servers to set.</param>
            public void SetLoginServers(IEnumerable<LoginServer> loginServers)
            {
                byte index = 0;
                foreach (LoginServer ls in loginServers)
                {
                    this.SetLoginServer(ls, index);
                    index++;
                }
            }
            /// <summary>
            /// Sets the client's login server(s).
            /// </summary>
            /// <param name="ip">The IP or hostname.</param>
            /// <param name="port">The port.</param>
            /// <param name="index">The specific index to set. Will set all indexes if left as default.</param>
            public void SetLoginServer(string ip, ushort port, byte index = UndefinedIndex)
            {
                this.SetLoginServer(new LoginServer(ip, port), index);
            }
        }
    }
}
