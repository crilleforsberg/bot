using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace KarelazisBot.Modules
{
    /// <summary>
    /// A class that hooks keys and executes a script when a given key is pressed.
    /// </summary>
    public partial class Hotkeys
    {
        /// <summary>
        /// Constructor for this class.
        /// </summary>
        /// <param name="c">The client to be associated with this object.</param>
        public Hotkeys(Objects.Client c)
        {
            this.Client = c;
            this.IsRunning = true;
            this.Actions = new List<Action>();
            this.KeyboardHook = new GlobalKeyboardHook();
            this.KeyboardHook.KeyDown += new KeyEventHandler(this.KeyDown);
            this.KeyboardHook.KeyUp += new KeyEventHandler(this.KeyUp);
        }

        /// <summary>
        /// Gets the client associated with this object.
        /// </summary>
        public Objects.Client Client { get; private set; }
        /// <summary>
        /// Gets whether hooked keys are currently being captured.
        /// </summary>
        public bool IsRunning { get; private set; }
        /// <summary>
        /// Gets or sets the list of actions.
        /// </summary>
        private List<Action> Actions { get; set; }
        /// <summary>
        /// Gets or sets the keyboard hook object.
        /// </summary>
        private GlobalKeyboardHook KeyboardHook { get; set; }

        #region events
        public delegate void ActionAddedHandler(Action action);
        public event ActionAddedHandler ActionAdded;
        public delegate void ActionRemovedHandler(Action action);
        public event ActionRemovedHandler ActionRemoved;
        public delegate void ActionFiredHandler(Action action);
        public event ActionFiredHandler ActionFired;
        public delegate void StatusChangedHandler(bool status);
        public event StatusChangedHandler StatusChanged;
        #endregion

        /// <summary>
        /// Starts capturing hooked keys.
        /// </summary>
        public void Start()
        {
            if (this.IsRunning) return;
            this.IsRunning = true;
            if (this.StatusChanged != null) this.StatusChanged(this.IsRunning);
        }
        /// <summary>
        /// Stops capturing hooked keys.
        /// </summary>
        public void Stop()
        {
            if (!this.IsRunning) return;
            this.IsRunning = false;
            if (this.StatusChanged != null) this.StatusChanged(this.IsRunning);
        }
        /// <summary>
        /// Checks whether a key is already hooked.
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public bool ContainsKey(Keys key)
        {
            foreach (Action a in this.GetActions())
            {
                if (a.Key == key) return true;
            }
            return false;
        }
        /// <summary>
        /// Gets an array of the currently loaded Action objects.
        /// </summary>
        /// <returns></returns>
        public IEnumerable<Action> GetActions()
        { 
            return this.Actions.ToArray();
        }
        /// <summary>
        /// Adds a new Action object.
        /// </summary>
        /// <param name="key">The key to hook.</param>
        /// <param name="scriptFile">The script file.</param>
        /// <param name="runAsync">Whether to run the script asynchronously.</param>
        public void AddAction(Keys key, FileInfo scriptFile, bool runAsync)
        {
            Action a = new Action(key, new Objects.Script(this.Client, scriptFile), runAsync);
            a.Script.Preload();
            this.Actions.Add(a);
            this.KeyboardHook.HookedKeys.Add(key);
            if (this.ActionAdded != null) this.ActionAdded(a);
        }
        /// <summary>
        /// Removes a hooked key.
        /// </summary>
        /// <param name="key"></param>
        public void RemoveKey(Keys key)
        {
            foreach (Action a in this.Actions)
            {
                if (a.Key == key)
                {
                    this.Actions.Remove(a);
                    this.KeyboardHook.HookedKeys.Add(key);
                    if (this.ActionRemoved != null) this.ActionRemoved(a);
                    return;
                }
            }
        }
        /// <summary>
        /// Removes all hooked keys.
        /// </summary>
        public void RemoveAllKeys()
        {
            foreach (Action a in this.GetActions()) this.RemoveKey(a.Key);
        }
        /// <summary>
        /// Unhooks the keyboard hook.
        /// </summary>
        public void Unhook()
        {
            this.KeyboardHook.unhook();
        }
        /// <summary>
        /// Hooks the keyboard hook.
        /// </summary>
        public void Hook()
        {
            this.KeyboardHook.hook();
        }
        /// <summary>
        /// Simulates a hotkey being pressed in Tibia.
        /// </summary>
        /// <param name="key">The key to press. Currently only supports arrow keys, F1-F12, Return, Tab and Escape.</param>
        /// <param name="controlModifier">Whether to use the CTRL modifier.</param>
        /// <param name="shiftModifier">Whether to use the SHIFT modifier.</param>
        public void Press(Keys key, bool controlModifier = false, bool shiftModifier = false)
        {
            IntPtr handle = this.Client.Window.Handle;
            if (handle == null || handle == IntPtr.Zero) return;

            if (controlModifier) WinAPI.PostMessage(handle, WinAPI.WM_KEYDOWN, WinAPI.VK_CONTROL, 0);
            if (shiftModifier) WinAPI.PostMessage(handle, WinAPI.WM_KEYDOWN, WinAPI.VK_SHIFT, 0);

            switch (key)
            {
                case Keys.Return:
                    WinAPI.PostMessage(handle, WinAPI.WM_KEYDOWN, (uint)Keys.Return, 0);
                    WinAPI.PostMessage(handle, WinAPI.WM_KEYUP, (uint)Keys.Return, 0);
                    break;
                case Keys.Escape:
                    WinAPI.PostMessage(handle, WinAPI.WM_KEYDOWN, (uint)Keys.Escape, 0);
                    WinAPI.PostMessage(handle, WinAPI.WM_KEYUP, (uint)Keys.Escape, 0);
                    break;
                case Keys.Tab:
                    WinAPI.PostMessage(handle, WinAPI.WM_CHAR, (uint)Keys.Tab, 0);
                    break;
                case Keys.Up:
                    WinAPI.PostMessage(handle, WinAPI.WM_KEYDOWN, WinAPI.VK_UP, 0);
                    WinAPI.PostMessage(handle, WinAPI.WM_KEYUP, WinAPI.VK_UP, 0);
                    break;
                case Keys.Left:
                    WinAPI.PostMessage(handle, WinAPI.WM_KEYDOWN, WinAPI.VK_LEFT, 0);
                    WinAPI.PostMessage(handle, WinAPI.WM_KEYUP, WinAPI.VK_LEFT, 0);
                    break;
                case Keys.Right:
                    WinAPI.PostMessage(handle, WinAPI.WM_KEYDOWN, WinAPI.VK_RIGHT, 0);
                    WinAPI.PostMessage(handle, WinAPI.WM_KEYUP, WinAPI.VK_RIGHT, 0);
                    break;
                case Keys.Down:
                    WinAPI.PostMessage(handle, WinAPI.WM_KEYDOWN, WinAPI.VK_DOWN, 0);
                    WinAPI.PostMessage(handle, WinAPI.WM_KEYUP, WinAPI.VK_DOWN, 0);
                    break;
                case Keys.F1:
                    WinAPI.PostMessage(handle, WinAPI.WM_KEYDOWN, WinAPI.VK_F1, 0);
                    WinAPI.PostMessage(handle, WinAPI.WM_KEYUP, WinAPI.VK_F1, 0);
                    break;
                case Keys.F2:
                    WinAPI.PostMessage(handle, WinAPI.WM_KEYDOWN, WinAPI.VK_F2, 0);
                    WinAPI.PostMessage(handle, WinAPI.WM_KEYUP, WinAPI.VK_F2, 0);
                    break;
                case Keys.F3:
                    WinAPI.PostMessage(handle, WinAPI.WM_KEYDOWN, WinAPI.VK_F3, 0);
                    WinAPI.PostMessage(handle, WinAPI.WM_KEYUP, WinAPI.VK_F3, 0);
                    break;
                case Keys.F4:
                    WinAPI.PostMessage(handle, WinAPI.WM_KEYDOWN, WinAPI.VK_F4, 0);
                    WinAPI.PostMessage(handle, WinAPI.WM_KEYUP, WinAPI.VK_F4, 0);
                    break;
                case Keys.F5:
                    WinAPI.PostMessage(handle, WinAPI.WM_KEYDOWN, WinAPI.VK_F5, 0);
                    WinAPI.PostMessage(handle, WinAPI.WM_KEYUP, WinAPI.VK_F5, 0);
                    break;
                case Keys.F6:
                    WinAPI.PostMessage(handle, WinAPI.WM_KEYDOWN, WinAPI.VK_F6, 0);
                    WinAPI.PostMessage(handle, WinAPI.WM_KEYUP, WinAPI.VK_F6, 0);
                    break;
                case Keys.F7:
                    WinAPI.PostMessage(handle, WinAPI.WM_KEYDOWN, WinAPI.VK_F7, 0);
                    WinAPI.PostMessage(handle, WinAPI.WM_KEYUP, WinAPI.VK_F7, 0);
                    break;
                case Keys.F8:
                    WinAPI.PostMessage(handle, WinAPI.WM_KEYDOWN, WinAPI.VK_F8, 0);
                    WinAPI.PostMessage(handle, WinAPI.WM_KEYUP, WinAPI.VK_F8, 0);
                    break;
                case Keys.F9:
                    WinAPI.PostMessage(handle, WinAPI.WM_KEYDOWN, WinAPI.VK_F9, 0);
                    WinAPI.PostMessage(handle, WinAPI.WM_KEYUP, WinAPI.VK_F9, 0);
                    break;
                case Keys.F10:
                    WinAPI.PostMessage(handle, WinAPI.WM_KEYDOWN, WinAPI.VK_F10, 0);
                    WinAPI.PostMessage(handle, WinAPI.WM_KEYUP, WinAPI.VK_F10, 0);
                    break;
                case Keys.F11:
                    WinAPI.PostMessage(handle, WinAPI.WM_KEYDOWN, WinAPI.VK_F11, 0);
                    WinAPI.PostMessage(handle, WinAPI.WM_KEYUP, WinAPI.VK_F11, 0);
                    break;
                case Keys.F12:
                    WinAPI.PostMessage(handle, WinAPI.WM_KEYDOWN, WinAPI.VK_F12, 0);
                    WinAPI.PostMessage(handle, WinAPI.WM_KEYUP, WinAPI.VK_F12, 0);
                    break;
            }

            if (controlModifier) WinAPI.PostMessage(handle, WinAPI.WM_KEYUP, WinAPI.VK_CONTROL, 0);
            if (shiftModifier) WinAPI.PostMessage(handle, WinAPI.WM_KEYUP, WinAPI.VK_SHIFT, 0);
        }
        public void Write(string s)
        {
            IntPtr handle = this.Client.TibiaProcess.MainWindowHandle;
            if (handle == null || handle == IntPtr.Zero) return;

            foreach (char c in s) WinAPI.PostMessage(handle, WinAPI.WM_CHAR, (uint)c, 0);
        }
        public void SendLeftMouseClick(int x, int y)
        {
            int LParam = WinAPI.MakeLParam(x, y);
            WinAPI.SendMessage(this.Client.Window.Handle, WinAPI.WM_LBUTTONDOWN, 0, LParam);
            WinAPI.SendMessage(this.Client.Window.Handle, WinAPI.WM_LBUTTONUP, 0, LParam);
        }

        private void KeyDown(object sender, KeyEventArgs e)
        {
            e.Handled = false;
            if (!this.IsRunning || !this.Client.Window.IsFocused()) return;

            foreach (Action action in this.Actions)
            {
                if (action.Key == e.KeyCode)
                {
                    e.Handled = true;

                    if (!action.KeyDown) break;

                    action.Script.Run(action.RunAsynchronous);
                    if (this.ActionFired != null) this.ActionFired(action);
                    return;
                }
            }
        }
        private void KeyUp(object sender, KeyEventArgs e)
        {
            e.Handled = false;
            if (!this.IsRunning || !this.Client.Window.IsFocused()) return;

            foreach (Action action in this.Actions)
            {
                if (action.Key == e.KeyCode)
                {
                    e.Handled = true;

                    if (action.KeyDown) break;

                    action.Script.Run(action.RunAsynchronous);
                    if (this.ActionFired != null) this.ActionFired(action);
                    return;
                }
            }
        }
    }
}
