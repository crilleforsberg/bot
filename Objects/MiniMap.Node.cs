using System.Runtime.InteropServices;

namespace KarelazisBot.Objects
{
    public partial class MiniMap
    {
        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct Node
        {
            public byte Color;
            public byte Speed;
        }
    }
}
