using System;
using System.Collections.Generic;
using System.Drawing;
using System.Threading;

namespace KarelazisBot.Objects
{
    /// <summary>
    /// A class that offers interaction with the client window.
    /// </summary>
    public class Window
    {
        /// <summary>
        /// Constructor for this class.
        /// </summary>
        /// <param name="c">The client to be associated with this object.</param>
        public Window(Objects.Client c)
        {
            this.Client = c;
            this.StatusBar = new Objects.StatusBar(c);
            this.GameWindow = new Objects.GameWindow(c);
        }

        /// <summary>
        /// Gets the client associated with this object.
        /// </summary>
        public Objects.Client Client { get; private set; }
        public Objects.StatusBar StatusBar { get; private set; }
        public Objects.GameWindow GameWindow { get; private set; }
        public IntPtr Handle
        {
            get { return this.Client.TibiaProcess.MainWindowHandle; }
        }

        public string GetCurrentDialogTitle()
        {
            if (this.Client.Memory.ReadByte(this.Client.Addresses.UI.ActionState) != (byte)Enums.ActionStates.Dialog) return string.Empty;
            return this.Client.Memory.ReadString(this.Client.Memory.ReadInt32(this.Client.Addresses.Dialog.Pointer) +
                this.Client.Addresses.Dialog.Distances.Title);
        }
        /// <summary>
        /// Sets the Tibia client's window text.
        /// </summary>
        /// <param name="text"></param>
        public void SetTitleText(string text)
        {
            // create new thread because it's a synchronous call
            // and if the fps is low, it'll lock up the calling thread
            new Thread(delegate()
                {
                    WinAPI.SetWindowText(this.Client.TibiaProcess.MainWindowHandle, text);
                }).Start();
        }
        /// <summary>
        /// Checks whether the Tibia client's window is minimized.
        /// </summary>
        /// <returns></returns>
        public bool IsMinimized()
        {
            return WinAPI.IsIconic(this.Handle);
        }
        /// <summary>
        /// Checks whether the Tibia client's window is maximized.
        /// </summary>
        /// <returns></returns>
        public bool IsMaximized()
        {
            return WinAPI.IsZoomed(this.Handle);
        }
        public bool IsFocused()
        {
            return WinAPI.GetForegroundWindow() == this.Handle;
        }
        /// <summary>
        /// Gets a struct containing information about the window, including screen and client rectangles.
        /// </summary>
        /// <returns></returns>
        public WinAPI.WINDOWINFO GetWindowInfo()
        {
            WinAPI.WINDOWINFO info = new WinAPI.WINDOWINFO(true);
            WinAPI.GetWindowInfo(this.Handle, ref info);
            return info;
        }
        public Size GetWindowSize()
        {
            return new Size(this.Client.Memory.ReadInt32(this.Client.Addresses.UI.WindowWidth),
                this.Client.Memory.ReadInt32(this.Client.Addresses.UI.WindowHeight));
        }
    }
}
