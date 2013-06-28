using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KarelazisBot.Objects
{
    public partial class ItemData
    {
        public class Container : ItemData
        {
            public Container(string name, ushort id, float weight, ushort capacity)
                : base(name, id, weight, false)
            {
                this.Capacity = capacity;
            }

            public ushort Capacity { get; private set; }
        }
    }
}
