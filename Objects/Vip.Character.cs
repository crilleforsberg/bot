using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KarelazisBot.Objects
{
    public partial class Vip
    {
        public class Character
        {
            public Character(Client client, int address, uint id, string name, bool online, Constants.Vip.Icons icon)
            {
                this.Client = client;
                this.Address = address;
                this.ID = id;
                this.Name = name;
                this.Online = online;
                this.Icon = icon;
            }

            public Client Client { get; private set; }
            public int Address { get; private set; }
            public uint ID { get; set; }
            public string Name { get; set; }
            public bool Online { get; set; }
            public Constants.Vip.Icons Icon { get; set; }

            public void SetIcon(Constants.Vip.Icons icon)
            {
                if (!Enum.IsDefined(typeof(Constants.Vip.Icons), icon)) return;
                this.Client.Memory.WriteByte(this.Address + this.Client.Addresses.Vip.Distances.Icon, (byte)icon);
                this.Icon = icon;
            }
        }
    }
}
