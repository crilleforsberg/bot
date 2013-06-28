// NOTE:
// This module has constantly been changed over time, which resulted in quite a mess
// It currently runs on a single-threaded model, which is limited but it works most of the time
// There is also a multi-threaded model, but it is not finished
// If you plan on actually using this code, I'd recommend having scripts as delegates instead of a string,
// as well as re-working how cavebot scripts are handled

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using KarelazisBot.Objects;

namespace KarelazisBot.Modules
{
    /// <summary>
    /// A class that manages primarily automated hunting.
    /// </summary>
    public partial class Cavebot
    {
        #region constructors
        /// <summary>
        /// Constructor for this class.
        /// </summary>
        /// <param name="c">The client to be associated with this object.</param>
        public Cavebot(Objects.Client c)
        {
            this.Client = c;
            this.CurrentSettings = new Settings();
            this.Loots = new List<Loot>();
            this.Targets = new List<Target>();
            this.Waypoints = new List<Waypoint>();
            this.StopwatchFoodEater = new System.Diagnostics.Stopwatch();
            this.StopwatchExhaust = new System.Diagnostics.Stopwatch();
            this.PathFinder = new Objects.PathFinder((ushort)Constants.Map.MaxX, (ushort)Constants.Map.MaxY);
            //this.PathFinder = new Objects.LocalPathFinder(this.Client);//new Objects.PathFinder(c.Addresses.Map.MaxX, c.Addresses.Map.MaxY);
        }
        #endregion

        #region properties
        /// <summary>
        /// Gets the client associated with this object.
        /// </summary>
        public Objects.Client Client { get; private set; }
        /// <summary>
        /// Gets whether this object is running.
        /// </summary>
        public bool IsRunning { get; private set; }
        /// <summary>
        /// Gets the pathfinder that will be used for pathfinding.
        /// </summary>
        public Objects.PathFinder PathFinder { get; private set; }
        /// <summary>
        /// Gets or sets the settings used for this object.
        /// </summary>
        public Settings CurrentSettings { get; set; }
        /// <summary>
        /// Gets the stopwatch used to only eat food when an interval of time has passed since the last time food was eaten.
        /// </summary>
        public System.Diagnostics.Stopwatch StopwatchFoodEater { get; private set; }
        /// <summary>
        /// Gets the stopwatch used to avoid getting exhausted when casting spells and runes.
        /// </summary>
        public System.Diagnostics.Stopwatch StopwatchExhaust { get; private set; }
        private int _CurrentWaypointIndex { get; set; }
        /// <summary>
        /// Gets or sets the current waypoint index.
        /// </summary>
        public int CurrentWaypointIndex
        {
            get
            {
                if (this._CurrentWaypointIndex >= this.Waypoints.Count) this._CurrentWaypointIndex = 0;
                else if (this._CurrentWaypointIndex < 0) this._CurrentWaypointIndex = 0;
                return this._CurrentWaypointIndex;
            }
            set
            {
                if (value >= this.Waypoints.Count) this._CurrentWaypointIndex = 0;
                else if (this._CurrentWaypointIndex < 0) this._CurrentWaypointIndex = 0;
                else this._CurrentWaypointIndex = value;
            }
        }
        private List<Waypoint> Waypoints { get; set; }
        private List<Target> Targets { get; set; }
        private List<Loot> Loots { get; set; }
        private Thread threadCavebot { get; set; }
        /// <summary>
        /// Gets or sets the master thread, where the code which handles everything resides.
        /// </summary>
        private Thread MasterThread { get; set; }
        #endregion

        #region events
        public delegate void WaypointHandler(Waypoint waypoint);
        public event WaypointHandler WaypointAdded;
        public delegate void WaypointInsertedHandler(Waypoint waypoint, int index);
        public event WaypointInsertedHandler WaypointInserted;
        public event WaypointHandler WaypointRemoved;
        public delegate void WaypointIndexChangedHandler(int index);
        public event WaypointIndexChangedHandler WaypointIndexChanged;

        public delegate void LootHandler(Loot loot);
        public event LootHandler LootAdded;
        public event LootHandler LootRemoved;

        public delegate void TargetHandler(Target target);
        public event TargetHandler TargetAdded;
        public event TargetHandler TargetRemoved;
        public event TargetHandler TargetKilled;

        public delegate void ItemLootedHandler(Objects.Item item);
        public event ItemLootedHandler ItemLooted;
        public delegate void CorpseLootedHandler(Map.Tile tile);
        public event CorpseLootedHandler CorpseLooted; 
            
        public delegate void StatusChangedHandler(bool cavebotStatus);
        public event StatusChangedHandler StatusChanged;

        private delegate void CleanupHandler();
        private event CleanupHandler CleanupCalled;
        #endregion

