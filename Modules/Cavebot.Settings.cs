using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KarelazisBot.Modules
{
    public partial class Cavebot
    {
        /// <summary>
        /// A collection class that holds some settings.
        /// </summary>
        public class Settings
        {
            #region constructors
            /// <summary>
            /// Constructor for this class.
            /// </summary>
            public Settings()
            {
                this.LoadDefaults();
            }
            #endregion

            #region methods
            /// <summary>
            /// Loads default settings.
            /// </summary>
            public void LoadDefaults()
            {
                this.CanUseMagicRope = true;
                this.EatFood = true;
                this.Exhaust = 2000;
                this.MinimumHealthToShoot = 75;
                this.NodeRadius = 3;
                this.NodeSkipRange = 3;
                this.OpenContainers = true;
                this.PrioritizeDanger = false;
                this.StickToCreature = true;
                this.StopAttackingWhenOutOfRange = true;
                this.UseGoldStacks = false;
                this.DebugMode = true;
                this.FastLooting = false;
                this.UseAlternateNodeFinder = true;
                this.KillBeforeLooting = false;
                this.ConsiderAllMonstersWhenKeepingAway = false;
                this.AllowDiagonalMovement = false;
                this.KeepAwayPerimeter = 3;
            }
            #endregion

            #region properties
            /// <summary>
            /// Whether to consider other players when choosing a target.
            /// </summary>
            public bool FriendlyMode { get; set; }
            /// <summary>
            /// Whether to write debug logs.
            /// </summary>
            public bool DebugMode { get; set; }
            /// <summary>
            /// Whether to use full gold stacks.
            /// </summary>
            public bool UseGoldStacks { get; set; }
            /// <summary>
            /// Whether to eat food from corpses. Food items are defined in Client.ItemList.Food.
            /// </summary>
            public bool EatFood { get; set; }
            /// <summary>
            /// How far away from the original waypoint to look for a subnode.
            /// </summary>
            public byte NodeRadius { get; set; }
            /// <summary>
            /// How far away from the node to skip to the next waypoint.
            /// </summary>
            public byte NodeSkipRange { get; set; }
            /// <summary>
            /// Whether to prioritize danger over range when attempting to find a new target.
            /// </summary>
            public bool PrioritizeDanger { get; set; }
            /// <summary>
            /// Whether to keep attacking the current target, regardless if there are better targets in range.
            /// </summary>
            public bool StickToCreature { get; set; }
            /// <summary>
            /// Whether to stop attacking the target when it's out of range.
            /// </summary>
            public bool StopAttackingWhenOutOfRange { get; set; }
            /// <summary>
            /// Whether to open containers in corpses.
            /// </summary>
            public bool OpenContainers { get; set; }
            /// <summary>
            /// Minimum amount of milliseconds between casting spells or shooting runes.
            /// </summary>
            public ushort Exhaust { get; set; }
            /// <summary>
            /// The minimum amount of health percent to cast spells or shoot runes.
            /// </summary>
            public ushort MinimumHealthToShoot { get; set; }
            /// <summary>
            /// Whether the player can use the magic rope spell or not.
            /// </summary>
            public bool CanUseMagicRope { get; set; }
            /// <summary>
            /// Whether to skip sleeping between looting items or not.
            /// </summary>
            public bool FastLooting { get; set; }
            /// <summary>
            /// Whether to use *.map files to find a subnode.
            /// </summary>
            public bool UseAlternateNodeFinder { get; set; }
            /// <summary>
            /// Whether to kill all creatures within range before attempting to open corpses and loot items.
            /// </summary>
            public bool KillBeforeLooting { get; set; }
            /// <summary>
            /// Consider all creatures within range when trying to keep away from the target.
            /// </summary>
            public bool ConsiderAllMonstersWhenKeepingAway { get; set; }
            /// <summary>
            /// Whether to allow diagonal movement. Only affects target locations that are adjacent to the player.
            /// </summary>
            public bool AllowDiagonalMovement { get; set; }
            /// <summary>
            /// How far away from the player to consider walking when keeping away from the target(s).
            /// </summary>
            public byte KeepAwayPerimeter { get; set; }
            #endregion
        }
    }
}
