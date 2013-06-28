
using System.Collections.Generic;
using System.IO;
using System.Threading;
using CSScriptLibrary;

namespace KarelazisBot.Modules
{
    /// <summary>
    /// A simple class to manage a collection of scripts.
    /// </summary>
    public partial class ScriptManager : IEnumerable<Objects.Script>
    {
        /// <summary>
        /// Constructor for this class.
        /// </summary>
        public ScriptManager(Objects.Client client)
        {
            this.Client = client;
            this.Scripts = new List<Objects.Script>();
            this.Variables = new VariableCollection(this);
            this.Libraries = new Dictionary<string, AsmHelper>();
            this.ScriptFinishedHandler = new Objects.Script.ScriptFinishedHandler(this.scriptFinished);
            this.ScriptStartedHandler = new Objects.Script.ScriptStartedHandler(this.scriptStarted);
            CSScript.CacheEnabled = false;
            CSScript.GlobalSettings.InMemoryAsssembly = true;
        }

        private Objects.Script.ScriptStartedHandler ScriptStartedHandler;
        private Objects.Script.ScriptFinishedHandler ScriptFinishedHandler;
        /// <summary>
        /// Gets or sets the collection of scripts this class manages.
        /// </summary>
        private List<Objects.Script> Scripts { get; set; }
        /// <summary>
        /// Gets a dictionary used for storing variables for re-use in scripts.
        /// Key = variable name, Value = value.
        /// </summary>
        public VariableCollection Variables { get; private set; }
        /// <summary>
        /// Gets a dictionary used for interacting with loaded libraries.
        /// Key = class name, Value = assembly helper object.
        /// </summary>
        public Dictionary<string, AsmHelper> Libraries { get; private set; }
        public Objects.Client Client { get; private set; }

        #region events
        public delegate void GenericScriptHandler(Objects.Script script);
        public event GenericScriptHandler ScriptFinished;
        public event GenericScriptHandler ScriptStarted;
        public event GenericScriptHandler ScriptAdded;
        public event GenericScriptHandler ScriptRemoved;
        public delegate void GenericLibraryHandler(string key);
        public event GenericLibraryHandler LibraryAdded;
        public event GenericLibraryHandler LibraryRemoved;
        #endregion

        #region private methods
        private void scriptStarted(Objects.Script script)
        {
            if (this.ScriptStarted != null) this.ScriptStarted(script);
        }
        private void scriptFinished(Objects.Script script)
        {
            if (this.ScriptFinished != null) this.ScriptFinished(script);
        }
        #endregion

        #region public methods
        /// <summary>
        /// Returns an array of scripts that are currently loaded.
        /// </summary>
        /// <returns></returns>
        public IEnumerable<Objects.Script> GetScripts()
        {
            return this.Scripts.ToArray();
        }
        /// <summary>
        /// Adds a script.
        /// </summary>
        /// <param name="script">The script to add.</param>
        public void AddScript(Objects.Script script)
        {
            this.Scripts.Add(script);
            if (this.ScriptAdded != null) this.ScriptAdded(script);
            script.Started += this.ScriptStartedHandler;
            script.Finished += this.ScriptFinishedHandler;
        }
        /// <summary>
        /// Removes a script.
        /// </summary>
        /// <param name="script">The script to remove.</param>
        /// <returns></returns>
        public bool RemoveScript(Objects.Script script)
        {
            if (this.Scripts.Remove(script))
            {
                script.Stop();
                script.Started -= this.ScriptStartedHandler;
                script.Finished -= this.ScriptFinishedHandler;
                if (this.ScriptRemoved != null) this.ScriptRemoved(script);
                return true;
            }
            return false;
        }
        /// <summary>
        /// Removes all scripts.
        /// </summary>
        public void RemoveAllScripts()
        {
            foreach (Objects.Script script in this.GetScripts()) this.RemoveScript(script);
        }
        /// <summary>
        /// Starts all scripts.
        /// </summary>
        public void StartAllScripts()
        {
            new Thread(delegate()
                {
                    foreach (Objects.Script script in this.Scripts)
                    {
                        if (script.IsRunning) continue;
                        if (!script.Run(true)) continue;
                        script.ResetEventLoaded.WaitOne(5000);
                        Thread.Sleep(2000);
                    }
                }).Start();
        }
        /// <summary>
        /// Stops all scripts.
        /// </summary>
        public void StopAllScripts()
        {
            foreach (Objects.Script script in this.Scripts)
            {
                if (script.IsRunning) script.Stop();
            }
        }
        public bool AddLibrary(FileInfo fi)
        {
            AsmHelper helper = new AsmHelper(CSScript.Load(fi.FullName));
            helper.CachingEnabled = false;
            string className = (string)helper.Invoke("*.GetClassName");
            if (this.Libraries.ContainsKey(className))
            {
                helper.Dispose();
                return false;
            }
            this.Libraries.Add(className, helper);
            if (this.LibraryAdded != null) this.LibraryAdded(className);
            return true;
        }
        public bool RemoveLibrary(string key)
        {
            if (!this.Libraries.Remove(key)) return false;
            if (this.LibraryRemoved != null) this.LibraryRemoved(key);
            return true;
        }
        public Objects.Script CreateScript()
        {
            return new Objects.Script(this.Client);
        }
        public Objects.Script CreateScript(FileInfo fi)
        {
            return new Objects.Script(this.Client, fi);
        }
        public Objects.Script this[int index]
        {
            get { return this.Scripts[index]; }
        }
        #endregion

        #region interface implementations
        public IEnumerator<Objects.Script> GetEnumerator()
        {
            return this.Scripts.GetEnumerator();
        }
        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return this.Scripts.GetEnumerator();
        }
        #endregion
    }
}
