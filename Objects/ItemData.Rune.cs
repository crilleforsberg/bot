using System.Drawing;

namespace KarelazisBot.Objects
{
    public partial class ItemData
    {
        public class Rune : ItemData
        {
            public Rune(string name, ushort id, float weight, bool stackable, ushort charges, ushort manaToMake, Image sprite = null)
                : base(name, id, weight, stackable, sprite)
            {
                this.Charges = charges;
                this.ManaToMake = manaToMake;
            }

            public ushort Charges { get; private set; }
            public ushort ManaToMake { get; private set; }
        }
    }
}
