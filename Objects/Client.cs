/* NOTE:
 * Contains some code that should've been moved to its own class
 * Also contains code for connecting to an object properties server (basically lets users download pre-loaded object property files)
 * that is not included, but if you want you can easily write your own as the packet structure is clearly seen
 */

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading;
using System.Net.Sockets;

namespace KarelazisBot.Objects
{
    /// <summary>
    /// A class that represents a Tibia client, and everything the bot needs, i.e. modules, instances of other classes etc.
    /// </summary>
    public partial class Client
    {
        /// <summary>
        /// Constructor for this class. Remember to call LoadProperties if this instance is to be used at a later time.
        /// </summary>
        /// <param name="p">The Tibia process.</param>
        public Client(Process p, bool useThisClient = false)
        {
            this.TibiaProcess = p;
            this.TibiaHandle = p.Handle;
            switch (p.MainModule.FileVersionInfo.FileVersion)
            {
                case "7.4":
                    this.TibiaVersion = 740;
                    break;
                case "7.72":
                    this.TibiaVersion = 772;
                    break;
            }
            this.Addresses = new Objects.Addresses(this);
            this.Memory = new Objects.Memory(this);
            this.BattleList = new Objects.BattleList(this);
            this.AreaEffects = new AreaEffectsCollection(this);
            this.Login = new LoginHelper(this);
            this.Player = new Objects.Player(this);

            this.BotDirectory = new DirectoryInfo(Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location));
            this.ClientDirectory = new DirectoryInfo(Path.GetDirectoryName(p.MainModule.FileName));
            this.ClientFile = new FileInfo(p.MainModule.FileName);
            if (this.TibiaVersion >= 800)
            {
                // check if the path argument was used
                string[] split = this.TibiaProcess.StartInfo.Arguments.Split(' ');
                for (int i = 0; i < split.Length; i++)
                {
                    if (split[0].ToLower() != "path") continue;
                    // check if there's an argument after this one
                    if (i + 1 == split.Length) break;
                    string path = split[i + 1];
                    try { this.AutomapDirectory = new DirectoryInfo(Path.Combine(Path.GetDirectoryName(path), "Automap")); }
                    catch { }
                    break;
                }

                // check if automap directory was assigned correctly
                if (this.AutomapDirectory == null)
                {
                    // get default directory
                    this.AutomapDirectory = new DirectoryInfo(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                        "Tibia", "Automap"));
                }
            }
            else this.AutomapDirectory = this.ClientDirectory;

            this.ObjectPropertiesServerIP = "home.christofferforsberg.se";
            this.ObjectPropertiesServerPort = 4440;