        #region public methods
        /// <summary>
        /// Starts the cavebot thread.
        /// </summary>
        public void Start()
        {
            if (this.IsRunning) return;
            this.IsRunning = true;
            //this.CurrentWaypointIndex = 0;
            this.threadCavebot = new Thread(new ThreadStart(this.CavebotLogic));
            this.threadCavebot.Start();
            //Thread t = new Thread(this.Run);
            //t.Start();
            if (this.StatusChanged != null) this.StatusChanged(this.IsRunning);
        }
        /// <summary>
        /// Stops the cavebot thread.
        /// </summary>
        public void Stop()
        {
            if (!this.IsRunning) return;
            this.IsRunning = false;
            if (this.CleanupCalled != null) this.CleanupCalled();
            if (this.threadCavebot != null && this.threadCavebot.IsAlive) this.threadCavebot.Abort();
            if (this.StatusChanged != null) this.StatusChanged(this.IsRunning);
            this.CurrentWaypointIndex = 0;
        }
        /// <summary>
        /// Restarts the cavebot thread.
        /// </summary>
        public void Restart()
        {
            this.Stop();
            this.Start();
        }
        /// <summary>
        /// Gets an array of currently loaded Waypoint objects.
        /// </summary>
        /// <returns></returns>
        public IEnumerable<Waypoint> GetWaypoints() { return this.Waypoints.ToArray(); }
        /// <summary>
        /// Adds a Waypoint object.
        /// </summary>
        /// <param name="wp"></param>
        public void AddWaypoint(Waypoint wp)
        {
            this.Waypoints.Add(wp);
            if (this.WaypointAdded != null) this.WaypointAdded(wp);
        }
        /// <summary>
        /// Inserts a Waypoint object at a given index.
        /// </summary>
        /// <param name="wp"></param>
        /// <param name="index"></param>
        public void InsertWaypoint(Waypoint wp, int index)
        {
            if (index >= 0 && index < this.Waypoints.Count)
            {
                this.Waypoints.Insert(index, wp);
                if (this.WaypointInserted != null) this.WaypointInserted(wp, index);
            }
        }
        /// <summary>
        /// Attempts to remove the first found match for a Waypoint object.
        /// </summary>
        /// <param name="wp"></param>
        public void RemoveWaypoint(Waypoint wp)
        {
            if (this.Waypoints.Contains(wp))
            {
                this.Waypoints.Remove(wp);
                if (this.WaypointRemoved != null) this.WaypointRemoved(wp);
            }
        }
        /// <summary>
        /// Removes all waypoints.
        /// </summary>
        public void RemoveAllWaypoints()
        {
            foreach (Waypoint wp in this.GetWaypoints().ToArray<Waypoint>()) this.RemoveWaypoint(wp);
        }
        public Waypoint CreateWaypoint(Waypoint.Types type, Objects.Location location, string label = "")
        {
            switch (type)
            {
                case Waypoint.Types.Script:
                    return new Waypoint(this, location, string.Empty, label);
                case Waypoint.Types.Node:
                    return new Waypoint(this, location, Enumerable.Empty<Objects.Location>(), label);
                default:
                    return new Waypoint(this, location, type, label);
            }
        }
        /// <summary>
        /// Gets an array of Loot objects.
        /// </summary>
        /// <returns></returns>
        public IEnumerable<Loot> GetLoot() { return this.Loots.ToArray(); }
        /// <summary>
        /// Adds a Loot object.
        /// </summary>
        /// <param name="loot"></param>
        public void AddLoot(Loot loot)
        {
            this.Loots.Add(loot);
            if (this.LootAdded != null) this.LootAdded(loot);
        }
        /// <summary>
        /// Attempts to remove the first found match for a Loot object.
        /// </summary>
        /// <param name="loot"></param>
        public void RemoveLoot(Loot loot)
        {
            if (this.Loots.Contains(loot))
            {
                this.Loots.Remove(loot);
                if (this.LootRemoved != null) this.LootRemoved(loot);
            }
        }
        /// <summary>
        /// Removes all loot.
        /// </summary>
        public void RemoveAllLoot()
        {
            foreach (Loot l in this.GetLoot().ToArray<Loot>()) this.RemoveLoot(l);
        }
        /// <summary>
        /// Gets an array of Target objects.
        /// </summary>
        /// <returns></returns>
        public IEnumerable<Target> GetTargets() { return this.Targets.ToArray(); }
        /// <summary>
        /// Adds a Target object.
        /// </summary>
        /// <param name="t"></param>
        public void AddTarget(Target t)
        {
            this.Targets.Add(t);
            if (this.TargetAdded != null) this.TargetAdded(t);
        }
        /// <summary>
        /// Attempts to remove the first found match for a Target object.
        /// </summary>
        /// <param name="t"></param>
        public void RemoveTarget(Target t)
        {
            if (this.Targets.Contains(t))
            {
                this.Targets.Remove(t);
                if (this.TargetRemoved != null) this.TargetRemoved(t);
            }
        }
        /// <summary>
        /// Removes all targets.
        /// </summary>
        public void RemoveAllTargets()
        {
            foreach (Target t in this.GetTargets().ToArray<Target>()) this.RemoveTarget(t);
        }
        public void Clear()
        {
            this.RemoveAllLoot();
            this.RemoveAllTargets();
            this.RemoveAllWaypoints();
        }
        /// <summary>
        /// Saves the current data (waypoints, loot, targets, settings) to a file.
        /// </summary>
        /// <param name="fileName">The name of the file. Can be a full file path or a relative file path as well.</param>
        public void Save(FileInfo file)
        {
            if (file.Exists) file.Delete();

            StringBuilder builder = new StringBuilder();
            int indent = 0, increment = 4;
            string indentation = string.Empty;
            // generate using statements
            builder.AppendLine("using System;");
            builder.AppendLine("using System.Collections.Generic;");
            builder.AppendLine("using System.Text;");
            builder.AppendLine("using KarelazisBot;");
            builder.AppendLine("using KarelazisBot.Modules;");
            builder.AppendLine("using KarelazisBot.Objects;\n");
            builder.AppendLine("using KarelazisBot.Modules;\n");

            // generate class and Main method
            builder.AppendLine("public class CavebotScript\n{");
            indent += increment;
            indentation = indent.GenerateIndentation();
            builder.AppendLine(indentation + "public static void Main(Client client)");
            builder.AppendLine(indentation + "{");
            indent += increment;
            indentation = indent.GenerateIndentation();

            // generate settings
            builder.AppendLine(indentation + "// SETTINGS");
            builder.AppendLine(indentation + "#region settings");
            builder.AppendLine(indentation + "var setting = new Cavebot.Settings();");
            var setting = this.CurrentSettings;
            builder.AppendLine(indentation + "setting.DebugMode = " + setting.DebugMode.ToString().ToLower() + ";");
            builder.AppendLine(indentation + "setting.AllowDiagonalMovement = " + setting.AllowDiagonalMovement.ToString().ToLower() + ";");
            builder.AppendLine(indentation + "setting.CanUseMagicRope = " + setting.CanUseMagicRope.ToString().ToLower() + ";");
            builder.AppendLine(indentation + "setting.ConsiderAllMonstersWhenKeepingAway = " + setting.ConsiderAllMonstersWhenKeepingAway.ToString().ToLower() + ";");
            builder.AppendLine(indentation + "setting.EatFood = " + setting.EatFood.ToString().ToLower() + ";");
            builder.AppendLine(indentation + "setting.Exhaust = " + setting.Exhaust + ";");
            builder.AppendLine(indentation + "setting.FastLooting = " + setting.FastLooting.ToString().ToLower() + ";");
            builder.AppendLine(indentation + "setting.FriendlyMode = " + setting.FriendlyMode.ToString().ToLower() + ";");
            builder.AppendLine(indentation + "setting.KeepAwayPerimeter = " + setting.KeepAwayPerimeter + ";");
            builder.AppendLine(indentation + "setting.KillBeforeLooting = " + setting.KillBeforeLooting.ToString().ToLower() + ";");
            builder.AppendLine(indentation + "setting.MinimumHealthToShoot = " + setting.MinimumHealthToShoot + ";");
            builder.AppendLine(indentation + "setting.NodeRadius = " + setting.NodeRadius + ";");
            builder.AppendLine(indentation + "setting.NodeSkipRange = " + setting.NodeSkipRange + ";");
            builder.AppendLine(indentation + "setting.OpenContainers = " + setting.OpenContainers.ToString().ToLower() + ";");
            builder.AppendLine(indentation + "setting.PrioritizeDanger = " + setting.PrioritizeDanger.ToString().ToLower() + ";");
            builder.AppendLine(indentation + "setting.StickToCreature = " + setting.StickToCreature.ToString().ToLower() + ";");
            builder.AppendLine(indentation + "setting.StopAttackingWhenOutOfRange = " + setting.StopAttackingWhenOutOfRange.ToString().ToLower() + ";");
            builder.AppendLine(indentation + "setting.UseAlternateNodeFinder = " + setting.UseAlternateNodeFinder.ToString().ToLower() + ";");
            builder.AppendLine(indentation + "client.Modules.Cavebot.CurrentSettings = setting;");
            builder.AppendLine(indentation + "#endregion\n");

            // generate waypoints
            builder.AppendLine(indentation + "// WAYPOINTS");
            builder.AppendLine(indentation + "#region waypoints");
            builder.AppendLine(indentation + "var nodes = new List<Location>();");
            builder.AppendLine(indentation + "StringBuilder script = new StringBuilder();");
            foreach (Waypoint wp in this.GetWaypoints())
            {
                switch (wp.Type)
                {
                    case Waypoint.Types.Walk:
                    case Waypoint.Types.Shovel:
                    case Waypoint.Types.Rope:
                    case Waypoint.Types.Pick:
                    case Waypoint.Types.Machete:
                    case Waypoint.Types.Ladder:
                        builder.AppendLine(indentation +
                            "client.Modules.Cavebot.AddWaypoint(new Cavebot.Waypoint(" +
                            "client.Modules.Cavebot, " +
                            "new Location(" + wp.Location.ToString() + "), " +
                            "Cavebot.Waypoint.Types." + wp.Type.ToString() +
                            (!string.IsNullOrEmpty(wp.Label) ? ", \"" + wp.Label + "\"" : string.Empty) +
                            "));");
                        break;
                    case Waypoint.Types.Script:
                        /*foreach (string line in wp.Script.Code.Split('\n'))
                        {
                            builder.AppendLine(indentation + "script += \"" + line + "\"");
                        }*/
                        string parsedCode = wp.Script.Code.Replace("\\\"", "<ESCAPEDQUOTE>").Replace("\"", "\\\"")
                            .Replace("<ESCAPEDQUOTE>", "\\\"");
                        builder.AppendLine("\n" + indentation + "script = new StringBuilder();");
                        foreach (string line in parsedCode.Split(new string[] { Environment.NewLine }, StringSplitOptions.None))
                        {
                            builder.AppendLine(indentation + "script.AppendLine(\"" + line + "\");");
                        }
                        builder.AppendLine(indentation +
                            "client.Modules.Cavebot.AddWaypoint(new Cavebot.Waypoint(" +
                            "client.Modules.Cavebot, " +
                            "new Location(" + wp.Location.ToString() + "), script.ToString()" +
                            (!string.IsNullOrEmpty(wp.Label) ? ", \"" + wp.Label + "\"" : string.Empty) +
                            "));");
                        break;
                    case Waypoint.Types.Node:
                        builder.AppendLine("\n" + indentation + "nodes.Clear();");
                        builder.AppendLine(indentation + "#region node waypoints");
                        foreach (Objects.Location subNode in wp.NodeLocations)
                        {
                            builder.AppendLine(indentation + "nodes.Add(new Location(" + subNode.ToString() + "));");
                        }
                        builder.AppendLine(indentation + "#endregion");
                        builder.AppendLine(indentation +
                            "client.Modules.Cavebot.AddWaypoint(new Cavebot.Waypoint(" +
                            "client.Modules.Cavebot, " +
                            "new Location(" + wp.Location.ToString() + "), nodes" +
                            (!string.IsNullOrEmpty(wp.Label) ? ", \"" + wp.Label + "\"" : string.Empty) +
                            "));");
                        break;
                }
            }
            builder.AppendLine(indentation + "#endregion");

            // generate targets
            builder.AppendLine("\n" + indentation + "// TARGETS");
            builder.AppendLine(indentation + "#region targets");
            builder.AppendLine(indentation + "Cavebot.Target target = null;");
            builder.AppendLine(indentation + "Cavebot.Target.Setting targetSetting = null;");
            builder.AppendLine(indentation + "var targetSettings = new List<Cavebot.Target.Setting>();\n");
            foreach (Target t in this.GetTargets())
            {
                var settings = t.GetSettings().ToList<Target.Setting>();
                builder.AppendLine(indentation + "#region " + t.Name);
                builder.AppendLine(indentation + "targetSettings = new List<Cavebot.Target.Setting>();");
                builder.AppendLine(indentation + "target = new Cavebot.Target(client.Modules.Cavebot);");
                builder.AppendLine(indentation + "target.Name = \"" + t.Name + "\";");
                builder.AppendLine(indentation + "target.DoLoot = " + t.DoLoot.ToString().ToLower() + ";");
                for (int i = 0; i < settings.Count; i++)
                {
                    var targetSetting = settings[i];
                    builder.AppendLine();
                    builder.AppendLine(indentation + "targetSetting = new Cavebot.Target.Setting(target);");
                    builder.AppendLine(indentation + "targetSetting.Count = " + targetSetting.Count + ";");
                    builder.AppendLine(indentation + "targetSetting.DangerLevel = " + targetSetting.DangerLevel + ";");
                    builder.AppendLine(indentation + "targetSetting.DistanceRange = " + targetSetting.DistanceRange + ";");
                    builder.AppendLine(indentation + "targetSetting.FightMode = Enums.FightMode." + targetSetting.FightMode.ToString() + ";");
                    builder.AppendLine(indentation + "targetSetting.FightStance = Enums.FightStance." + targetSetting.FightStance.ToString() + ";");
                    builder.AppendLine(indentation + "targetSetting.MustBeReachable = " + targetSetting.MustBeReachable.ToString().ToLower() + ";");
                    builder.AppendLine(indentation + "targetSetting.MustBeShootable = " + targetSetting.MustBeShootable.ToString().ToLower() + ";");
                    builder.AppendLine(indentation + "targetSetting.Range = " + targetSetting.Range + ";");
                    builder.AppendLine(indentation + "targetSetting.Rune = " + "Cavebot.Target.Setting.Runes." + targetSetting.Rune.ToString() + ";");
                    builder.AppendLine(indentation + "targetSetting.Spell = " +
                        (string.IsNullOrEmpty(targetSetting.Spell) ? "string.Empty" : "\"" + targetSetting.Spell + "\"") + ";");
                    builder.AppendLine(indentation + "targetSetting.UseThisSetting = " + targetSetting.UseThisSetting.ToString().ToLower() + ";");
                    builder.AppendLine(indentation + "targetSettings.Add(targetSetting);");
                }
                builder.AppendLine(indentation + "target.SetSettings(targetSettings);");
                builder.AppendLine(indentation + "client.Modules.Cavebot.AddTarget(target);");
                builder.AppendLine(indentation + "#endregion");
            }
            builder.AppendLine(indentation + "#endregion\n");

            // generate loot
            builder.AppendLine("\n" + indentation + "// LOOT");
            builder.AppendLine(indentation + "#region loot");
            foreach (Loot loot in this.GetLoot())
            {
                builder.AppendLine(indentation + "client.Modules.Cavebot.AddLoot(new Cavebot.Loot(" +
                    loot.ID + ", " +
                    (string.IsNullOrEmpty(loot.Name) ? "string.Empty" : "\"" + loot.Name + "\"") + ", " +
                    loot.Cap + ", " +
                    "Cavebot.Loot.Destinations." + loot.Destination.ToString() + ", " +
                    loot.Index + "));");
            }
            builder.AppendLine(indentation + "#endregion");

            // add closing brackets
            indent -= increment;
            indentation = indent.GenerateIndentation();
            builder.AppendLine(indentation + "}");
            indent -= increment;
            indentation = indent.GenerateIndentation();
            builder.AppendLine(indentation + "}");
            using (StreamWriter writer = file.CreateText())
            {
                writer.Write(builder.ToString());
            }

            /*Objects.Packet p = new Objects.Packet();

            p.AddUInt16(this.Client.BotVersion);

            p.AddByte((byte)Enums.CavebotDatType.Settings);
            p.AddBool(this.CurrentSettings.EatFood);
            p.AddUInt16(this.CurrentSettings.Exhaust);
            p.AddUInt16(this.CurrentSettings.MinimumHealthToShoot);
            p.AddByte(this.CurrentSettings.NodeRadius);
            p.AddByte(this.CurrentSettings.NodeSkipRange);
            p.AddBool(this.CurrentSettings.OpenContainers);
            p.AddBool(this.CurrentSettings.PrioritizeDanger);
            p.AddBool(this.CurrentSettings.StickToCreature);
            p.AddBool(this.CurrentSettings.StopAttackingWhenOutOfRange);
            p.AddBool(this.CurrentSettings.UseGoldStacks);
            p.AddBool(this.CurrentSettings.CanUseMagicRope);
            p.AddBool(this.CurrentSettings.UseAlternateNodeFinder);
            p.AddBool(this.CurrentSettings.KillBeforeLooting);

            p.AddByte((byte)Enums.CavebotDatType.Targetting);
            p.AddUInt16((ushort)this.Targets.Count);
            foreach (Target t in this.GetTargets())
            {
                p.AddString(t.Name);
                p.AddUInt16((ushort)t.GetSettings().ToArray<Target.Setting>().Length);
                foreach (Target.Setting s in t.GetSettings())
                {
                    p.AddByte(s.Count);
                    p.AddByte(s.Range);
                    p.AddByte((byte)s.FightMode);
                    p.AddByte((byte)s.FightStance);
                    p.AddString(s.Ring);
                    p.AddString(s.Rune);
                    p.AddString(s.Spell);
                    p.AddByte(s.DangerLevel);
                    p.AddBool(s.FriendlyMode);
                    p.AddBool(s.MustBeReachable);
                    p.AddBool(s.MustBeShootable);
                    p.AddBool(s.DoLoot);
                    p.AddBool(s.UseThisSetting);
                }
            }

            p.AddByte((byte)Enums.CavebotDatType.Loot);
            p.AddUInt16((ushort)this.Loots.Count);
            foreach (Loot l in this.GetLoot())
            {
                p.AddUInt16(l.ID);
                p.AddString(string.Empty);
                p.AddUInt16((ushort)l.Cap);
                p.AddByte((byte)l.Destination);
                p.AddByte(l.Index);
            }

            p.AddByte((byte)Enums.CavebotDatType.Waypoints);
            p.AddUInt16((ushort)this.Waypoints.Count);
            foreach (Waypoint wp in this.GetWaypoints())
            {
                p.AddByte((byte)wp.Type);
                p.AddUInt16((ushort)wp.Location.X);
                p.AddUInt16((ushort)wp.Location.Y);
                p.AddByte((byte)wp.Location.Z);
                if (wp.Label == null) p.AddString(string.Empty);
                else p.AddString(wp.Label);
                if (wp.Type == Modules.Cavebot.Waypoint.Types.Script)
                {
                    p.AddString(wp.Script.Code);
                    //p.AddUInt16((ushort)wp.Scripts.Count);
                    //foreach (Objects.Script script in wp.Scripts) p.AddString(script.ScriptPathShort);
                }
                else if (wp.Type == Modules.Cavebot.Waypoint.Types.Node)
                {
                    p.AddUInt16((ushort)wp.NodeLocations.Count);
                    foreach (Objects.Location loc in wp.NodeLocations)
                    {
                        p.AddUInt16((ushort)loc.X);
                        p.AddUInt16((ushort)loc.Y);
                        p.AddByte((byte)loc.Z);
                    }
                }
            }

            using (System.IO.FileStream fstream = System.IO.File.Create(fileName))
            {
                fstream.Write(p.ToBytes(), 0, p.Length);
            }*/
        }
        /// <summary>
        /// Loads data (waypoints, loot, targets, settings) from a file.
        /// </summary>
        /// <param name="file">The file to load.</param>
        /// <returns>Returns true if file is found and is valid, false if not.</returns>
        public bool Load(FileInfo file)
        {
            if (!file.Exists) return false;

            if (file.Extension.ToLower() == ".cs")
            {
                Objects.Script script = new Objects.Script(this.Client, file);
                this.Clear();
                script.Run();
                return true;
            }
            else
            {
                Objects.Packet p = new Objects.Packet();
                using (FileStream fstream = file.OpenRead())
                {
                    byte[] buffer = new byte[fstream.Length];
                    fstream.Read(buffer, 0, buffer.Length);
                    p = new Objects.Packet(buffer);
                }
                // read metadata
                ushort version = p.GetUInt16();
                if (this.Load(version, p)) return true;
                this.RemoveAllWaypoints();
                this.RemoveAllLoot();
                this.RemoveAllTargets();
                this.CurrentSettings.LoadDefaults();
                return false;
            }
        }
        /// <summary>
        /// Loads data (waypoints, loot, targets, settings) from a binary file loaded into a Objects.Packet object.
        /// </summary>
        /// <param name="version">The version of LibreBot which created the file.</param>
        /// <param name="p">The Objects.Packet object.</param>
        /// <returns></returns>
        private bool Load(ushort version, Objects.Packet p)
        {
            while (p.GetPosition < p.Length)
            {
                Enums.CavebotDatType type = (Enums.CavebotDatType)p.GetByte();
                ushort count = 0;
                switch (type)
                {
                    case Enums.CavebotDatType.Settings:
                        switch (version)
                        {
                            case 212:
                            case 213:
                                this.CurrentSettings.EatFood = p.GetBool();
                                this.CurrentSettings.Exhaust = p.GetUInt16();
                                this.CurrentSettings.MinimumHealthToShoot = p.GetUInt16();
                                this.CurrentSettings.NodeRadius = p.GetByte();
                                this.CurrentSettings.NodeSkipRange = p.GetByte();
                                this.CurrentSettings.OpenContainers = p.GetBool();
                                this.CurrentSettings.PrioritizeDanger = p.GetBool();
                                this.CurrentSettings.StickToCreature = p.GetBool();
                                this.CurrentSettings.StopAttackingWhenOutOfRange = p.GetBool();
                                this.CurrentSettings.UseGoldStacks = p.GetBool();
                                this.CurrentSettings.CanUseMagicRope = p.GetBool();
                                this.CurrentSettings.UseAlternateNodeFinder = p.GetBool();
                                this.CurrentSettings.KillBeforeLooting = p.GetBool();
                                break;
                            default:
                                this.CurrentSettings.EatFood = p.GetBool();
                                this.CurrentSettings.Exhaust = p.GetUInt16();
                                this.CurrentSettings.MinimumHealthToShoot = p.GetUInt16();
                                this.CurrentSettings.NodeRadius = p.GetByte();
                                this.CurrentSettings.NodeSkipRange = p.GetByte();
                                this.CurrentSettings.OpenContainers = p.GetBool();
                                this.CurrentSettings.PrioritizeDanger = p.GetBool();
                                this.CurrentSettings.StickToCreature = p.GetBool();
                                this.CurrentSettings.StopAttackingWhenOutOfRange = p.GetBool();
                                this.CurrentSettings.UseGoldStacks = p.GetBool();
                                this.CurrentSettings.CanUseMagicRope = p.GetBool();
                                break;
                        }
                        break;
                    case Enums.CavebotDatType.Loot:
                        count = p.GetUInt16();
                        for (int i = 0; i < count; i++)
                        {
                            switch (version)
                            {
                                default:
                                    Loot l = new Loot();
                                    l.ID = p.GetUInt16();
                                    l.Name = p.GetString();
                                    l.Cap = p.GetUInt16();
                                    string destination = p.GetString();
                                    switch (destination.ToLower())
                                    {
                                        case "ground":
                                            l.Destination = Loot.Destinations.Ground;
                                            break;
                                        default:
                                            l.Destination = Loot.Destinations.EmptyContainer;
                                            l.Index = byte.Parse(destination[1].ToString());
                                            break;
                                    }
                                    this.AddLoot(l);
                                    break;
                                case 213:
                                    this.AddLoot(new Loot(p.GetUInt16(),
                                        p.GetString(),
                                        p.GetUInt16(),
                                        (Loot.Destinations)p.GetByte(),
                                        p.GetByte()));
                                    break;
                            }
                        }
                        break;
                    case Enums.CavebotDatType.Targetting:
                        count = p.GetUInt16();
                        for (int i = 0; i < count; i++)
                        {
                            Target t = new Target(this);
                            t.Name = p.GetString();
                            if (version < 212) // load old settings (single setting environment)
                            {
                                Target.Setting setting = new Target.Setting(t);
                                setting.Count = p.GetByte();
                                setting.Range = p.GetByte();
                                setting.DistanceRange = 3;
                                setting.FightMode = (Enums.FightMode)p.GetByte();
                                setting.FightStance = (Enums.FightStance)p.GetByte();
                                p.GetString(); // ring
                                p.GetString(); // rune
                                setting.Spell = p.GetString();
                                setting.DangerLevel = p.GetByte();
                                p.GetBool(); // friendly mode
                                setting.MustBeReachable = p.GetBool();
                                setting.MustBeShootable = p.GetBool();
                                t.DoLoot = p.GetBool();
                                setting.UseThisSetting = true;
                                p.GetBool(); // alarm, deprecated
                                t.SetSettings(new List<Target.Setting>() { setting });
                            }
                            else // load new settings (multiple setting environment)
                            {
                                ushort settingsCount = p.GetUInt16();
                                List<Target.Setting> settings = new List<Target.Setting>();
                                for (int j = 0; j < settingsCount; j++)
                                {
                                    Target.Setting setting = new Target.Setting(t);
                                    setting.Count = p.GetByte();
                                    setting.Range = p.GetByte();
                                    setting.DistanceRange = 3;
                                    setting.FightMode = (Enums.FightMode)p.GetByte();
                                    setting.FightStance = (Enums.FightStance)p.GetByte();
                                    p.GetString(); // ring
                                    p.GetString(); // rune
                                    setting.Spell = p.GetString();
                                    setting.DangerLevel = p.GetByte();
                                    p.GetBool(); // friendly mode
                                    setting.MustBeReachable = p.GetBool();
                                    setting.MustBeShootable = p.GetBool();
                                    t.DoLoot = p.GetBool();
                                    setting.UseThisSetting = true;
                                    settings.Add(setting);
                                }
                                t.SetSettings(settings);
                            }
                            this.AddTarget(t);
                        }
                        break;
                    case Enums.CavebotDatType.Waypoints:
                        count = p.GetUInt16();
                        for (int i = 0; i < count; i++)
                        {
                            Modules.Cavebot.Waypoint.Types wpType = (Modules.Cavebot.Waypoint.Types)p.GetByte();
                            Objects.Location wpLocation = new Objects.Location(p.GetUInt16(), p.GetUInt16(), p.GetByte());
                            string wpLabel = p.GetString();
                            List<Objects.Location> wpNodes = new List<Objects.Location>();
                            string wpScript = string.Empty;
                            switch (wpType)
                            {
                                case Modules.Cavebot.Waypoint.Types.Node:
                                    switch (version)
                                    {
                                        case 200:
                                            this.AddWaypoint(new Waypoint(this, wpLocation, wpType));
                                            break;
                                        case 201:
                                        default:
                                            ushort nodeCount = p.GetUInt16();
                                            for (int j = 0; j < nodeCount; j++) wpNodes.Add(new Objects.Location(p.GetUInt16(), p.GetUInt16(), p.GetByte()));
                                            this.AddWaypoint(new Waypoint(this, wpLocation, wpNodes));
                                            break;
                                    }
                                    break;
                                case Modules.Cavebot.Waypoint.Types.Script:
                                    if (version == 211)
                                    {
                                        ushort scriptCount = p.GetUInt16();
                                        List<Objects.Script> scripts = new List<Objects.Script>();
                                        string filePath = p.GetString();
                                        string contents = System.IO.File.Exists(filePath) ?
                                            System.IO.File.ReadAllText(filePath) :
                                            string.Empty;
                                        /*for (int j = 0; j < scriptCount; j++)
                                        {
                                            string fileName = p.GetString();
                                            if (!System.IO.File.Exists(fileName)) continue;
                                            Objects.Script script = new Objects.Script(this.Client, fileName, wpLocation);
                                            scripts.Add(script);
                                        }*/
                                        this.AddWaypoint(new Waypoint(this, wpLocation, contents));
                                    }
                                    else
                                    {
                                        wpScript = p.GetString();
                                        this.AddWaypoint(new Waypoint(this, wpLocation, wpScript));
                                    }
                                    break;
                                default:
                                    this.AddWaypoint(new Waypoint(this, wpLocation, wpType));
                                    break;
                            }
                        }
                        break;
                    default:
                        return false;
                }
            }
            return true;
        }
        /// <summary>
        /// Attempts to get the optimal tile for an AoE rune or spell.
        /// <para></para>
        /// Returns null if no tile is found.
        /// </summary>
        /// <param name="areaEffect">The area effect that is going to be used.</param>
        /// <returns></returns>
        public Map.Tile GetAreaEffectTile(Objects.AreaEffect areaEffect)
        {
            return this.GetAreaEffectTile(areaEffect, this.Client.Map.GetTilesOnScreen());
        }
        /// <summary>
        /// Attempts to get the optimal tile for an AoE rune or spell.
        /// <para></para>
        /// Returns null if no tile is found.
        /// </summary>
        /// <param name="areaEffect">The area effect that is going to be used.</param>
        /// <param name="tilesOnScreen">A collection of the current tiles visible on screen.</param>
        /// <returns></returns>
        public Map.Tile GetAreaEffectTile(Objects.AreaEffect areaEffect, Map.TileCollection tilesOnScreen)
        {
            return this.GetAreaEffectTile(areaEffect, tilesOnScreen, null);
        }
        /// <summary>
        /// Attempts to get the optimal tile for an AoE rune or spell.
        /// <para></para>
        /// Returns null if no tile is found.
        /// </summary>
        /// <param name="areaEffect">The area effect that is going to be used.</param>
        /// <param name="tilesOnScreen">A collection of the current tiles visible on screen.</param>
        /// <param name="target">The target that must be hit. Can be null if no target is present.</param>
        /// <returns></returns>
        public Map.Tile GetAreaEffectTile(Objects.AreaEffect areaEffect, Map.TileCollection tilesOnScreen,
            Objects.Creature target)
        {
            List<Map.Tile> bestTiles = new List<Map.Tile>();
            int bestCount = -1;
            uint playerID = this.Client.Player.ID;
            Map.Tile playerTile = tilesOnScreen.GetTile(count: this.Client.Player.ID);
            if (playerTile == null) return null;
            Objects.Location playerLoc = playerTile.WorldLocation;
            uint targetID = target == null ? 0 : target.ID;

            // store the locations of creatures
            List<Map.Tile> creaturesOnScreen = new List<Map.Tile>();
            foreach (Map.Tile tile in tilesOnScreen.GetTiles())
            {
                if (tile.ContainsCreature() && !tile.ContainsCreature(playerID)) creaturesOnScreen.Add(tile);
            }

            foreach (Map.Tile tile in creaturesOnScreen)
            {
                if (playerLoc.CanShootLocation(this.Client, tile.WorldLocation, tilesOnScreen))
                {
                    ushort count = 0;
                    bool foundTarget = target == null ? true : false;
                    foreach (Map.Tile creatureTile in creaturesOnScreen)
                    {
                        if (creatureTile.WorldLocation.IsInAreaEffect(areaEffect, tile.WorldLocation,
                            (Enums.Direction)this.Client.Player.Direction))
                        {
                            count++;

                            if (!foundTarget && creatureTile.ContainsCreature(targetID)) foundTarget = true;
                        }
                    }
                    if (count == 0 || !foundTarget) continue;

                    if (count == bestCount) bestTiles.Add(tile);
                    else if (count > bestCount)
                    {
                        bestTiles.Clear();
                        bestTiles.Add(tile);
                        bestCount = count;
                    }
                }
            }
            if (bestTiles.Count == 0) return null;
            return bestTiles[new Random().Next(bestTiles.Count)];
        }
        /// <summary>
        /// Attempts to get the best target. Returns null if unsuccessful.
        /// </summary>
        /// <param name="tilesOnScreen">A tile collection used for pathfinding.</param>
        /// <param name="creatures">A collection of creatures visible on screen.</param>
        /// <param name="players">A collection of players visible on screen.</param>
        /// <param name="setCreature">Set to true if Target.Creature should be set.</param>
        /// <returns></returns>
        public Target GetBestTarget(Map.TileCollection tilesOnScreen, IEnumerable<Objects.Creature> creatures,
            IEnumerable<Objects.Creature> players, bool setCreature)
        {
            Target bestTarget = null;
            int bestDanger = -1;
            int bestDistance = 100;
            Objects.Location playerLoc = this.Client.Player.Location;
            foreach (Target t in this.GetTargets())
            {
                Target.Setting s = t.GetBestSetting(creatures, players,
                    tilesOnScreen, setCreature);
                if (s == null) continue;
                if (t.Creature == null || !t.Creature.IsVisible) continue;

                var pfNodes = t.Creature.GetTilesToCreature(tilesOnScreen, this.PathFinder).ToArray();
                int distance = pfNodes.Length != 0 ?
                    pfNodes.Length :
                    (int)playerLoc.DistanceTo(t.Creature.Location);
                if ((this.CurrentSettings.PrioritizeDanger && bestDanger < s.DangerLevel) ||
                    (!this.CurrentSettings.PrioritizeDanger && distance < bestDistance))
                {
                    bestDistance = distance;
                    bestDanger = s.DangerLevel;
                    bestTarget = t;
                }
            }
            return bestTarget;
        }
        #endregion

