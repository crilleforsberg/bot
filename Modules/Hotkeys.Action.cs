using System.Windows.Forms;

namespace KarelazisBot.Modules
{
    public partial class Hotkeys
    {
        /// <summary>
        /// A collection class for a key and a script.
        /// </summary>
        public class Action
        {
            /// <summary>
            /// Constructor for this class.
            /// </summary>
            /// <param name="key">The key to check for.</param>
            /// <param name="script">The script to execute when the key is pressed.</param>
            /// <param name="runAsync">Whether to run the script asynchronously.</param>
            public Action(Keys key, Objects.Script script, bool runAsync, bool keyDown = true)
            {
                this.Key = key;
                this.Script = script;
                this.RunAsynchronous = runAsync;
                this.KeyDown = keyDown;
            }

            /// <summary>
            /// The key that will act as a trigger for the script.
            /// </summary>
            public Keys Key { get; set; }
            /// <summary>
            /// The script to execute when the key is pressed.
            /// </summary>
            public Objects.Script Script { get; set; }
            /// <summary>
            /// Whether to run the script asynchronously.
            /// </summary>
            public bool RunAsynchronous { get; set; }
            /// <summary>
            /// Whether the script should be run on KeyDown.
            /// If this is set to false, it will instead run on KeyUp.
            /// </summary>
            public bool KeyDown { get; set; }
        }
    }
}
