using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KarelazisBot.Modules
{
    public partial class Cavebot
    {
        /// <summary>
        /// A class that allows scripts to be run in the cavebot.
        /// </summary>
        public class Script
        {
            #region constructors
            /// <summary>
            /// Constructor for this class.
            /// </summary>
            /// <param name="parent">The Cavebot module that will host this script.</param>
            /// <param name="code">The C# code that will be run when this script is executed.</param>
            public Script(Cavebot parent, string code)
                : this(parent, code, null) { }
            /// <summary>
            /// Constructor for this class.
            /// </summary>
            /// <param name="parent">The Cavebot module that will host this script.</param>
            /// <param name="code">The C# code that will be run when this script is executed.</param>
            /// <param name="waypoint">The waypoint to associate with this script.</param>
            public Script(Cavebot parent, string code, Waypoint waypoint)
            {
                this.Parent = parent;
                this.Code = code;
                this.Waypoint = waypoint;
            }
            #endregion

            #region properties
            /// <summary>
            /// Gets the Cavebot module that will hold this script.
            /// </summary>
            public Cavebot Parent { get; private set; }
            /// <summary>
            /// Gets or sets the C# code that will be executed.
            /// </summary>
            public string Code { get; set; }
            /// <summary>
            /// Gets the waypoint associated with this Script object. May be null.
            /// </summary>
            public Waypoint Waypoint { get; private set; }
            /// <summary>
            /// Gets or sets the FileInfo object associated with this Script object.
            /// </summary>
            private System.IO.FileInfo FileInfo { get; set; }
            /// <summary>
            /// Gets or sets the Objects.Script object.
            /// </summary>
            private Objects.Script ScriptPlaceholder { get; set; }
            #endregion

            #region methods
            /// <summary>
            /// Executes the script.
            /// </summary>
            /// <param name="asynchronous">Whether to run the script on another thread or not.</param>
            public void Run(bool asynchronous)
            {
                if (this.Code.Length <= 0) return;
                this.FileInfo = this.Parent.Client.Modules.CacheManager.SetFile(new Random().Next() + "tempcavebotscript.cs",
                    System.Text.Encoding.UTF8.GetBytes(this.Code));
                this.ScriptPlaceholder = new Objects.Script(this.Parent.Client, this.FileInfo);
                this.ScriptPlaceholder.Finished += new Objects.Script.ScriptFinishedHandler(delegate(Objects.Script script)
                {
                    this.FileInfo.Delete();
                    this.ScriptPlaceholder = null;
                });
                if (!this.ScriptPlaceholder.Run(asynchronous)) this.FileInfo.Delete();
            }
            /// <summary>
            /// Stop running this script.
            /// </summary>
            public void Stop()
            {
                if (this.ScriptPlaceholder != null) this.ScriptPlaceholder.Stop();
            }
            #endregion
        }
    }
}