        #region private methods
        private void Run()
        {
            #region local variables
            Walker walker = new Walker(this);
            Targeting targeting = new Targeting(this);
            Looter looter = new Looter(this);
            List<Objects.Location> corpseLocations = new List<Objects.Location>();
            Objects.Location currentCorpseLocation = Objects.Location.Invalid;
            Waypoint currentWaypoint = null;
            bool cleanupCalled = false;
            Map.TileCollection tileCollection = null;

            #region events
            // create anonymous methods and reference them
            // because we want to unsub them later
            TargetHandler anonTargetKilled = delegate(Target t)
            {
                if (t == null || t.Creature == null) return;
                if (!t.DoLoot) return;
                if (!this.Client.Player.Location.IsOnScreen(t.Creature.Location)) return;
                if (!corpseLocations.Contains(t.Creature.Location)) corpseLocations.Add(t.Creature.Location);
            };
            Targeting.CreatureDiedHandler anonCreatureDied = delegate(Objects.Creature c)
            {
                string name = c.Name.ToLower();
                foreach (Target t in this.GetTargets())
                {
                    if (t.Name.ToLower() == name)
                    {
                        if (!t.DoLoot) break;
                        if (!corpseLocations.Contains(c.Location)) corpseLocations.Add(c.Location);
                        break;
                    }
                }
            };
            Walker.WaypointHandler anonWaypointExecutedEnd = delegate(Waypoint waypoint, bool success)
            {
                if (this.Waypoints.Count == 0) return;
                this.CurrentWaypointIndex++;
            };
            Looter.CorpseOpenedHandler anonCorpseOpened = delegate(Objects.Container container)
            {
                looter.LootItems(container, this.GetLoot(), true);
            };
            Looter.LootingFinishedHandler anonLootingFinished = delegate(Objects.Container container)
            {
                if (this.Waypoints.Count == 0) return;

                if (container.IsOpen) container.Close();
                corpseLocations.Remove(currentCorpseLocation);
                currentCorpseLocation = Objects.Location.Invalid;
            };
            Targeting.ErrorHandler anonTargetingError = delegate(Exception ex)
            {
                DateTime dt = DateTime.Now;
                string dtString = dt.Year + "-" + dt.Month + "-" + dt.Day + " " + dt.Hour + ":" + dt.Minute + ":" + dt.Second;
                File.AppendAllText("cavebot-debug.txt", "[" + dtString + "] Targeting:\n" + ex.Message + "\n" + ex.StackTrace + "\n");
            };
            Walker.ErrorHandler anonWalkerError = delegate(Exception ex)
            {
                DateTime dt = DateTime.Now;
                string dtString = dt.Year + "-" + dt.Month + "-" + dt.Day + " " + dt.Hour + ":" + dt.Minute + ":" + dt.Second;
                File.AppendAllText("cavebot-debug.txt", "[" + dtString + "] Waypoints:\n" + ex.Message + "\n" + ex.StackTrace + "\n");
            };
            Looter.ErrorOccurredHandler anonLooterError = delegate(Exception ex)
            {
                DateTime dt = DateTime.Now;
                string dtString = dt.Year + "-" + dt.Month + "-" + dt.Day + " " + dt.Hour + ":" + dt.Minute + ":" + dt.Second;
                File.AppendAllText("cavebot-debug.txt", "[" + dtString + "] Looter:\n" + ex.Message + "\n" + ex.StackTrace + "\n");
            };
            CleanupHandler anonCleanup = delegate()
            {
                this.TargetKilled -= anonTargetKilled;
                walker.WaypointExecutedEnd -= anonWaypointExecutedEnd;
                looter.CorpseOpened -= anonCorpseOpened;
                looter.LootingFinished -= anonLootingFinished;
                targeting.CreatureDied -= anonCreatureDied;
                targeting.ErrorOccurred -= anonTargetingError;
                walker.ErrorOccurred -= anonWalkerError;
                looter.ErrorOccurred -= anonLooterError;
                cleanupCalled = true;
            };
            // subscribe to events
            this.TargetKilled += anonTargetKilled;
            walker.WaypointExecutedEnd += anonWaypointExecutedEnd;
            looter.CorpseOpened += anonCorpseOpened;
            looter.LootingFinished += anonLootingFinished;
            targeting.CreatureDied += anonCreatureDied;
            targeting.ErrorOccurred += anonTargetingError;
            walker.ErrorOccurred += anonWalkerError;
            looter.ErrorOccurred += anonLooterError;
            // subscribe to cleanup, as we need to unsub before exiting
            this.CleanupCalled += anonCleanup;
            #endregion
            #endregion

            while (this.IsRunning)
            {
                try
                {
                    Thread.Sleep(500);

                    // check if IsRunning changed or cleanup called while thread was sleeping
                    if (!this.IsRunning || cleanupCalled) break;

                    if (!this.Client.Player.Connected) continue;

                    // check if a script waypoint is currently running
                    if (walker.IsRunning &&
                        currentWaypoint != null &&
                        currentWaypoint.Type == Waypoint.Types.Script)
                    {
                        continue;
                    }

                    Objects.Location playerLoc = this.Client.Player.Location;
                    IEnumerable<Objects.Creature> creatures = this.Client.BattleList.GetCreatures(false, true),
                        visiblePlayers = this.Client.BattleList.GetPlayers(true, true);
                    List<Objects.Creature> visibleCreatures = new List<Objects.Creature>();
                    foreach (Objects.Creature c in creatures)
                    {
                        if (c.IsVisible) visibleCreatures.Add(c);
                    }

                    foreach (Objects.Creature c in targeting.GetKilledCreatures(creatures))
                    {
                        string name = c.Name.ToLower();
                        foreach (Target t in this.GetTargets())
                        {
                            if (t.Name.ToLower() != name) continue;
                            if (!corpseLocations.Contains(c.Location)) corpseLocations.Add(c.Location);
                            break;
                        }
                    }

                    #region targeting
                    if (targeting.IsRunning)
                    {
                        targeting.UpdateCache(tileCollection, visibleCreatures, visiblePlayers);
                        continue;
                    }
                    else if (this.CurrentSettings.KillBeforeLooting || corpseLocations.Count == 0)
                    {
                        Target t = this.GetBestTarget(tileCollection, creatures, this.Client.BattleList.GetPlayers(true, true), true);
                        if (t != null)
                        {
                            if (this.CurrentSettings.KillBeforeLooting && looter.IsRunning) looter.CancelExecution();
                            if (this.Client.TibiaVersion < 810) this.StopwatchExhaust.Restart(); // restart to avoid instant attack->spell/rune
                            targeting.ExecuteTarget(t, tileCollection, visibleCreatures, visiblePlayers);
                            continue;
                        }
                    }
                    #endregion

                    #region looting
                    if (looter.IsRunning)
                    {
                        looter.UpdateCache(tileCollection);
                        continue;
                    }
                    // loot always if there are no waypoints
                    if (this.Waypoints.Count == 0)
                    {
                        List<ushort> ids = new List<ushort>();
                        foreach (Loot loot in this.GetLoot())
                        {
                            ids.Add(loot.ID);
                        }
                        foreach (Objects.Container c in this.Client.Inventory.GetContainers(1))
                        {
                            if (c.Name.Contains("Backpack")) continue;
                            if (c.GetItems(ids).ToArray().Length == 0) continue;

                            looter.LootItems(c, this.GetLoot(), false);
                        }
                    }
                    // check loot locations
                    else if (corpseLocations.Count > 0)
                    {
                        if (this.Client.Player.IsWalking) continue;

                        // find new corpse location
                        if (!currentCorpseLocation.IsValid() || !playerLoc.IsInRange(currentCorpseLocation))
                        {
                            int distance = int.MaxValue;
                            Objects.Location bestLoc = Objects.Location.Invalid;
                            foreach (Objects.Location loc in corpseLocations)
                            {
                                if (!playerLoc.IsInRange(loc)) continue;

                                int corpseDistance = 0;
                                if (playerLoc.IsOnScreen(loc))
                                {
                                    Map.Tile tile = tileCollection.GetTile(loc);
                                    if (tile == null) continue;
                                    Map.TileObject topItem = tile.GetTopUseItem(false);
                                    if (!topItem.HasFlag(Enums.ObjectPropertiesFlags.IsContainer)) continue;

                                    var tilesToLoc = playerLoc.GetTilesToLocation(this.Client, loc,
                                        tileCollection, this.PathFinder, true, true).ToArray();
                                    if (tilesToLoc.Length == 0) continue;

                                    corpseDistance = tilesToLoc.Length - 1;
                                }
                                else corpseDistance = (int)playerLoc.DistanceTo(loc) + 20;

                                if (corpseDistance < distance)
                                {
                                    distance = corpseDistance;
                                    bestLoc = loc;
                                }
                            }
                            if (bestLoc.IsValid()) currentCorpseLocation = bestLoc;
                        }

                        // move to loot location
                        if (currentCorpseLocation.IsValid() && playerLoc.IsInRange(currentCorpseLocation))
                        {
                            if (this.Client.Window.StatusBar.GetText() == Enums.StatusBar.ThereIsNoWay)
                            {
                                corpseLocations.Remove(currentCorpseLocation);
                                currentCorpseLocation = Objects.Location.Invalid;
                                this.Client.Window.StatusBar.SetText(string.Empty);
                                continue;
                            }

                            if (playerLoc.IsOnScreen(currentCorpseLocation))
                            {
                                Map.Tile tile = tileCollection.GetTile(currentCorpseLocation);
                                if (tile == null)
                                {
                                    corpseLocations.Remove(currentCorpseLocation);
                                    currentCorpseLocation = Objects.Location.Invalid;
                                    continue;
                                }
                                Map.TileObject topItem = tile.GetTopUseItem(false);
                                if (!topItem.HasFlag(Enums.ObjectPropertiesFlags.IsContainer))
                                {
                                    corpseLocations.Remove(currentCorpseLocation);
                                    currentCorpseLocation = Objects.Location.Invalid;
                                    continue;
                                }

                                if (playerLoc.DistanceTo(currentCorpseLocation) >= 2)
                                {
                                    Map.TileCollection adjTiles = tileCollection.GetAdjacentTileCollection(tile);
                                    int adjDistance = playerLoc.GetTilesToLocation(this.Client,
                                        tile.WorldLocation, tileCollection, this.PathFinder, true).ToArray().Length;
                                    Objects.Location adjLoc = Objects.Location.Invalid;
                                    foreach (Map.Tile adjTile in adjTiles.GetTiles())
                                    {
                                        if (!adjTile.IsWalkable()) continue;

                                        var tilesToCorpse = playerLoc.GetTilesToLocation(this.Client,
                                            adjTile.WorldLocation, tileCollection, this.PathFinder, true).ToArray();
                                        if (tilesToCorpse.Length == 0) continue;

                                        int dist = tilesToCorpse.Length - 1;
                                        if (dist < adjDistance)
                                        {
                                            adjDistance = dist;
                                            adjLoc = adjTile.WorldLocation;
                                        }
                                    }

                                    if (adjLoc.IsValid())
                                    {
                                        this.Client.Player.GoTo = adjLoc;
                                        continue;
                                    }

                                    corpseLocations.Remove(currentCorpseLocation);
                                    currentCorpseLocation = Objects.Location.Invalid;
                                    continue;
                                }

                                // we're in range to open the corpse, so let's open it
                                looter.OpenCorpse(tileCollection, tile);
                                continue;
                            }
                            else
                            {
                                this.Client.Player.GoTo = currentCorpseLocation;
                                continue;
                            }
                        }
                        else currentCorpseLocation = Objects.Location.Invalid;
                    }
                    #endregion

                    #region waypoints
                    if (walker.IsRunning)
                    {
                        walker.UpdateCache(tileCollection);
                        continue;
                    }
                    else if (this.Waypoints.Count > 0)
                    {
                        // check if this is the first time we're running the walker
                        // and if waypoint index wasn't set by user
                        // if so, find the closest waypoint
                        if (currentWaypoint == null && this.CurrentWaypointIndex == 0)
                        {
                            int distance = 200;
                            Waypoint closestWaypoint = null;
                            foreach (Waypoint wp in this.GetWaypoints())
                            {
                                if (!playerLoc.IsInRange(wp.Location)) continue;
                                int wpDist = (int)playerLoc.DistanceTo(wp.Location);
                                if (distance > wpDist)
                                {
                                    distance = wpDist;
                                    closestWaypoint = wp;
                                }
                            }
                            if (closestWaypoint == null) continue;
                            currentWaypoint = closestWaypoint;
                        }
                        else currentWaypoint = this.Waypoints[this.CurrentWaypointIndex];

                        walker.ExecuteWaypoint(currentWaypoint, tileCollection);
                    }
                    #endregion
                }
                catch (Exception ex)
                {

                }
            }

            this.CleanupCalled -= anonCleanup;
        }
        private void CavebotLogic()
        {
            if (this.Client == null) return;
            Target currentTarget = new Target(this);
            Objects.Location currentSubNode = Objects.Location.Invalid,
                currentCorpseLocation = Objects.Location.Invalid;
            Random rand = new Random();
            Map.TileCollection tileCollection = new Map.TileCollection(this.Client, new List<Map.Tile>());
            Objects.MiniMap.MergedChunk mergedChunk = null;
            List<Objects.Location> corpseLocations = new List<Objects.Location>();
            List<Objects.Creature> creaturesToLoot = new List<Objects.Creature>();
            // key = creature ID, value = time when looted (Environment.TickCount)
            Dictionary<uint, long> creaturesAlreadyLooted = new Dictionary<uint, long>();
            bool firstWaypoint = true,
                doSleep = true; // set this to true if the next round should sleep
            this.IsRunning = true;
            Target.Setting bestSetting = null;
            ushort oldTileID = 0;
            Objects.Location oldTileLoc = Objects.Location.Invalid;
            Stack<Objects.Location> backtrackedLocations = new Stack<Objects.Location>();

            while (this.IsRunning)
            {
                try
                {
                    if (doSleep) Thread.Sleep(500);
                    else doSleep = true;

                    if (this.CurrentSettings.UseGoldStacks)
                    {
                        Objects.Item goldStack = this.Client.Inventory.GetItem(this.Client.ItemList.Valuables.GoldCoin, 100);
                        if (goldStack != null) goldStack.Use();
                    }

                    #region Always loot
                    if (this.Targets.Count == 0 && this.Loots.Count > 0)
                    {
                        foreach (Objects.Container container in this.Client.Inventory.GetContainers())
                        {
                            if (container.IsOpen && !container.Name.EndsWith("Backpack") && !container.Name.EndsWith("Bag"))
                            {
                                this.LootItems(container);
                            }
                        }
                    }
                    #endregion

                    // get tiles on screen
                    tileCollection = this.Client.Map.GetTilesOnScreen();
                    // get minimap data
                    Objects.Location playerLoc = this.Client.Player.Location;
                    //mergedChunk = this.Client.MiniMap.GetMergedChunk(playerLoc.Offset(-this.Client.Addresses.Map.CenterX,
                    //    -this.Client.Addresses.Map.CenterY),
                    //    playerLoc.Offset(this.Client.Addresses.Map.CenterX, this.Client.Addresses.Map.CenterY));

                    #region Targeting and looting
                    // check if current target is null,
                    // if so, assign it to a new value
                    // necessary to avoid exceptions
                    if (currentTarget == null) currentTarget = new Target(this);

                    if (backtrackedLocations.Count > 0)
                    {
                        var peekedLoc = backtrackedLocations.Peek();
                        if (peekedLoc.Z != playerLoc.Z) backtrackedLocations.Clear();
                        else if (peekedLoc.DistanceTo(playerLoc) >= 3) backtrackedLocations.Push(playerLoc);
                    }
                    else backtrackedLocations.Push(playerLoc);

                    /*var ptile = tileCollection.GetTile(count: this.Client.Player.ID);
                    //if (ptile != null && ptile.Items.Count > 1) this.Client.Window.SetTitleText(this.Client.Memory.ReadUInt32(ptile.Items[1].Address).ToString());
                    //if (ptile != null) this.Client.Window.SetTitleText(ptile.MemoryLocation.ToString() + " - " + ptile.TileNumber + " - " + ptile.WorldLocation);
                    //else this.Client.Window.SetTitleText("null");

                    foreach (Map.Tile t in tileCollection.GetTiles())
                    {
                        if (t.IsWalkable(this.Client))
                        {
                            this.Client.Memory.WriteUInt16(t.Ground.Address + this.Client.Addresses.MapDistances.ObjectData, 231);
                        }
                        else this.Client.Memory.WriteUInt16(t.Ground.Address + this.Client.Addresses.MapDistances.ObjectData, 836);
                    }*/

                    if (currentTarget.Creature != null && bestSetting != null)
                    {
                        // add dead creatures to corpse list
                        if (this.CurrentSettings.KillBeforeLooting)
                        {
                            // clear old entries
                            long currentTime = Environment.TickCount;
                            List<uint> keysToRemove = new List<uint>();
                            foreach (var keypair in creaturesAlreadyLooted)
                            {
                                if (currentTime > keypair.Value + 1000 * 60 * 5)
                                {
                                    keysToRemove.Add(keypair.Key);
                                }
                            }
                            foreach (var key in keysToRemove) creaturesAlreadyLooted.Remove(key);

                            foreach (Objects.Creature c in this.Client.BattleList.GetCreatures(true, true))
                            {
                                if (creaturesAlreadyLooted.ContainsKey(c.ID)) continue;
                                string name = c.Name.ToLower();
                                foreach (Target t in this.GetTargets())
                                {
                                    if (t.Name.ToLower() == name)
                                    {
                                        uint id = c.ID;
                                        bool found = false;
                                        foreach (Objects.Creature creature in creaturesToLoot)
                                        {
                                            if (creature.ID == id)
                                            {
                                                found = true;
                                                break;
                                            }
                                        }
                                        if (!found) creaturesToLoot.Add(c);
                                    }
                                }
                            }
                            playerLoc = this.Client.Player.Location;
                            foreach (Objects.Creature c in creaturesToLoot)
                            {
                                if (c.IsDead && playerLoc.IsOnScreen(c.Location))
                                {
                                    Objects.Location loc = c.Location;
                                    Map.Tile tile = tileCollection.GetTile(loc);
                                    if (tile == null) break;
                                    Map.TileObject topItem = tile.GetTopUseItem(false);
                                    if (!this.Client.GetObjectProperty(topItem.ID).HasFlag(Enums.ObjectPropertiesFlags.IsContainer)) break;
                                    if (!corpseLocations.Contains(loc)) corpseLocations.Add(loc);
                                    break;
                                }
                            }
                        }

                        bool doLoot = false; // set to true if the cavebot should loot open containers
                        #region Creature checks and stance stuff
                        // initiate "loop" to allow better codeflow
                        while (true)
                        {
                            // check if creature's HP% is 0 and check for new targets if target is unlootable
                            if (!currentTarget.DoLoot && currentTarget.Creature.HealthPercent == 0)
                            {
                                if (this.TargetKilled != null) this.TargetKilled(currentTarget);
                                currentTarget.Creature = null;
                                break;
                            }

                            // check if creature is dead, and loot it if so
                            if (currentTarget.Creature.IsDead)
                            {
                                if (this.TargetKilled != null) this.TargetKilled(currentTarget);
                                doLoot = currentTarget.DoLoot;
                                if (doLoot) Thread.Sleep(rand.Next(100, 250));
                                break;
                            }
                            else if (currentTarget.Creature.HealthPercent == 0)
                            {
                                Thread.Sleep(50);
                                continue;
                            }
                            else if (!currentTarget.Creature.IsVisible)
                            {
                                currentTarget.Creature = null;
                                break;
                            }

                            // check if creature is still on screen
                            if (!this.Client.Player.Location.IsOnScreen(currentTarget.Creature.Location) ||
                                (!currentTarget.Creature.IsVisible && currentTarget.Creature.HealthPercent > 0))
                            {
                                if (this.Client.Player.Target != 0) this.Client.Packets.Attack(0);
                                currentTarget.Creature = null;
                                break;
                            }

                            // check if the cavebot should stop attacking the creature due to out of range
                            if (this.CurrentSettings.StopAttackingWhenOutOfRange &&
                                currentTarget.Creature.Location.DistanceTo(this.Client.Player.Location) > bestSetting.Range + 2)
                            {
                                if (this.Client.Player.Target != 0) this.Client.Packets.Attack(0);
                                currentTarget.Creature = null;
                                break;
                            }

                            // check if the cavebot should stop attacking if the creature is not reachable and/or shootable
                            if ((bestSetting.MustBeReachable && !currentTarget.Creature.IsReachable(tileCollection, this.PathFinder)) ||
                                (bestSetting.MustBeShootable && !currentTarget.Creature.IsShootable(tileCollection)))
                            {
                                if (this.Client.Player.Target != 0) this.Client.Packets.Attack(0);
                                currentTarget.Creature = null;
                                break;
                            }

                            // check if there is a better creature to attack
                            if (!this.CurrentSettings.StickToCreature)
                            {
                                Target newTarget = this.GetBestTarget(tileCollection, this.Client.BattleList.GetCreatures(true, true),
                                    this.Client.BattleList.GetPlayers(true, true), true);
                                if (newTarget != null && currentTarget != newTarget)
                                {
                                    currentTarget = newTarget;
                                    doSleep = false;
                                    break;
                                }
                            }

                            // check if player is attacking wrong creature
                            if (currentTarget.Creature.ID != this.Client.Player.Target)
                            {
                                currentTarget.Creature.Attack();
                                Thread.Sleep(100);
                            }

                            // move according to stance, also change fight mode/stance if necessary
                            if (this.Client.Player.FightMode != bestSetting.FightMode) this.Client.Player.FightMode = bestSetting.FightMode;
                            Map.Tile playerTile = tileCollection.GetTile(count: this.Client.Player.ID);
                            playerLoc = playerTile.WorldLocation;
                            Map.Tile creatureTile = tileCollection.GetTile(count: currentTarget.Creature.ID);
                            if (playerTile != null && creatureTile != null)
                            {
                                switch (bestSetting.FightStance)
                                {
                                    case Enums.FightStance.Follow:
                                        if (this.Client.Player.FightStance != Enums.FightStance.Follow) this.Client.Player.FightStance = Enums.FightStance.Follow;
                                        break;
                                    case Enums.FightStance.FollowDiagonalOnly:
                                    case Enums.FightStance.DistanceFollow:
                                    case Enums.FightStance.DistanceWait:
                                    case Enums.FightStance.FollowStrike:
                                        if (this.Client.Player.FightStance != Enums.FightStance.Stand) this.Client.Player.FightStance = Enums.FightStance.Stand;
                                        Objects.Location targetLoc = currentTarget.GetBestLocation(bestSetting, tileCollection,
                                            this.Client.BattleList.GetCreatures(true, true), backtrackedLocations);
                                        if (!targetLoc.IsValid()) break;

                                        if (backtrackedLocations.Count > 0)
                                        {
                                            var peekedLoc = backtrackedLocations.Peek();
                                            if (peekedLoc == targetLoc && peekedLoc.DistanceTo(playerLoc) < 2) backtrackedLocations.Pop();
                                        }
                                        if (this.Client.Player.GoTo != targetLoc && playerLoc != targetLoc)
                                        {
                                            this.Client.Player.GoTo = targetLoc;
                                            /*Map.Tile t = tileCollection.GetTile(targetLoc);
                                            if (t == null) break;
                                            if (t.WorldLocation != oldTileLoc)
                                            {
                                                if (oldTileLoc.IsValid())
                                                {
                                                    Map.Tile oldTile = tileCollection.GetTile(oldTileLoc);
                                                    this.Client.Memory.WriteUInt16(oldTile.Ground.Address + this.Client.Addresses.MapDistances.ObjectData, oldTileID);
                                                }

                                                oldTileLoc = t.WorldLocation;
                                                oldTileID = t.Ground.Data;
                                                this.Client.Memory.WriteUInt16(t.Ground.Address + this.Client.Addresses.MapDistances.ObjectData, 406);
                                            }*/
                                        }
                                        break;
                                    case Enums.FightStance.Stand:
                                        if (this.Client.Player.FightStance != Enums.FightStance.Stand) this.Client.Player.FightStance = Enums.FightStance.Stand;
                                        break;
                                }

                                // Shoot rune or spell
                                if (this.Client.Player.HealthPercent >= this.CurrentSettings.MinimumHealthToShoot &&
                                    currentTarget.Creature.HealthPercent > 0)
                                {
                                    ushort runeID = 0;
                                    if ((runeID = bestSetting.GetRuneID()) != 0)
                                    {
                                        Objects.Item rune = this.Client.Inventory.GetItem(runeID);
                                        if (rune != null)
                                        {
                                            if (!this.StopwatchExhaust.IsRunning || this.StopwatchExhaust.ElapsedMilliseconds >= this.CurrentSettings.Exhaust)
                                            {
                                                if (bestSetting.RuneIsAoE())
                                                {
                                                    Map.Tile bestTile = this.GetAreaEffectTile(bestSetting.GetAreaEffect(), tileCollection, currentTarget.Creature);
                                                    if (bestTile != null)
                                                    {
                                                        rune.UseOnLocation(bestTile.WorldLocation);
                                                        this.StopwatchExhaust.Reset();
                                                        this.StopwatchExhaust.Start();
                                                    }
                                                }
                                                else if (currentTarget.Creature.IsShootable(tileCollection))
                                                {
                                                    //rune.UseOnCreature(currentTarget.Creature);
                                                    rune.UseOnBattleList(currentTarget.Creature);
                                                    this.StopwatchExhaust.Reset();
                                                    this.StopwatchExhaust.Start();
                                                }
                                            }
                                        }
                                    }
                                    else if (bestSetting.Spell != string.Empty)
                                    {
                                        if (this.Client.Player.Location.IsAdjacentNonDiagonalOnly(currentTarget.Creature.Location))
                                        {
                                            Enums.Direction direction = Enums.Direction.Down;
                                            int diffX = playerTile.WorldLocation.X - creatureTile.WorldLocation.X,
                                                diffY = playerTile.WorldLocation.Y - creatureTile.WorldLocation.Y;
                                            if (diffX > 0) direction = Enums.Direction.Left;
                                            else if (diffX < 0) direction = Enums.Direction.Right;
                                            else if (diffY > 0) direction = Enums.Direction.Up;
                                            else if (diffY < 0) direction = Enums.Direction.Down;
                                            if ((Enums.Direction)this.Client.Player.Direction != direction)
                                            {
                                                this.Client.Packets.Turn(direction);
                                                Thread.Sleep(300);
                                            }
                                            if ((Enums.Direction)this.Client.Player.Direction == direction &&
                                                (!this.StopwatchExhaust.IsRunning || this.StopwatchExhaust.ElapsedMilliseconds > this.CurrentSettings.Exhaust))
                                            {
                                                this.Client.Packets.Say(bestSetting.Spell);
                                                this.StopwatchExhaust.Reset();
                                                this.StopwatchExhaust.Start();
                                            }
                                        }
                                    }
                                }
                            }
                            // break loop
                            break;
                        }
                        #endregion

                        #region Loot creature
                        if (doLoot && currentTarget.Creature != null)
                        {
                            Objects.Location loc = currentTarget.Creature.Location;
                            currentTarget.Creature = null;

                            if (this.CurrentSettings.KillBeforeLooting) corpseLocations.Add(loc);
                            else
                            {
                                Objects.Container container = this.OpenCorpse(loc);
                                if (container != null)
                                {
                                    // loot container
                                    this.LootItems(container);
                                    container.Close();
                                    Thread.Sleep(50);
                                }
                            }
                        }
                        #endregion
                    }

                    #region check for new target
                    if (currentTarget.Creature == null)
                    {
                        Target newTarget = this.GetBestTarget(tileCollection, this.Client.BattleList.GetCreatures(true, true),
                            this.Client.BattleList.GetPlayers(true, true), true);
                        if (newTarget != null)
                        {
                            currentTarget = newTarget;
                            doSleep = false;
                            // reset exhaust stopwatch to prevent instant attack->rune
                            this.StopwatchExhaust.Restart();
                        }
                    }
                    #endregion

                    if (currentTarget.Creature != null)
                    {
                        // get target setting
                        bestSetting = currentTarget.GetBestSetting(this.Client.BattleList.GetCreatures(true, true),
                            this.Client.BattleList.GetPlayers(true, true), tileCollection, false);
                        if (bestSetting == null) currentTarget.Creature = null;
                        continue;
                    }

                    #region multiple corpse looting
                    if (corpseLocations.Count > 0)
                    {
                        playerLoc = this.Client.Player.Location;
                        Objects.Location closestLoc = Objects.Location.Invalid;
                        double closestRange = double.MaxValue;

                        foreach (Objects.Location loc in corpseLocations)
                        {
                            if (loc.Z != playerLoc.Z) continue;
                            double dist = playerLoc.DistanceTo(loc);
                            if (dist < closestRange)
                            {
                                closestRange = dist;
                                closestLoc = loc;
                            }
                        }
                        if (closestLoc != Objects.Location.Invalid &&
                            this.Client.Window.StatusBar.GetText() == Enums.StatusBar.ThereIsNoWay)
                        {
                            // remove multiples of a location
                            foreach (Objects.Location loc in corpseLocations.ToArray())
                            {
                                if (loc == closestLoc) corpseLocations.Remove(loc);
                            }
                            closestLoc = Objects.Location.Invalid;
                        }
                        else if (closestLoc != Objects.Location.Invalid)
                        {
                            if (playerLoc.DistanceTo(closestLoc) < 2)
                            {
                                Objects.Container container = this.OpenCorpse(closestLoc);
                                if (container != null)
                                {
                                    this.LootItems(container);
                                    container.Close();
                                }
                                // remove multiples of a location
                                foreach (Objects.Location loc in corpseLocations.ToArray())
                                {
                                    if (loc == closestLoc) corpseLocations.Remove(loc);
                                }
                            }
                            else if (playerLoc.IsOnScreen(closestLoc) && !this.Client.Player.IsWalking)
                            {
                                Map.Tile tile = tileCollection.GetTile(closestLoc);
                                Map.Tile playerTile = tileCollection.GetTile(count: this.Client.Player.ID);
                                if (tile != null)
                                {
                                    closestLoc = tile.WorldLocation;
                                    closestRange = playerTile.WorldLocation.DistanceTo(tile.WorldLocation);
                                    foreach (Map.Tile t in tileCollection.GetAdjacentTileCollection(tile).GetTiles())
                                    {
                                        if (!t.IsWalkable()) continue;
                                        double dist = playerTile.WorldLocation.DistanceTo(t.WorldLocation);
                                        if (dist < closestRange)
                                        {
                                            closestLoc = tile.WorldLocation;
                                            closestRange = dist;
                                        }
                                    }
                                    this.Client.Player.GoTo = closestLoc;
                                }
                            }
                            else if (!this.Client.Player.IsWalking) this.Client.Player.GoTo = closestLoc;
                        }

                        bool found = false;
                        foreach (Objects.Location loc in corpseLocations)
                        {
                            if (loc.Z == playerLoc.Z)
                            {
                                found = true;
                                break;
                            }
                        }
                        if (found) continue;
                    }
                    #endregion
                    #endregion

                    #region Waypoints
                    if (this.Waypoints.Count == 0) continue;

                    if (firstWaypoint)
                    {
                        if (this.CurrentWaypointIndex == 0)
                        {
                            Waypoint closestWaypoint = null;
                            int index = 0;
                            playerLoc = this.Client.Player.Location;
                            for (int i = 0; i < this.Waypoints.Count; i++)
                            {
                                Waypoint wp = this.Waypoints[i];
                                if (wp.Location.Z != playerLoc.Z) continue;
                                if (closestWaypoint == null ||
                                    wp.Location.DistanceTo(playerLoc) < closestWaypoint.Location.DistanceTo(playerLoc))
                                {
                                    closestWaypoint = wp;
                                    index = i;
                                }
                            }
                            if (closestWaypoint != null) this.CurrentWaypointIndex = index;
                        }
                        firstWaypoint = false;
                    }

                    Waypoint currentWaypoint = this.GetWaypoints().ToArray<Waypoint>()[this.CurrentWaypointIndex];
                    if (currentWaypoint.Location.Z != this.Client.Player.Z)
                    {
                        // scan from index to end for a new waypoint
                        for (int i = this.CurrentWaypointIndex; i < this.Waypoints.Count; i++)
                        {
                            Waypoint wp = this.Waypoints[i];
                            if (wp.Location.Z == this.Client.Player.Z)
                            {
                                this.CurrentWaypointIndex = i;
                                currentWaypoint = wp;
                                break;
                            }
                        }
                        if (currentWaypoint.Location.Z != this.Client.Player.Z)
                        {
                            // scan from 0 to index for a new waypoint
                            for (int i = 0; i < this.CurrentWaypointIndex; i++)
                            {
                                Waypoint wp = this.Waypoints[i];
                                if (wp.Location.Z == this.Client.Player.Z)
                                {
                                    this.CurrentWaypointIndex = i;
                                    currentWaypoint = wp;
                                    break;
                                }
                            }
                        }
                    }
                    if (currentWaypoint.Location.Z != this.Client.Player.Z) continue;

                    if (this.Client.Window.StatusBar.GetText() == Enums.StatusBar.ThereIsNoWay)
                    {
                        this.Client.Window.StatusBar.SetText(string.Empty);
                        this.CurrentWaypointIndex++;
                        continue;
                    }

                    switch (currentWaypoint.Type)
                    {
                        case Modules.Cavebot.Waypoint.Types.Script:
                            int oldIndex = this.CurrentWaypointIndex;
                            currentWaypoint.Script.Run(false);
                            if (oldIndex == this.CurrentWaypointIndex) this.CurrentWaypointIndex++;
                            if (this.Client.Window.StatusBar.GetText() == Enums.StatusBar.ThereIsNoWay)
                            {
                                this.Client.Window.StatusBar.SetText(string.Empty);
                                continue;
                            }
                            break;
                        case Modules.Cavebot.Waypoint.Types.Node:
                            if (currentSubNode.IsValid() &&
                                this.Client.Player.Location.DistanceTo(currentSubNode) <= this.CurrentSettings.NodeSkipRange)
                            {
                                this.CurrentWaypointIndex++;
                                currentSubNode = Objects.Location.Invalid;
                                break;
                            }
                            if (!this.Client.Player.IsWalking)
                            {
                                if (this.CurrentSettings.UseAlternateNodeFinder)
                                {
                                    int tries = this.CurrentSettings.NodeRadius * 2;
                                    bool found = false;
                                    for (int i = 0; i < tries; i++)
                                    {
                                        Objects.Location loc = currentWaypoint.Location.Offset(
                                            rand.Next(-this.CurrentSettings.NodeRadius, this.CurrentSettings.NodeRadius),
                                            rand.Next(-this.CurrentSettings.NodeRadius, this.CurrentSettings.NodeRadius),
                                            0);
                                        if (this.Client.Modules.MapViewer.IsWalkable(loc))
                                        {
                                            currentSubNode = loc;
                                            found = true;
                                            break;
                                        }
                                    }
                                    if (!found) currentSubNode = currentWaypoint.Location;
                                    this.Client.Player.GoTo = currentSubNode;
                                }
                                else if (currentWaypoint.NodeLocations.Count > 0)
                                {
                                    Objects.Location subNode = currentWaypoint.NodeLocations[rand.Next(0, currentWaypoint.NodeLocations.Count)];
                                    this.Client.Player.GoTo = subNode;
                                    currentSubNode = subNode;
                                }
                                else
                                {
                                    this.Client.Player.GoTo = currentWaypoint.Location;
                                    currentSubNode = currentWaypoint.Location;
                                }
                            }
                            else
                            {
                                bool found = currentSubNode.IsValid() && currentWaypoint.Location == currentSubNode;
                                if (currentSubNode.IsValid() && !found && currentWaypoint.NodeLocations.Count > 0)
                                {
                                    foreach (Objects.Location loc in currentWaypoint.NodeLocations)
                                    {
                                        if (currentSubNode == loc) { found = true; break; }
                                    }
                                }
                                if (!found)
                                {
                                    if (currentWaypoint.NodeLocations.Count > 0)
                                    {
                                        Objects.Location subNode = currentWaypoint.NodeLocations[rand.Next(0, currentWaypoint.NodeLocations.Count)];
                                        this.Client.Player.GoTo = subNode;
                                        currentSubNode = subNode;
                                    }
                                    else
                                    {
                                        this.Client.Player.GoTo = currentWaypoint.Location;
                                        currentSubNode = currentWaypoint.Location;
                                    }
                                }
                            }
                            break;
                        case Modules.Cavebot.Waypoint.Types.Walk:
                            if (this.Client.Player.Location == currentWaypoint.Location) this.CurrentWaypointIndex++;
                            else if (!this.Client.Player.IsWalking) this.Client.Player.GoTo = currentWaypoint.Location;
                            break;
                        case Modules.Cavebot.Waypoint.Types.Rope:
                        case Modules.Cavebot.Waypoint.Types.Shovel:
                        case Modules.Cavebot.Waypoint.Types.Ladder:
                        case Modules.Cavebot.Waypoint.Types.Machete:
                        case Modules.Cavebot.Waypoint.Types.Pick:
                            if (this.Client.Player.IsWalking) break;
                            if (!this.Client.Player.Location.IsOnScreen(currentWaypoint.Location))
                            {
                                this.Client.Player.GoTo = currentWaypoint.Location;
                                break;
                            }

                            if ((this.Client.Player.Location.DistanceTo(currentWaypoint.Location) >= 2 || this.Client.Player.Location == currentWaypoint.Location) &&
                                currentWaypoint.Type != Modules.Cavebot.Waypoint.Types.Ladder)
                            {
                                Map.Tile playerTile = tileCollection.GetTile(count: this.Client.Player.ID);
                                if (playerTile == null) break;
                                Map.Tile wpTile = tileCollection.GetTile(currentWaypoint.Location);
                                if (wpTile == null) break;
                                double closest = 30;
                                Objects.Location closestLoc = Objects.Location.Invalid;
                                foreach (Map.Tile t in tileCollection.GetAdjacentTileCollection(wpTile).GetTiles())
                                {
                                    double dist = t.WorldLocation.DistanceTo(playerTile.WorldLocation);
                                    if (dist < closest &&
                                        playerTile.WorldLocation.CanReachLocation(this.Client, t.WorldLocation, tileCollection))
                                    {
                                        closest = dist;
                                        closestLoc = t.WorldLocation;
                                    }
                                }
                                if (closestLoc.IsValid()) this.Client.Player.GoTo = closestLoc;
                            }
                            else if (this.Client.Player.Location.IsAdjacentTo(currentWaypoint.Location) ||
                                currentWaypoint.Type == Modules.Cavebot.Waypoint.Types.Ladder)
                            {
                                Map.Tile playerTile = this.Client.Map.GetPlayerTile();
                                if (playerTile == null) break;
                                Map.Tile t = tileCollection.GetTile(currentWaypoint.Location);
                                if (t == null || t.ObjectsCount == 0) break;

                                switch (currentWaypoint.Type)
                                {
                                    case Modules.Cavebot.Waypoint.Types.Rope:
                                        if (!t.ContainsRopeHole())
                                        {
                                            this.CurrentWaypointIndex++;
                                            break;
                                        }
                                        bool ropeHoleCleared = false;
                                        while (true)
                                        {
                                            Map.Tile ropeTile = this.Client.Map.GetTile(currentWaypoint.Location, null);
                                            if (ropeTile == null) break;
                                            Map.TileObject topItem = ropeTile.GetTopMoveItem();
                                            if (topItem.StackIndex == 0)
                                            {
                                                ropeHoleCleared = true;
                                                break;
                                            }
                                            Objects.ObjectProperties props = this.Client.GetObjectProperty(topItem.ID);
                                            if (props.HasFlag(Enums.ObjectPropertiesFlags.IsImmobile)) break;

                                            List<Map.Tile> adjacentTiles = new List<Map.Tile>(this.Client.Map.GetAdjacentTiles(ropeTile).GetTiles());
                                            adjacentTiles.Shuffle();
                                            Map.Tile appropriateTile = null;
                                            foreach (Map.Tile tile in adjacentTiles)
                                            {
                                                if (tile.IsWalkable())
                                                {
                                                    appropriateTile = tile;
                                                    break;
                                                }
                                            }
                                            if (appropriateTile == null) break;
                                            Map.TileObject toTopItem = appropriateTile.GetTopMoveItem();
                                            topItem.Move(toTopItem.ToItemLocation());
                                            Thread.Sleep(rand.Next(400, 800));
                                        }
                                        if (!ropeHoleCleared) break;
                                        Objects.Item rope = this.Client.Inventory.GetItem(this.Client.ItemList.Tools.Rope);
                                        if (rope == null && !this.CurrentSettings.CanUseMagicRope)
                                        {
                                            this.CurrentWaypointIndex++;
                                            break;
                                        }
                                        if (rope != null)
                                        {
                                            // check if tile contains items blocking rope hole
                                            Map.Tile ropeTile = this.Client.Map.GetTile(currentWaypoint.Location, null);
                                            if (ropeTile == null) break;
                                            Map.TileObject topItem = ropeTile.GetTopUseItem(true);
                                            if (topItem.StackIndex != 0) break;
                                            for (int j = 0; j < 2; j++)
                                            {
                                                rope.UseOnTile(ropeTile);
                                                for (int i = 0; i < 12; i++)
                                                {
                                                    if (this.Client.Player.Z != currentWaypoint.Location.Z) break;
                                                    Thread.Sleep(100);
                                                }
                                                if (this.Client.Player.Z != currentWaypoint.Location.Z) break;
                                            }
                                        }
                                        if (this.CurrentSettings.CanUseMagicRope && currentWaypoint.Location.Z == this.Client.Player.Z &&
                                            this.Client.Player.Mana >= 20)
                                        {
                                            this.Client.Player.GoTo = currentWaypoint.Location;
                                            for (int i = 0; i < 35; i++)
                                            {
                                                if (this.Client.Player.Location == currentWaypoint.Location)
                                                {
                                                    this.Client.Packets.Say("exani tera");
                                                    for (int i2 = 0; i2 < 15; i2++)
                                                    {
                                                        if (this.Client.Player.Z != currentWaypoint.Location.Z) break;
                                                        Thread.Sleep(100);
                                                    }
                                                    break;
                                                }
                                                Thread.Sleep(100);
                                            }
                                        }
                                        break;
                                    case Modules.Cavebot.Waypoint.Types.Ladder:
                                        if (!t.ContainsLadder())
                                        {
                                            this.CurrentWaypointIndex++;
                                            break;
                                        }
                                        Map.TileObject ladderTileObject = t.GetTopUseItem(false);
                                        if (ladderTileObject == null) break;
                                        for (int i = 0; i < 3; i++)
                                        {
                                            ladderTileObject.Use();
                                            for (int j = 0; j < 10; j++)
                                            {
                                                if (this.Client.Player.Z != currentWaypoint.Location.Z) break;
                                                Thread.Sleep(100);
                                            }
                                            if (this.Client.Player.Z != currentWaypoint.Location.Z) break;
                                        }
                                        this.CurrentWaypointIndex++;
                                        break;
                                    case Modules.Cavebot.Waypoint.Types.Shovel:
                                        Objects.Item shovel = this.Client.Inventory.GetItem(this.Client.ItemList.Tools.Shovel);
                                        if (shovel != null) shovel.UseOnLocation(currentWaypoint.Location);
                                        Thread.Sleep(500);
                                        this.CurrentWaypointIndex++;
                                        break;
                                    case Modules.Cavebot.Waypoint.Types.Pick:
                                        Objects.Item pick = this.Client.Inventory.GetItem(this.Client.ItemList.Tools.Pick);
                                        if (pick != null) pick.UseOnLocation(currentWaypoint.Location);
                                        Thread.Sleep(500);
                                        this.CurrentWaypointIndex++;
                                        break;
                                    case Modules.Cavebot.Waypoint.Types.Machete:
                                        Objects.Item machete = this.Client.Inventory.GetItem(this.Client.ItemList.Tools.Machete);
                                        if (machete != null) machete.UseOnLocation(currentWaypoint.Location);
                                        Thread.Sleep(500);
                                        this.CurrentWaypointIndex++;
                                        break;
                                }
                            }
                            break;
                    }
                    #endregion
                }
                catch (ThreadAbortException ex) { }
                catch (Exception ex)
                {
                    DateTime dt = DateTime.Now;
                    if (ex is ThreadAbortException) { break; }
                    else if (this.CurrentSettings.DebugMode)
                    {
                        try
                        {
                            System.IO.File.AppendAllText("errors-cavebot.txt",
                                dt.ToString("[yyyy-MM-dd HH:mm:ss] ") + ex.Message + "\n" + ex.StackTrace + "\n" +
                                "Variables:\nWaypoint: " + this.CurrentWaypointIndex + "/" + this.Waypoints.Count +
                                "\nCurrentSubNode: " + (currentSubNode != null ? currentSubNode.ToString() : "null") +
                                "\nCurrentTarget: " + (currentTarget == null ? "null" : "not null") +
                                "\nCurrentTarget.Creature: " + ((currentTarget != null && currentTarget.Creature == null) ? "null" : "not null") + "\n");
                        }
                        catch
                        {
                            System.IO.File.AppendAllText("errors-cavebot.txt",
                                dt.ToString("[yyyy-MM-dd HH:mm:ss] ") + ex.Message + "\n" + ex.StackTrace + "\n");
                        }
                        this.Restart();
                        return;
                    }
                }
            }
            this.IsRunning = false;
        }
        /// <summary>
        /// Attempts to open a corpse at a given location.
        /// </summary>
        /// <param name="loc">The world location where the corpse is.</param>
        /// <returns>Returns an Objects.Container object if successful, null if unsuccessful.</returns>
        private Objects.Container OpenCorpse(Objects.Location loc)
        {
            // check if the cavebot has any wanted lootable items
            if (this.Loots.Count == 0)
            {
                if (this.CurrentSettings.DebugMode)
                {
                    DateTime dt = DateTime.Now;
                    System.IO.File.AppendAllText("debug-cavebot.txt",
                        dt.ToString("[yyyy-MM-dd HH:mm:ss]") + " Looting: Looting list is empty\n");
                }
                return null;
            }
            // check if loc got fucked up
            if (!this.Client.Player.Location.IsOnScreen(loc))
            {
                if (this.CurrentSettings.DebugMode)
                {
                    DateTime dt = DateTime.Now;
                    System.IO.File.AppendAllText("debug-cavebot.txt",
                        dt.ToString("[yyyy-MM-dd HH:mm:ss]") + " Looting: Location not on screen\n");
                }
                return null;
            }
            Map.Tile tile = this.Client.Map.GetTile(loc, null);
            // check if tile is valid
            if (tile == null || !tile.WorldLocation.IsOnScreen(this.Client.Player.Location) || tile.ObjectsCount == 1)
            {
                if (this.CurrentSettings.DebugMode)
                {
                    DateTime dt = DateTime.Now;
                    System.IO.File.AppendAllText("debug-cavebot.txt",
                        dt.ToString("[yyyy-MM-dd HH:mm:ss]") + " Looting: Invalid tile\ntile == null - " + (tile == null).ToString() +
                        (tile != null ?
                            "\ntile on screen - " + tile.WorldLocation.IsOnScreen(this.Client.Player.Location) +
                            "\ntile objects count: " + tile.ObjectsCount
                            : string.Empty) +
                        "\n");
                }
                return null;
            }
            // check if corpse is on a tile with a ladder
            if (tile.ContainsLadder())
            {
                if (this.CurrentSettings.DebugMode)
                {
                    DateTime dt = DateTime.Now;
                    System.IO.File.AppendAllText("debug-cavebot.txt",
                        dt.ToString("[yyyy-MM-dd HH:mm:ss]") + " Looting: tile contains ladder\n");
                }
                return null;
            }
            // find corpse
            Map.TileObject corpse = tile.GetTopUseItem(false);
            if (!this.Client.GetObjectProperty(corpse.ID).HasFlag(Enums.ObjectPropertiesFlags.IsContainer))
            {
                if (this.CurrentSettings.DebugMode)
                {
                    DateTime dt = DateTime.Now;
                    System.IO.File.AppendAllText("debug-cavebot.txt",
                        dt.ToString("[yyyy-MM-dd HH:mm:ss]") + " Looting: top item is not a container\n");
                }
                return null;
            }
            if (corpse.Parent == null ||
                !corpse.Parent.WorldLocation.IsOnScreen(this.Client.Player.Location))
            {
                if (this.CurrentSettings.DebugMode)
                {
                    DateTime dt = DateTime.Now;
                    System.IO.File.AppendAllText("debug-cavebot.txt",
                        dt.ToString("[yyyy-MM-dd HH:mm:ss]") + " Looting: corpse.Parent == " + (corpse.Parent == null ? "true " : "false ") +
                        (corpse.Parent != null ? ", parent tile location on screen == " +
                        corpse.Parent.WorldLocation.IsOnScreen(this.Client.Player.Location) :
                        string.Empty) + "\n");
                }
                return null;
            }
            Objects.Container fromContainer = this.Client.Inventory.GetFirstClosedContainer();
            if (fromContainer == null) return null;
            bool opened = false, invalidItem = false;
            for (int i = 0; i < 3; i++)
            {
                corpse.Use();

                for (int j = 0; j < 15; j++)
                {
                    if (this.Client.Window.StatusBar.GetText() == Enums.StatusBar.YouCanNotUseThisObject) { invalidItem = true; break; }
                    if (fromContainer.IsOpen) { opened = true; break; }
                    Thread.Sleep(100);
                }
                if (opened || invalidItem) break;
            }

            if (!fromContainer.IsOpen)
            {
                if (this.CurrentSettings.DebugMode)
                {
                    DateTime dt = DateTime.Now;
                    System.IO.File.AppendAllText("debug-cavebot.txt",
                        dt.ToString("[yyyy-MM-dd HH:mm:ss]") + " Looting: Corpse never opened, opened == " +
                        opened + " invalidItem == " + invalidItem + "\n");
                }
                return null;
            }
            return fromContainer;
        }
        /// <summary>
        /// Loots all items from a given container.
        /// </summary>
        /// <param name="fromContainer">The Objects.Container object to loot.</param>
        private void LootItems(Objects.Container fromContainer)
        {
            if (fromContainer == null || !fromContainer.IsOpen || fromContainer.ItemsAmount == 0) return;
            if (!this.StopwatchFoodEater.IsRunning) this.StopwatchFoodEater.Start();
            int index = fromContainer.ItemsAmount - 1,
                retryCount = 0;
            Random rand = new Random();
            #region item handling for this container
            while (index >= 0)
            {
                if (!fromContainer.IsOpen || fromContainer.ItemsAmount == 0) return;
                if (retryCount >= 3)
                {
                    retryCount = 0;
                    index--;
                    continue;
                }

                Objects.Item item = fromContainer.GetItemInSlot((byte)index);
                if (item == null)
                {
                    index--;
                    continue;
                }
                // check if it's food, and eat if it is
                if (!this.StopwatchFoodEater.IsRunning) this.StopwatchFoodEater.Start();
                if (this.CurrentSettings.EatFood &&
                    this.Client.ItemList.Food.All.Contains(item.ID) &&
                    this.StopwatchFoodEater.ElapsedMilliseconds >= 16000)
                {
                    this.StopwatchFoodEater.Reset();
                    this.StopwatchFoodEater.Start();
                    ushort amount = item.Count;
                    item.Use();
                    if (amount <= 1)
                    {
                        Thread.Sleep(rand.Next(175, 265));
                        index--;
                        continue;
                    }
                    else
                    {
                        for (int i = 0; i < Math.Min(amount, (ushort)3); i++)
                        {
                            item = fromContainer.GetItemInSlot((byte)index);
                            if (item == null) break;
                            item.Use();
                            Thread.Sleep(rand.Next(150, 280));
                            if (this.Client.Window.StatusBar.GetText() == Enums.StatusBar.YouAreFull) break;
                        }
                        if (fromContainer.GetItemInSlot((byte)index) == null)
                        {
                            index--;
                            continue;
                        }
                    }
                }
                // check if item is in the loot list
                Loot loot = null;
                foreach (Loot l in this.Loots.ToArray())
                {
                    if (l.ID != item.ID) continue;

                    loot = l;
                    break;
                }
                if (loot == null ||
                    (loot.Destination != Loot.Destinations.Ground && loot.Cap > this.Client.Player.Cap))
                {
                    index--;
                    continue;
                }
                Objects.ItemLocation toItem = null;
                switch (loot.Destination)
                {
                    case Loot.Destinations.Ground:
                        Map.Tile playerTile = this.Client.Map.GetPlayerTile();
                        if (playerTile == null) break;
                        List<Map.Tile> adjacentTiles = new List<Map.Tile>(this.Client.Map.GetAdjacentTiles(playerTile).GetTiles());
                        adjacentTiles.Shuffle();
                        Map.Tile suitableTile = playerTile;
                        foreach (Map.Tile t in adjacentTiles)
                        {
                            if (t.IsWalkable())
                            {
                                suitableTile = t;
                                break;
                            }
                        }
                        Map.TileObject topItem = suitableTile.GetTopMoveItem();
                        item.Move(topItem.ToItemLocation());
                        toItem = new Objects.ItemLocation(new Objects.Item(this.Client));
                        break;
                    case Loot.Destinations.EmptyContainer:
                        toItem = this.Client.Inventory.GetFirstSuitableSlot(item, loot.Index);
                        if (toItem != null) item.Move(toItem);
                        break;
                }
                if (toItem != null)
                {
                    if (!item.WaitForInteraction(800))
                    {
                        retryCount++;
                        continue;
                    }
                    else
                    {
                        if (this.ItemLooted != null) this.ItemLooted(item);
                        if (!this.CurrentSettings.FastLooting) Thread.Sleep(rand.Next(350, 700));
                        retryCount = 0;
                    }
                }
                index--;
            }
            #endregion
            index = (byte)(fromContainer.ItemsAmount - 1); retryCount = 0;
            if (!this.CurrentSettings.OpenContainers) return;
            while (index >= 0)
            {
                if (!fromContainer.IsOpen || fromContainer.ItemsAmount == 0) return;
                if (retryCount >= 3) { retryCount = 0; index--; continue; }

                Objects.Item item = fromContainer.GetItemInSlot((byte)index);
                if (item == null)
                {
                    index--;
                    continue;
                }
                if (item.HasFlag(Enums.ObjectPropertiesFlags.IsContainer))
                {
                    item.Use();
                    if (!item.WaitForInteraction(800))
                    {
                        retryCount++;
                        continue;
                    }
                    else
                    {
                        if (!this.CurrentSettings.FastLooting) Thread.Sleep(rand.Next(450, 600));
                        this.LootItems(fromContainer);
                        return;
                    }
                }
                index--;
            }
        }
        #endregion
    }
}
