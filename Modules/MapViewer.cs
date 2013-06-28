// NOTE: Not finished

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;

namespace KarelazisBot.Modules
{
    public class MapViewer
    {
        public MapViewer(Objects.Client c)
        {
            this.Client = c;
            string dir = c.TibiaProcess.MainModule.FileName;
            this.TibiaDirectory = dir.Substring(0, dir.LastIndexOf("\\") + 1);
            this.TibiaDirectory += this.TibiaDirectory.EndsWith("\\") ? string.Empty : "\\";
            this.CacheDirectory = "mapfiledata";
            this.Location = (this.Client.Player.Connected ? this.Client.Player.Location : new Objects.Location());
        }

        private bool HasGeneratedCache { get; set; }
        private readonly int MapFileDimension = 256;
        /// <summary>
        /// Used to determine the minimum X-axis value (pre-multiplication) for Cipsoft's map.
        /// This value is used to read map file names.
        /// </summary>
        private readonly ushort MapFileMinimumX = 120;
        private readonly byte FloorMin = 0, FloorMax = 15;
        /// <summary>
        /// The directory of the Tibia client.
        /// </summary>
        public string TibiaDirectory { get; private set; }
        public string CacheDirectory { get; private set; }
        private readonly string MaskOT = "0??0??";
        private readonly string MaskReal = "1??1??";
        public Objects.Client Client { get; private set; }
        public Objects.Location Location { get; set; }

        #region private methods
        private IEnumerable<MapFile> GetFloor(Objects.Location worldLocation)
        {
            string[] files = Directory.GetFiles(this.TibiaDirectory,
                ((worldLocation.X / this.MapFileDimension) > this.MapFileMinimumX ? this.MaskReal : this.MaskOT) +
                worldLocation.Z.ToString("00") + ".map");
            foreach (string s in files) yield return new MapFile(new FileInfo(s));
        }
        private void SaveFloor(Objects.Location worldLocation)
        {
            List<MapFile> mapFiles = new List<MapFile>(this.GetFloor(worldLocation));
            Rectangle r = this.GetBounds(mapFiles);
            if (r.Width == 0 || r.Height == 0) return;
            Bitmap bmap = new Bitmap(r.Width, r.Height);
            foreach (MapFile mfile in mapFiles)
            {
                if (!mfile.File.Exists)
                {
                    for (int x = 0; x < this.MapFileDimension; x++)
                    {
                        for (int y = 0; y < this.MapFileDimension; y++)
                        {
                            bmap.SetPixel(mfile.Location.X - r.Left + x,
                                mfile.Location.Y - r.Top + y,
                                Color.Black);
                        }
                    }
                }
                using (FileStream fstream = mfile.File.OpenRead())
                {
                    for (int x = 0; x < this.MapFileDimension; x++)
                    {
                        for (int y = 0; y < this.MapFileDimension; y++)
                        {
                            bmap.SetPixel(mfile.Location.X - r.Left + x,
                                mfile.Location.Y - r.Top + y,
                                this.GetAutomapColor(fstream.ReadByte()));
                        }
                    }
                }
            }
            MemoryStream memStream = new MemoryStream();
            bmap.Save(memStream, ImageFormat.Png);
            this.Client.Modules.CacheManager.SetFile(this.CacheDirectory + "\\" + worldLocation.Z + ".png", memStream.ToArray());
            memStream.Dispose();
            bmap.Dispose();
        }
        private void SaveAllFloors()
        {
            for (int i = this.FloorMin; i <= this.FloorMax; i++)
            {
                // save Cipsoft minimap files
                var t = new System.Threading.Thread(delegate()
                    {
                        try { this.SaveFloor(new Objects.Location(32000, 32000, i)); }
                        catch (Exception ex) { System.Windows.Forms.MessageBox.Show(ex.Message + "\n" + ex.StackTrace); }
                    });
                t.Start();
                // save OpenTibia minimap files
                var t2 = new System.Threading.Thread(delegate()
                    {
                        try { this.SaveFloor(new Objects.Location(0, 0, i)); }
                        catch (Exception ex) { System.Windows.Forms.MessageBox.Show(ex.Message + "\n" + ex.StackTrace); }
                    });
                t2.Start();
                System.Threading.Thread.Sleep(100);
            }
        }
        private Rectangle GetBounds(IEnumerable<MapFile> mapFiles)
        {
            int startX = -1,
                startY = -1,
                endX = -1,
                endY = -1;
            foreach (MapFile mapFile in mapFiles)
            {
                if (!mapFile.File.Exists) continue;
                if (startX == -1 || mapFile.Location.X < startX) startX = mapFile.Location.X;
                if (endX == -1 || mapFile.Location.X > endX) endX = mapFile.Location.X;
                if (startY == -1 || mapFile.Location.Y < startY) startY = mapFile.Location.Y;
                if (endY == -1 || mapFile.Location.Y > endY) endY = mapFile.Location.Y;
            }
            if (startX == -1 || endX == -1 || startY == -1 || endY == -1) return new Rectangle();
            return new Rectangle(startX, startY, endX - startX + this.MapFileDimension, endY - startY + this.MapFileDimension);
        }
        #endregion

