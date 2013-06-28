using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KarelazisBot.Objects
{
    public partial class Vip
    {
        public Vip(Objects.Client client)
        {
            this.Client = client;
            this.CachedCharacters = new Character[this.Client.Addresses.Vip.MaxEntries];

            for (int i = 0; i < this.CachedCharacters.Length; i++)
            {
                this.CachedCharacters[i] = new Character(this.Client,
                    this.Client.Addresses.Vip.Start + this.Client.Addresses.Vip.Step * i,
                    0, string.Empty, false, Constants.Vip.Icons.Blank);
            }
        }

        public Objects.Client Client { get; private set; }
        public ushort Count
        {
            get { return this.Client.Memory.ReadUInt16(this.Client.Addresses.Vip.Count); }
            private set { this.Client.Memory.WriteUInt16(this.Client.Addresses.Vip.Count, value); }
        }

        private Character[] CachedCharacters { get; set; }

        public IEnumerable<Character> GetCharacters(bool onlineOnly = false)
        {
            for (int i = 0; i < this.Count; i++)
            {
                Character c = this.CachedCharacters[i];
                int address = this.Client.Addresses.Vip.Start + this.Client.Addresses.Vip.Step * i;
                c.ID = this.Client.Memory.ReadUInt32(address + this.Client.Addresses.Vip.Distances.ID);
                c.Name = this.Client.Memory.ReadString(address + this.Client.Addresses.Vip.Distances.Name);
                c.Online = this.Client.Memory.ReadBool(address + this.Client.Addresses.Vip.Distances.Online);
                c.Icon = (Constants.Vip.Icons)this.Client.Memory.ReadByte(address + this.Client.Addresses.Vip.Distances.Icon);

                if (onlineOnly && !c.Online) break;

                yield return c;
            }
        }
        public IEnumerable<Character> GetCharacters(Constants.Vip.Icons icon, bool onlineOnly = false)
        {
            foreach (Character c in this.GetCharacters(onlineOnly))
            {
                if (c.Icon != icon) continue;
                yield return c;
            }
        }
        public void Add(string name)
        {
            this.Client.Packets.VipAdd(name);
        }
        public void Remove(Character c)
        {
            this.Client.Packets.VipRemove(c.ID);
        }
    }
}
