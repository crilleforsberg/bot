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
            public class LoginServer
            {
                public LoginServer(string ip, ushort port)
                {
                    this.IP = ip;
                    this.Port = port;
                }

                public string IP { get; private set; }
                public ushort Port { get; private set; }
            }
        }
    }
}
