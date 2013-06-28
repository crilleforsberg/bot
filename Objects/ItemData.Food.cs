using System.Drawing;

namespace KarelazisBot.Objects
{
    public partial class ItemData
    {
        public class Food : ItemData
        {
            public Food(string name, ushort id, float weight, bool stackable, uint regenTime, Image sprite = null)
                : base(name, id, weight, stackable, sprite)
            {
                this.RegenerationTime = regenTime;
            }

            /// <summary>
            /// Gets how many seconds of regeneration a single use of this item gives.
            /// </summary>
            public uint RegenerationTime { get; private set; }
        }
    }
}