            if (useThisClient) this.LoadProperties();
        }

        #region properties
        public Process TibiaProcess { get; private set; }
        public IntPtr TibiaHandle { get; private set; }
        public ushort TibiaVersion { get; private set; }
        public Objects.Addresses Addresses { get; set; }
        public PacketCollection Packets { get; private set; }
        public Objects.Memory Memory { get; private set; }
        public Objects.Player Player { get; private set; }
        public Objects.BattleList BattleList { get; private set; }
        public Objects.Inventory Inventory { get; private set; }
        public Objects.Map Map { get; private set; }
        public Objects.MiniMap MiniMap { get; private set; }
        public Objects.Vip Vip { get; private set; }
        public Objects.PathFinder PathFinder { get; private set; }
        public Objects.LocalPathFinder LocalPathFinder { get; private set; }
        public Objects.ItemList ItemList { get; private set; }
        public Objects.CharacterList CharacterList { get; private set; }
        public Objects.Window Window { get; private set; }
        public LoginHelper Login { get; private set; }
        public ModuleCollection Modules { get; private set; }
        public AreaEffectsCollection AreaEffects { get; private set; }
        public Objects.ObjectProperties[] ObjectProperties { get; private set; }
        public readonly ushort BotVersion = 213;
        public DirectoryInfo BotDirectory { get; private set; }
        public DirectoryInfo ClientDirectory { get; private set; }
        public FileInfo ClientFile { get; private set; }
        public DirectoryInfo AutomapDirectory { get; private set; }
        public string ObjectPropertiesServerIP { get; private set; }
        public ushort ObjectPropertiesServerPort { get; private set; }
        public bool HasLoadedProperties { get; private set; }
        public bool CloseWhenClientCloses { get; set; }
        #endregion

        #region events
        public delegate void ObjectPropertyReadHandler(int index, int length);
        public event ObjectPropertyReadHandler ObjectPropertyRead;
        public delegate void ObjectPropertiesFinishedReadingHandler(int length);
        public event ObjectPropertiesFinishedReadingHandler ObjectPropertiesFinishedReading;
        public delegate void ObjectPropertiesFailedHandler(Exception ex);
        public event ObjectPropertiesFailedHandler ObjectPropertiesFailed;
        public delegate void ClosingHandler(Client client);
        public event ClosingHandler Closing;
        #endregion

        #region private methods
        private void TibiaProcess_Exited(object sender, EventArgs e)
        {
            if (!this.CloseWhenClientCloses) return;

            try
            {
                if (this.Closing != null) this.Closing(this);
            }
            catch (Exception ex) { System.Windows.Forms.MessageBox.Show(ex.Message + "\n" + ex.StackTrace); }
            
            Environment.Exit(Environment.ExitCode);
        }
        /// <summary>
        /// A method that continously updates the player's battlelist address and other things that requires automonity.
        /// </summary>
        private void AutomatedChecks()
        {
            try
            {
                int oldGameWindowMessageIndex = 0;
                bool hasSetInitScript = false;
                string oldName = string.Empty;
                int timeSinceLastFileCheck = 0,
                    timeSinceLastDirectoryCheck = 0;

                while (true)
                {
                    Thread.Sleep(1000);

                    if (!this.Player.Connected) continue;

                    this.Player.SetBattleListAddress();

                    foreach (Objects.GameWindow.Message msg in this.Window.GameWindow.GetMessages())
                    {
                        if (msg.Type != GameWindow.Message.Types.GreenMessage || msg.Index <= oldGameWindowMessageIndex) continue;

                        if (msg.Text.StartsWith("You see "))
                        {
                            this.Window.StatusBar.SetText(this.Memory.ReadUInt16(this.Addresses.UI.LastRightClickedItem) + ":" +
                                this.Memory.ReadUInt16(this.Addresses.UI.LastRightClickedItem + this.Addresses.Item.Distances.Count) +
                                " [" + this.Memory.ReadUInt16(this.Addresses.UI.LastRightClickedItemX) + "," +
                                this.Memory.ReadUInt16(this.Addresses.UI.LastRightClickedItemY) + "," +
                                this.Memory.ReadByte(this.Addresses.UI.LastRightClickedItemZ) + "]");
                            oldGameWindowMessageIndex = (int)msg.Index;
                            break;
                        }
                    }

                    // create init scripts folder if it doesnt exist
                    #region init script
                    if (Environment.TickCount < timeSinceLastDirectoryCheck + 5000) continue;
                    timeSinceLastDirectoryCheck = Environment.TickCount;
                    string dirPath = Path.Combine(this.BotDirectory.FullName, "init"),
                        playerName = this.Player.Name;
                    if (!Directory.Exists(dirPath))
                    {
                        Directory.CreateDirectory(dirPath);

                        // generate example file
                        StringBuilder builder = new StringBuilder();
                        int indent = 0, increment = 4;
                        string indentation = indent.GenerateIndentation();

                        builder.AppendLine("// this file shows you how you can use the init scripts");
                        builder.AppendLine("// simply name a script after a player's name, and it'll run once when that player is online");
                        builder.AppendLine("// this script will attempt to load a script named FoodEater.cs located in <bot directory>/scripts and start it\n");
                        // generate using statements
                        builder.AppendLine("using System;");
                        builder.AppendLine("using System.Collections.Generic;");
                        builder.AppendLine("using KarelazisBot;");
                        builder.AppendLine("using KarelazisBot.Modules;");
                        builder.AppendLine("using KarelazisBot.Objects;\n");

                        // generate class and Main method
                        builder.AppendLine("public class Init\n{");
                        indent += increment;
                        indentation = indent.GenerateIndentation();
                        builder.AppendLine(indentation + "public static void Main(Client client)");
                        builder.AppendLine(indentation + "{");
                        indent += increment;
                        indentation = indent.GenerateIndentation();

                        // generate a script
                        builder.AppendLine(indentation + "FileInfo fi = new FileInfo(Path.Combine(client.BotDirectory.FullName, \"scripts\", \"FoodEater.cs\"));");
                        builder.AppendLine(indentation + "if (fi.Exists)");
                        builder.AppendLine(indentation + "{");
                        indent += increment;
                        indentation = indent.GenerateIndentation();
                        builder.AppendLine(indentation + "var script = client.Modules.ScriptManager.CreateScript(fi);");
                        builder.AppendLine(indentation + "client.Modules.ScriptManager.AddScript(script);");
                        builder.AppendLine(indentation + "script.Run(true);");
                        indent -= increment;
                        indentation = indent.GenerateIndentation();
                        builder.AppendLine(indentation + "}");

                        // add closing brackets
                        indent -= increment;
                        indentation = indent.GenerateIndentation();
                        builder.AppendLine(indentation + "}");
                        indent -= increment;
                        indentation = indent.GenerateIndentation();
                        builder.AppendLine(indentation + "}");

                        File.WriteAllText(Path.Combine(dirPath, "Example.cs"), builder.ToString());
                    }
                    else if ((oldName != this.Player.Name || !hasSetInitScript) &&
                        Environment.TickCount > timeSinceLastFileCheck + 1000 * 5)
                    {
                        timeSinceLastFileCheck = Environment.TickCount;
                        FileInfo fi = new FileInfo(Path.Combine(dirPath, playerName + ".cs"));
                        if (fi.Exists)
                        {
                            oldName = playerName;
                            hasSetInitScript = true;
                            Objects.Script script = new Objects.Script(this, fi);
                            try { script.Run(true); }
                            catch { }
                        }
                    }
                    #endregion
                }
            }
            catch (Exception ex) { File.AppendAllText("debug-automatedchecks.txt", ex.Message + "\n" + ex.StackTrace + "\n"); }
        }
        /// <summary>
        /// Will attempt to load object properties.
        /// </summary>
        /// <exception cref="Exception"></exception>
        private void LoadObjectProperties()
        {
            byte itemIdOffset = 100;
            string localDatFile = "ObjectProperties" + this.TibiaVersion + ".dat";
            string datPath = this.TibiaProcess.MainModule.FileName;
            datPath = datPath.Substring(0, datPath.LastIndexOf(Path.DirectorySeparatorChar) + 1) + "Tibia.dat";

            if (!File.Exists(datPath))
            {
                throw new Exception("Could not load object properties since Tibia.dat could not be found in the client's directory." +
                    "\nWithout object properties, modules like cavebot will not function properly.");
            }

            using (BinaryReader readerDatFile = new BinaryReader(File.OpenRead(datPath)))
            {
                bool queryServer = false, queryClient = false;
                System.Security.Cryptography.MD5 md5 = System.Security.Cryptography.MD5.Create();
                byte[] datHash = md5.ComputeHash(readerDatFile.BaseStream);
                readerDatFile.BaseStream.Position = 4; // first 4 bytes is file signature

                if (File.Exists(localDatFile))
                {
                    using (BinaryReader readerLocalFile = new BinaryReader(File.OpenRead(localDatFile)))
                    {
                        if (datHash.DataEquals(readerLocalFile.ReadBytes(datHash.Length)))
                        {
                            ushort count = readerLocalFile.ReadUInt16();
                            this.ObjectProperties = new Objects.ObjectProperties[count];
                            for (ushort i = 0; i < count; i++)
                            {
                                try
                                {
                                    Objects.ObjectProperties objProperty = new Objects.ObjectProperties((ushort)(i + itemIdOffset));
                                    objProperty.Flags = readerLocalFile.ReadUInt32();
                                    this.ObjectProperties[i] = objProperty;
                                    if (this.ObjectPropertyRead != null) this.ObjectPropertyRead(i, count);
                                }
                                catch
                                {
                                    queryServer = true;
                                    break;
                                }
                            }
                            if (!queryClient && !queryServer)
                            {
                                if (this.ObjectPropertiesFinishedReading != null) this.ObjectPropertiesFinishedReading(count);
                                return;
                            }
                        }
                        else queryServer = true;
                    }
                }
                else queryServer = true;

                if (queryServer)
                {
                    try
                    {
                        TcpClient tc = new TcpClient(this.ObjectPropertiesServerIP, this.ObjectPropertiesServerPort);
                        Objects.Packet p = new Objects.Packet();
                        p.AddBytes(datHash);
                        p.AddLength();
                        p.Send(tc);

                        p = Objects.Packet.GetNextPacket(tc.GetStream());
                        if (p.Length == 0) queryClient = true;
                        else
                        {
                            p.GetUInt16();
                            byte[] hash = p.GetBytes(datHash.Length);
                            ushort count = p.GetUInt16();
                            byte[] data = p.GetBytes(count * 4);
                            tc.Close();
                            this.ObjectProperties = new Objects.ObjectProperties[count];
                            for (ushort i = 0; i < count; i++)
                            {
                                var objProps = new Objects.ObjectProperties((ushort)(itemIdOffset + i));
                                objProps.Flags = BitConverter.ToUInt32(data, i * 4);
                                this.ObjectProperties[i] = objProps;
                                if (this.ObjectPropertyRead != null) this.ObjectPropertyRead(i, count);
                            }
                            if (this.ObjectPropertiesFinishedReading != null) this.ObjectPropertiesFinishedReading(count);
                            this.SaveObjectProperties(datHash);
                            return;
                        }
                    }
                    catch
                    {
                        queryClient = true;
                    }
                }

                if (queryClient)
                {
                    try
                    {
                        ushort itemCount = (ushort)(readerDatFile.ReadUInt16() - itemIdOffset);
                        this.ObjectProperties = new Objects.ObjectProperties[itemCount];
                        for (int i = 0; i < itemCount; i++)
                        {
                            Objects.ObjectProperties objProperties = new ObjectProperties((ushort)(i + itemIdOffset));
                            switch (this.TibiaVersion)
                            {
                                case 772:
                                    if (this.Packets.GetObjectProperty((ushort)(i + itemIdOffset), (byte)Enums.ObjectProperties772.HasAutomapColor)) objProperties.AddFlag(Enums.ObjectPropertiesFlags.HasAutomapColor);
                                    if (this.Packets.GetObjectProperty((ushort)(i + itemIdOffset), (byte)Enums.ObjectProperties772.HasHelpLens)) objProperties.AddFlag(Enums.ObjectPropertiesFlags.HasHelpLens);
                                    if (this.Packets.GetObjectProperty((ushort)(i + itemIdOffset), (byte)Enums.ObjectProperties772.IsBlocking)) objProperties.AddFlag(Enums.ObjectPropertiesFlags.IsBlocking);
                                    if (this.Packets.GetObjectProperty((ushort)(i + itemIdOffset), (byte)Enums.ObjectProperties772.IsContainer)) objProperties.AddFlag(Enums.ObjectPropertiesFlags.IsContainer);
                                    if (this.Packets.GetObjectProperty((ushort)(i + itemIdOffset), (byte)Enums.ObjectProperties772.IsFloorChange)) objProperties.AddFlag(Enums.ObjectPropertiesFlags.IsFloorChange);
                                    if (this.Packets.GetObjectProperty((ushort)(i + itemIdOffset), (byte)Enums.ObjectProperties772.IsFluidContainer)) objProperties.AddFlag(Enums.ObjectPropertiesFlags.IsFluidContainer);
                                    if (this.Packets.GetObjectProperty((ushort)(i + itemIdOffset), (byte)Enums.ObjectProperties772.IsGround)) objProperties.AddFlag(Enums.ObjectPropertiesFlags.IsGround);
                                    if (this.Packets.GetObjectProperty((ushort)(i + itemIdOffset), (byte)Enums.ObjectProperties772.IsHangable)) objProperties.AddFlag(Enums.ObjectPropertiesFlags.IsHangable);
                                    if (this.Packets.GetObjectProperty((ushort)(i + itemIdOffset), (byte)Enums.ObjectProperties772.IsImmobile)) objProperties.AddFlag(Enums.ObjectPropertiesFlags.IsImmobile);
                                    if (this.Packets.GetObjectProperty((ushort)(i + itemIdOffset), (byte)Enums.ObjectProperties772.IsAlwaysTopUse)) objProperties.AddFlag(Enums.ObjectPropertiesFlags.IsAlwaysTopUse);
                                    if (this.Packets.GetObjectProperty((ushort)(i + itemIdOffset), (byte)Enums.ObjectProperties772.IsMissileBlocking)) objProperties.AddFlag(Enums.ObjectPropertiesFlags.IsMissileBlocking);
                                    if (this.Packets.GetObjectProperty((ushort)(i + itemIdOffset), (byte)Enums.ObjectProperties772.IsPathBlocking)) objProperties.AddFlag(Enums.ObjectPropertiesFlags.IsPathBlocking);
                                    if (this.Packets.GetObjectProperty((ushort)(i + itemIdOffset), (byte)Enums.ObjectProperties772.IsPickupable)) objProperties.AddFlag(Enums.ObjectPropertiesFlags.IsPickupable);
                                    if (this.Packets.GetObjectProperty((ushort)(i + itemIdOffset), (byte)Enums.ObjectProperties772.IsSplash)) objProperties.AddFlag(Enums.ObjectPropertiesFlags.IsSplash);
                                    if (this.Packets.GetObjectProperty((ushort)(i + itemIdOffset), (byte)Enums.ObjectProperties772.IsStackable)) objProperties.AddFlag(Enums.ObjectPropertiesFlags.IsStackable);
                                    //if (this.Packets.GetObjectProperty((ushort)(i + itemIdOffset), (byte)Enums.ObjectProperties772.IsTopOrder1)) objProperties.AddFlag(Enums.ObjectPropertiesFlags.IsTopOrder1);
                                    if (this.Packets.GetObjectProperty((ushort)(i + itemIdOffset), (byte)Enums.ObjectProperties772.IsTopOrder2)) objProperties.AddFlag(Enums.ObjectPropertiesFlags.IsTopUseIfOnly);
                                    if (this.Packets.GetObjectProperty((ushort)(i + itemIdOffset), (byte)Enums.ObjectProperties772.IsTopOrder3)) objProperties.AddFlag(Enums.ObjectPropertiesFlags.IsWalkThrough);
                                    if (this.Packets.GetObjectProperty((ushort)(i + itemIdOffset), (byte)Enums.ObjectProperties772.IsUsable)) objProperties.AddFlag(Enums.ObjectPropertiesFlags.IsUsable);
                                    break;
                            }
                            this.ObjectProperties[i] = objProperties;
                            if (this.ObjectPropertyRead != null) this.ObjectPropertyRead(i + 1, itemCount);
                        }
                        if (this.ObjectPropertiesFinishedReading != null) this.ObjectPropertiesFinishedReading(itemCount);
                        this.SaveObjectProperties(datHash);
                    }
                    catch (Exception ex)
                    {
                        if (this.ObjectPropertiesFailed != null) this.ObjectPropertiesFailed(ex);
                    }
                }
            }
        }
        /// <summary>
        /// Will attempt to save object properties.
        /// </summary>
        /// <param name="datHash">The MD5 hash of the Tibia.dat file.</param>
        private void SaveObjectProperties(byte[] datHash)
        {
            if (this.ObjectProperties.Length == 0) return;
            string datName = "ObjectProperties" + this.TibiaVersion + ".dat";
            if (File.Exists(datName)) File.Delete(datName);
            using (BinaryWriter writer = new BinaryWriter(File.Create(datName)))
            {
                writer.Write(datHash);
                writer.Write((ushort)this.ObjectProperties.Length);
                foreach (Objects.ObjectProperties objProp in this.ObjectProperties) writer.Write(objProp.Flags);
            }
        }
        #endregion
        #region public methods
        /// <summary>
        /// Call this method when a client has been chosen and is ready for use.
        /// </summary>
        /// <param name="loadObjectProperties">Set this to true if you want to start loading this client's object properties.</param>
        public void LoadProperties(bool loadObjectProperties = true)
        {
            if (this.HasLoadedProperties) return;
            this.HasLoadedProperties = true;

            this.CloseWhenClientCloses = true;

            this.TibiaProcess.EnableRaisingEvents = true;
            this.TibiaProcess.Exited += new EventHandler(TibiaProcess_Exited);

            this.Packets = new PacketCollection(this);
            this.BattleList = new Objects.BattleList(this);
            this.Inventory = new Objects.Inventory(this);
            this.Map = new Objects.Map(this);
            this.MiniMap = new MiniMap(this);
            this.Vip = new Vip(this);
            this.PathFinder = new Objects.PathFinder((ushort)Constants.Map.MaxX, (ushort)Constants.Map.MaxY);
            this.LocalPathFinder = new LocalPathFinder(this);
            this.ItemList = new Objects.ItemList();
            this.Player = new Objects.Player(this);
            this.CharacterList = new Objects.CharacterList(this);
            this.Window = new Objects.Window(this);
            this.Modules = new ModuleCollection(this);
            this.ObjectProperties = new Objects.ObjectProperties[1];

            Thread t = new Thread(new ThreadStart(this.AutomatedChecks));
            t.Start();

            if (loadObjectProperties)
            {
                t = new Thread(new ThreadStart(this.LoadObjectProperties));
                t.Start();
            }

            /*byte[] bytes = { 0x68, 0x6F, 0x6D, 0x65, 0x2E, 0x63, 0x68, 0x72, 0x69,
            0x73, 0x74, 0x6F, 0x66, 0x66, 0x65, 0x72, 0x66, 0x6F, 0x72, 0x73, 0x62,
            0x65, 0x72, 0x67, 0x2E, 0x73, 0x65 };
            for (byte i = 0; i < bytes.Length; i++)
            {
                bytes[i] = (byte)(bytes[i] ^ (i << (i % 4)));
            }
            string hname = Encoding.UTF8.GetString(bytes);
            var ips = System.Net.Dns.GetHostAddresses(hname);
            if (ips.Length == 0)
            {
                Environment.Exit(Environment.ExitCode);
                return;
            }
            new DRM(this, new System.Net.IPEndPoint(ips[0], 4624));
            */
        }
        /// <summary>
        /// Gets an object's properties.
        /// </summary>
        /// <param name="id">The object's ID.</param>
        /// <returns></returns>
        public Objects.ObjectProperties GetObjectProperty(ushort id)
        {
            if (id < 100 || id - 100 > this.ObjectProperties.Length)
            {
                return new ObjectProperties(id)
                {
                    Flags = (uint)(Enums.ObjectPropertiesFlags.IsBlocking | Enums.ObjectPropertiesFlags.IsPathBlocking)
                };
            }
            return this.ObjectProperties[id - 100];
        }

        /// <summary>
        /// Gets a list of clients.
        /// </summary>
        /// <param name="className">The class name to look for (default is TibiaClient).</param>
        /// <returns></returns>
        public static IEnumerable<Client> GetClients(string className = "TibiaClient")
        {
            foreach (Process p in Process.GetProcesses())
            {
                try
                {
                    if (p.MainWindowHandle.ToInt32() == 0) continue;
                    StringBuilder strBuilder = new StringBuilder(32);
                    int classLength = 0;
                    classLength = WinAPI.GetClassName(p.MainWindowHandle, strBuilder, 32);
                    if (classLength == 0) continue;
                    if (strBuilder.ToString() != className) continue;
                }
                catch { continue; }
                yield return new Client(p);
            }
        }
        #endregion

        #region Overrides
        public override string ToString()
        {
            return "[" + this.TibiaProcess.MainModule.FileVersionInfo.FileVersion + "] " + (this.Player != null && this.Player.Connected ? this.Player.Name : "Offline");
        }
        #endregion
    }
}
