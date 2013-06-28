using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KarelazisBot.Modules
{
    public partial class Cavebot
    {
        /// <summary>
        /// A class that represents a waypoint. Also contains the ability to execute scripts.
        /// </summary>
        public class Waypoint
        {
            #region constructors
            /// <summary>
            /// Constructor an empty waypoint.
            /// </summary>
            /// <param name="parent">The Cavebot module that will host this object.</param>
            public Waypoint(Cavebot parent, string label = "")
            {
                this.Parent = parent;
                this.Location = Objects.Location.Invalid;
                this.Label = label;
            }
            /// <summary>
            /// Constructor for a regular waypoint.
            /// </summary>
            /// <param name="parent">The Cavebot module that will host this object.</param>
            /// <param name="loc">A world location.</param>
            /// <param name="type">The waypoint type.</param>
            public Waypoint(Cavebot parent, Objects.Location loc, Types type, string label = "")
            {
                this.Parent = parent;
                this.Location = loc;
                this.Type = type;
                this.Script = new Script(this.Parent, string.Empty, this);
                this.NodeLocations = new List<Objects.Location>();
                this.Label = label;
            }
            /// <summary>
            /// Constructor for a node waypoint.
            /// </summary>
            /// <param name="parent">The Cavebot module that will host this object.</param>
            /// <param name="loc">A world location.</param>
            /// <param name="nodes">A collection of world locations that will serve as subnodes.</param>
            public Waypoint(Cavebot parent, Objects.Location loc, IEnumerable<Objects.Location> nodes, string label = "")
            {
                this.Parent = parent;
                this.Location = loc;
                this.Type = Types.Node;
                this.NodeLocations = nodes != null ?
                    nodes.ToList<Objects.Location>() :
                    new List<Objects.Location>();
                this.Script = new Script(this.Parent, string.Empty, this);
                this.Label = label;
            }
            /// <summary>
            /// Constructor for a scripted waypoint.
            /// </summary>
            /// <param name="parent">The Cavebot module that will host this object.</param>
            /// <param name="loc">A world location.</param>
            /// <param name="code">The C# code that will be loaded as a script.</param>
            public Waypoint(Cavebot parent, Objects.Location loc, string code, string label = "")
            {
                this.Parent = parent;
                this.Location = loc;
                this.Script = new Script(this.Parent, code, this);
                this.Type = Types.Script;
                this.NodeLocations = new List<Objects.Location>();
                this.Label = label;
            }
            #endregion

            #region properties
            /// <summary>
            /// Gets the Cavebot object that hosts this waypoint.
            /// </summary>
            public Cavebot Parent { get; private set; }
            /// <summary>
            /// Gets or sets the label for this waypoint.
            /// </summary>
            public string Label { get; set; }
            /// <summary>
            /// Gets or sets the type for this waypoint.
            /// </summary>
            public Types Type { get; set; }
            /// <summary>
            /// Gets or sets the world location for this waypoint.
            /// </summary>
            public Objects.Location Location { get; set; }
            /// <summary>
            /// Gets the list of subnodes.
            /// </summary>
            public List<Objects.Location> NodeLocations { get; private set; }
            /// <summary>
            /// Gets or sets the script that this waypoint holds.
            /// </summary>
            public Cavebot.Script Script { get; set; }
            #endregion

            public enum Types
            {
                Walk = 0,
                Node = 1,
                Rope = 2,
                Shovel = 3,
                Ladder = 4,
                Machete = 5,
                Pick = 6,
                Script = 7
            }

            public override string ToString()
            {
                string wpDescriptor = this.Type.ToString() + (this.Type == Types.Node && this.NodeLocations.Count > 0 ? "+" : string.Empty) +
                    " " + this.Location.ToString();
                if (Label != null && Label != string.Empty) return "(" + this.Label + ") " + wpDescriptor;
                return wpDescriptor;
            }
        }
    }
}