        #region public methods
        public Color GetAutomapColor(int i)
        {
            switch (i)
            {
                case 0x0C: // Foliage, dark green
                    return Color.FromArgb(0, 0x66, 0);
                case 0x18: // Grass, green
                    return Color.FromArgb(0, 0xcc, 0);
                case 0x1e: // Swamp, bright green
                    return Color.FromArgb(0, 0xFF, 0);
                case 0x28: // Water, blue
                    return Color.FromArgb(0x33, 0, 0xcc);
                case 0x56: // Stone wall, dark grey
                    return Color.FromArgb(0x66, 0x66, 0x66);
                case 0x72: // Not sure, maroon
                    return Color.FromArgb(0x99, 0x33, 0);
                case 0x79: // Dirt, brown
                    return Color.FromArgb(0x99, 0x66, 0x33);
                case 0x81: // Paths, tile floors, other floors
                    return Color.FromArgb(0x99, 0x99, 0x99);
                case 0xB3: // Ice, light blue
                    return Color.FromArgb(0xcc, 0xff, 0xff);
                case 0xBA: // Walls, red
                    return Color.FromArgb(0xff, 0x33, 0);
                case 0xC0: // Lava, orange
                    return Color.FromArgb(0xff, 0x66, 0);
                case 0xCF: // Sand, tan
                    return Color.FromArgb(0xff, 0xcc, 0x99);
                case 0xD2: // Ladder, yellow
                    return Color.FromArgb(0xff, 0xff, 0);
                case 0: // Nothing, black
                    return Color.Black;
                default: // Unknown, white
                    return Color.White;
            }
        }
        /// <summary>
        /// Checks whether a color (from image) is walkable.
        /// </summary>
        /// <param name="color"></param>
        /// <returns></returns>
        public bool IsWalkable(Color color)
        {
            if (color == Color.FromArgb(0, 0xcc, 0) || color == Color.FromArgb(0x99, 0x66, 0x33) ||
                color == Color.FromArgb(0x99, 0x99, 0x99) || color == Color.FromArgb(0xcc, 0xff, 0xff) ||
                color == Color.FromArgb(0xff, 0xcc, 0x99) || color == Color.FromArgb(0xff, 0xff, 0))
            {
                return true;
            }
            return false;
        }
        /// <summary>
        /// Checks whether a color byte (from *.map file) is walkable.
        /// </summary>
        /// <param name="colorByte"></param>
        /// <returns></returns>
        public bool IsWalkable(byte colorByte)
        {
            switch (colorByte)
            {
                case 0x0C: // Foliage, dark green
                case 0x1e: // Swamp, bright green
                case 0x28: // Water, blue
                case 0x56: // Stone wall, dark grey
                case 0x72: // Not sure, maroon
                case 0xBA: // Walls, red
                case 0xC0: // Lava, orange
                case 0: // Nothing, black
                default: // Unknown, white
                    return false;
                case 0x18: // Grass, green
                case 0x79: // Dirt, brown
                case 0x81: // Paths, tile floors, other floors
                case 0xB3: // Ice, light blue
                case 0xCF: // Sand, tan
                case 0xD2: // Ladder, yellow
                    return true;
            }
        }
        /// <summary>
        /// Checks if a world location is walkable.
        /// </summary>
        /// <param name="loc">A world location.</param>
        /// <returns></returns>
        public bool IsWalkable(Objects.Location loc)
        {
            MapFile mapFile = this.GetAlignedMapFile(loc);
            if (mapFile == null) return false;
            bool isWalkable = false;
            using (FileStream fstream = mapFile.File.OpenRead())
            {
                int index = loc.Y - mapFile.Location.Y;
                index += (loc.X - mapFile.Location.X) * this.MapFileDimension;
                fstream.Seek(index, SeekOrigin.Begin);
                isWalkable = this.IsWalkable((byte)fstream.ReadByte());
            }
            return isWalkable;
        }
        public void GenerateCache()
        {
            this.SaveAllFloors();
        }
        public MapFile GetAlignedMapFile(ushort x, ushort y, byte z)
        {
            return this.GetAlignedMapFile(new Objects.Location(x, y, z));
        }
        public MapFile GetAlignedMapFile(Objects.Location location)
        {
            Point pt = new Point(location.X, location.Y);
            foreach (var mapFile in this.GetFloor(location))
            {
                if (mapFile.Bounds.Contains(pt)) return mapFile;
            }
            return null;
        }
        /// <summary>
        /// Gets a FileInfo object containing a *.map file. Does not align locations.
        /// </summary>
        /// <param name="x">X-axis of the location.</param>
        /// <param name="y">Y-axis of the location.</param>
        /// <param name="z">Z-axis of the location.</param>
        /// <returns></returns>
        public FileInfo GetMapFile(ushort x, ushort y, byte z)
        {
            return new FileInfo(Path.Combine(this.TibiaDirectory,
                (x / this.MapFileDimension).ToString("000") +
                (y / this.MapFileDimension).ToString("000") +
                z.ToString("00") + ".map"));
        }
        /// <summary>
        /// Gets a FileInfo object containing a *.map file. Does not align locations.
        /// </summary>
        /// <param name="location">A world location.</param>
        /// <returns></returns>
        public FileInfo GetMapFile(Objects.Location location)
        {
            return this.GetMapFile((ushort)location.X, (ushort)location.Y, (byte)location.Z);
        }
        /// <summary>
        /// Reads a list of *.map files and returns an image based on its contents.
        /// </summary>
        /// <param name="mapFiles">A list of MapFile objects.</param>
        /// <returns></returns>
        public Image GetImage(IEnumerable<MapFile> mapFiles)
        {
            // get boundaries
            int minX = -1, minY = -1, maxX = -1, maxY = -1;
            foreach (MapFile mapFile in mapFiles)
            {
                if (!mapFile.File.Exists) continue;
                if (minX == -1 || mapFile.Location.X < minX) minX = mapFile.Location.X;
                if (maxX == -1 || mapFile.Location.X > maxX) maxX = mapFile.Location.X;
                if (minY == -1 || mapFile.Location.Y < minY) minY = mapFile.Location.Y;
                if (maxY == -1 || mapFile.Location.Y > minY) maxY = mapFile.Location.Y;
            }
            // get image size
            int width = maxX - minX + this.MapFileDimension,
                height = maxY - minY + this.MapFileDimension;
            Bitmap bmap = new Bitmap(width, height);
            // draw stuff
            using (Graphics g = Graphics.FromImage(bmap))
            {
                foreach (MapFile mapFile in mapFiles)
                {
                    if (!mapFile.File.Exists) continue;
                    int x = mapFile.Location.X - minX,
                        y = mapFile.Location.Y - minY;
                    g.DrawImageUnscaledAndClipped(this.GetImageFromFile(mapFile.File),
                        new Rectangle(x, y, this.MapFileDimension, this.MapFileDimension));
                }
            }
            return bmap;
        }
        /// <summary>
        /// Reads a *.map file and returns an image based on its contents.
        /// </summary>
        /// <param name="mapFile">A MapFile object.</param>
        /// <returns></returns>
        public Image GetImage(MapFile mapFile)
        {
            return this.GetImage(new List<MapFile>() { mapFile });
        }
        /// <summary>
        /// Reads a *.map file and returns an image based on its contents.
        /// </summary>
        /// <param name="mapFile">a FileInfo object.</param>
        /// <returns></returns>
        public Image GetImageFromFile(FileInfo mapFile)
        {
            Bitmap bmap = new Bitmap(this.MapFileDimension, this.MapFileDimension, PixelFormat.Format16bppArgb1555);
            // return empty map if it doesn't exist
            if (mapFile == null || !mapFile.Exists) return bmap;
            using (FileStream fstream = mapFile.OpenRead())
            {
                for (int x = 0; x < this.MapFileDimension; x++)
                {
                    for (int y = 0; y < this.MapFileDimension; y++)
                    {
                        bmap.SetPixel(x, y, this.GetAutomapColor(fstream.ReadByte()));
                    }
                }
            }
            return bmap;
        }
        public Image GetImage(Objects.Location worldLocation)
        {
            if (worldLocation.Z > this.FloorMax || worldLocation.Z < this.FloorMin) return null;
            FileInfo fi = this.Client.Modules.CacheManager.GetFile(worldLocation.Z + ".png", this.CacheDirectory);
            if (fi == null)
            {
                if (!this.HasGeneratedCache) this.GenerateCache();
                else return null;
                this.HasGeneratedCache = true;
                return this.GetImage(worldLocation);
            }
            return Image.FromFile(fi.FullName);
        }
        /// <summary>
        /// Gets a rectangle that represents the bounds of a floor.
        /// </summary>
        /// <param name="worldLocation">The Objects.Location object used to determine whether to use OpenTibia minimap files or Cipsoft minimap files.</param>
        /// <returns></returns>
        public Rectangle GetBounds(Objects.Location worldLocation)
        {
            return this.GetBounds(this.GetFloor(worldLocation));
        }
        public void DrawExtras(Graphics g, Rectangle mapBounds, Rectangle viewport, byte floor, float scale,
            bool drawCreatures, bool drawPlayersAndNPCs, bool drawWaypoints, bool drawPlayerCross)
        {
            if (g == null || mapBounds == null || viewport == null) return;

            if (drawWaypoints)
            {
                foreach (Modules.Cavebot.Waypoint wp in this.Client.Modules.Cavebot.GetWaypoints())
                {
                    // disregard waypoints on other floors
                    if (wp.Location.Z != floor) continue;
                    // get map location
                    Point pt = new Point(wp.Location.X - mapBounds.X, wp.Location.Y - mapBounds.Y);
                    if (viewport.Contains(pt))
                    {
                        // get position relative to viewport and scale
                        pt = new Point((int)((pt.X - viewport.Left) * scale),
                            (int)((pt.Y - viewport.Top) * scale));
                        // draw circle
                        int ellipseSize = 10;
                        g.FillEllipse(Brushes.SkyBlue, new Rectangle(pt.X - ellipseSize / 2, pt.Y - ellipseSize / 2,
                            ellipseSize, ellipseSize));
                        // draw outline
                        g.DrawEllipse(Pens.Black, new Rectangle(pt.X - ellipseSize / 2, pt.Y - ellipseSize / 2,
                            ellipseSize, ellipseSize));
                    }
                }
            }

            if (drawCreatures || drawPlayersAndNPCs)
            {
                uint playerID = this.Client.Player.ID;
                foreach (Objects.Creature c in this.Client.BattleList.GetCreatures(true, true))
                {
                    // disregard creatures on other floors
                    if (c.Z != floor) continue;
                    // disregard the player
                    if (c.ID == playerID) continue;
                    // disregard what we don't want to draw
                    if ((drawPlayersAndNPCs && !drawCreatures && c.Type != Objects.Creature.Types.Player) ||
                        (drawCreatures && !drawPlayersAndNPCs && c.Type != Objects.Creature.Types.Creature))
                    {
                        continue;
                    }
                    // get map location
                    Point pt = new Point(c.X - mapBounds.X, c.Y - mapBounds.Y);
                    if (viewport.Contains(pt))
                    {
                        // get position relative to viewport and scale
                        pt = new Point((int)((pt.X - viewport.Left) * scale),
                            (int)((pt.Y - viewport.Top) * scale));
                        // draw a cross, color depends on creature type
                        Pen pen = c.Type == Objects.Creature.Types.Creature ? Pens.Red : Pens.SaddleBrown;
                        g.DrawLine(pen, new Point(pt.X - (int)(25 / scale), pt.Y),
                                new Point(pt.X + (int)(25 / scale), pt.Y));
                        g.DrawLine(pen, new Point(pt.X, pt.Y - (int)(25 / scale)),
                                new Point(pt.X, pt.Y + (int)(25 / scale)));
                    }
                }
            }

            if (drawPlayerCross)
            {
                Objects.Location playerLoc = this.Client.Player.Location;
                if (playerLoc.Z != floor) return;
                // get map location
                Point pt = new Point(playerLoc.X - mapBounds.X, playerLoc.Y - mapBounds.Y);
                if (viewport.Contains(pt))
                {
                    pt = new Point((int)((pt.X - viewport.Left) * scale),
                        (int)((pt.Y - viewport.Top) * scale));
                    g.DrawLine(Pens.White, new Point(pt.X - (int)(25 / scale), pt.Y),
                        new Point(pt.X + (int)(25 / scale), pt.Y));
                    g.DrawLine(Pens.White, new Point(pt.X, pt.Y - (int)(25 / scale)),
                        new Point(pt.X, pt.Y + (int)(25 / scale)));
                }
            }
        }
        #endregion

