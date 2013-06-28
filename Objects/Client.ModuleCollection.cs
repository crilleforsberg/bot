using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KarelazisBot.Objects
{
    public partial class Client
    {
        /// <summary>
        /// A class that holds instances of a collection of modules, i.e. cavebot, healer, hotkeys etc.
        /// </summary>
        public class ModuleCollection
        {
            /// <summary>
            /// Constructor for this class, simply creates new instances of modules.
            /// </summary>
            /// <param name="c"></param>
            public ModuleCollection(Objects.Client c)
            {
                this.Client = c;
                this.CacheManager = new Modules.CacheManager("cache");
                this.Cavebot = new Modules.Cavebot(c);
                this.CombobotClient = new Modules.CombobotClient(c);
                this.CombobotServer = new Modules.CombobotServer(c, 3000);
                this.Healer = new Modules.Healer(c, 500, 400, 700);
                this.Hotkeys = new Modules.Hotkeys(c);
                this.ScriptManager = new Modules.ScriptManager(c);
                this.ExperienceCounter = new Modules.ExperienceCounter(c);
                this.TextToSpeech = new Modules.TextToSpeech(c);
                this.MapViewer = new Modules.MapViewer(c);
                //this.HUD = new Modules.HUD(c, true);
                this.Website = new Modules.Website(c);
            }

            /// <summary>
            /// Gets the client associated with this object.
            /// </summary>
            public Objects.Client Client { get; private set; }
            public Modules.Cavebot Cavebot { get; private set; }
            public Modules.CombobotClient CombobotClient { get; private set; }
            public Modules.CombobotServer CombobotServer { get; private set; }
            public Modules.Healer Healer { get; private set; }
            public Modules.Hotkeys Hotkeys { get; private set; }
            public Modules.ScriptManager ScriptManager { get; private set; }
            public Modules.ExperienceCounter ExperienceCounter { get; private set; }
            public Modules.TextToSpeech TextToSpeech { get; private set; }
            public Modules.MapViewer MapViewer { get; private set; }
            public Modules.CacheManager CacheManager { get; private set; }
            //public Modules.HUD HUD { get; private set; }
            public Modules.Website Website { get; private set; }
        }
    }
}
