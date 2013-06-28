using System.Drawing;

namespace KarelazisBot.Objects
{
    public partial class ItemData
    {
        public ItemData(string name, ushort id, float weight, bool stackable, Image sprite = null)
        {
            this.ID = id;
            this.Weight = weight;
            this.IsStackable = stackable;
            this.Sprite = sprite;
        }

        public string Name { get; private set; }
        public ushort ID { get; private set; }
        public float Weight { get; private set; }
        public bool IsStackable { get; private set; }
        public Image Sprite { get; set; }
    }
}