        /// <summary>
        /// A class used to ease the calculations and use of Tibia's *.map files.
        /// </summary>
        public class MapFile
        {
            public MapFile(FileInfo fi)
            {
                this.File = fi;
                if (fi == null || !fi.Exists) return;
                string fileName = Path.GetFileNameWithoutExtension(fi.FullName);
                ushort x = (ushort)(ushort.Parse(fileName.Substring(0, 3)) * 256),
                       y = (ushort)(ushort.Parse(fileName.Substring(3, 3)) * 256);
                byte z = byte.Parse(fileName.Substring(6, 2));
                this.Location = new Objects.Location(x, y, z);
                this.Size = new Size(256, 256);
                this.Bounds = new Rectangle(this.Location.X, this.Location.Y, this.Size.Width, this.Size.Height);
            }

            /// <summary>
            /// Gets the location in global/world coordinates (i.e. 32000, 32000, 7). Points to the top left in an image.
            /// </summary>
            public Objects.Location Location { get; private set; }
            /// <summary>
            /// Gets the size of this *.map file.
            /// </summary>
            public Size Size { get; private set; }
            /// <summary>
            /// Gets the bounds of this *.map file. Does not include Z axis.
            /// </summary>
            public Rectangle Bounds { get; private set; }
            /// <summary>
            /// Gets the FileInfo object that corresponds to the *.map file.
            /// </summary>
            public FileInfo File { get; private set; }
        }
    }
}
