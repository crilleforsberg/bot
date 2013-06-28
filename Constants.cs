using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KarelazisBot
{
    public static class Constants
    {
        public static class Map
        {
            public const int MemoryLocationCenterX = 8,
                MemoryLocationCenterY = 6,
                TileNumberCenterIndex = 116,
                TileNumberCenterOffset = 116 * 4,
                MaxTiles = 2016,
                MaxX = 18,
                MaxY = 14,
                MaxZ = 8,
                MaxTilesPerFloor = 252;
        }

        public static class Inventory
        {
            public const int MinimumContainerNumber = 0x40,
                MaxContainers = 16;
        }

        public static class Vip
        {
            public enum Icons
            {
                Blank = 0,
                Heart = 1,
                Skull = 2,
                Lightning = 3,
                Crosshair = 4,
                Star = 5,
                YinYang = 6,
                Triangle = 7,
                X = 8,
                Dollar = 9,
                Cross = 10
            }
        }
    }
}
