using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KarelazisBot.Objects
{
    public partial class CreatureData
    {
        public class Loot
        {
            public Loot(string name, ushort itemID, bool stackable, uint worth)
            {
                this.Name = name;
                this.ItemID = itemID;
                this.Stackable = stackable;
            }

            public string Name { get; private set; }
            public ushort ItemID { get; set; }
            public bool Stackable { get; set; }
            public uint Worth { get; set; }

            public enum Chance
            {
                Always,
                Normal,
                SemiRare,
                Rare,
                VeryRare
            }
        }
    }
}
