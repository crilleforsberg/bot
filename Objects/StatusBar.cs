using System;
using System.Collections.Generic;
using System.Text;

namespace KarelazisBot.Objects
{
    /// <summary>
    /// A class that offers interaction with the client's status bar.
    /// </summary>
    public class StatusBar
    {
        /// <summary>
        /// Constructor for this class.
        /// </summary>
        /// <param name="c">The client to be associated with this object.</param>
        public StatusBar(Objects.Client c)
        { 
            this.Client = c;
        }

        /// <summary>
        /// Gets the client associated with this object.
        /// </summary>
        public Objects.Client Client { get; private set; }

        /// <summary>
        /// Gets the currently shown text in the status bar. Returns string.Empty if no text is shown.
        /// </summary>
        /// <returns></returns>
        public string GetText()
        {
            return this.Client.Memory.ReadByte(this.Client.Addresses.UI.StatusBarTime) > 0 ?
                this.Client.Memory.ReadString(this.Client.Addresses.UI.StatusBarText, 64, true) : string.Empty;
        }
        /// <summary>
        /// Sets the status bar's text for 4 seconds.
        /// </summary>
        /// <param name="text">The text to set.</param>
        public void SetText(string text)
        {
            this.SetText(text, 4);
        }
        /// <summary>
        /// Sets the status bar's text for a given amount of time.
        /// </summary>
        /// <param name="text">The text to set.</param>
        /// <param name="seconds">The amount of seconds to display the text for.</param>
        public void SetText(string text, byte seconds)
        {
            this.Client.Memory.WriteString(this.Client.Addresses.UI.StatusBarText, text);
            this.Client.Memory.WriteByte(this.Client.Addresses.UI.StatusBarTime, !string.IsNullOrEmpty(text) ? (byte)(seconds * 10) : (byte)0);
        }
    }
}
