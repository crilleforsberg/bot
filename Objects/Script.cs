using System;
using System.Speech.Synthesis;
using System.Threading;
using System.IO;
using CSScriptLibrary;

namespace KarelazisBot.Objects
{
    /// <summary>
    /// A class that handles scripting in C#.NET.
    /// </summary>
    public class Script
    {
        #region constructors
        /// <summary>
        /// Constructor for an empty script.
        /// </summary>
        /// <param name="c">The Objects.Client object that will host this Script object.</param>
        public Script(Objects.Client c) : this(c, null, new Location()) { }
        /// <summary>
        /// Constructor for a script in a file.
        /// </summary>
        /// <param name="c">The Objects.Client object that will host this Script object.</param>
        /// <param name="scriptFile">The script file.</param>
        public Script(Objects.Client c, FileInfo scriptFile) : this(c, scriptFile, new Location()) { }
        /// <summary>
        /// Constructor for a script in a file and is associated with a world location.
        /// </summary>
        /// <param name="c">The Objects.Client object that will host this Script object.</param>
        /// <param name="scriptFileh">The script file.</param>
        /// <param name="wpLocation">The world location associated with this script.</param>
        public Script(Objects.Client c, FileInfo scriptFile, Objects.Location wpLocation)
        {
            this.Client = c;
            this.ScriptFile = scriptFile;
            this.WaypointLocation = wpLocation;
            this.TTS = new SpeechSynthesizer();
            this.ResetEventLoaded = new AutoResetEvent(false);
        }
        #endregion

        #region properties
        /// <summary>
        /// Gets the Objects.Client object that hosts this Script object.
        /// </summary>
        public Objects.Client Client { get; private set; }
        /// <summary>
        /// Gets or sets the file path to the script.
        /// </summary>
        public FileInfo ScriptFile { get; set; }
        /// <summary>
        /// Gets whether the script is running.
        /// </summary>
        public bool IsRunning { get; private set; }
        /// <summary>
        /// Gets or sets the world location associated with this script.
        /// </summary>
        public Objects.Location WaypointLocation { get; set; }
        /// <summary>
        /// Gets a SpeechSynthesizer object, used primarily for alarms.
        /// </summary>
        public SpeechSynthesizer TTS { get; private set; }
        /// <summary>
        /// Gets or sets the thread dedicated to running the script.
        /// </summary>
        private Thread DedicatedThread { get; set; }
        /// <summary>
        /// Gets or sets whether [Running/Stopped] is displayed in ToString().
        /// </summary>
        public bool ShowStatus { get; set; }
        /// <summary>
        /// Gets a reset event used for waiting until this script has finished loading,
        /// useful for quickly loading many scripts.
        /// </summary>
        public AutoResetEvent ResetEventLoaded { get; private set; }

        private bool Loaded { get; set; }
        private AsmHelper AssemblyHelper { get; set; }
        private object Instance { get; set; }
        #endregion

        #region events
        public delegate void ScriptFinishedHandler(Objects.Script script);
        public event ScriptFinishedHandler Finished;
        public delegate void ScriptStartedHandler(Objects.Script script);
        public event ScriptStartedHandler Started;
        #endregion

        #region methods
        public override string ToString()
        {
            return this.ScriptFile.Name + (this.ShowStatus ? " [" + (this.IsRunning ? "Running" : "Stopped") + "]" : string.Empty);
        }

        /// <summary>
        /// Executes this script.
        /// </summary>
        /// <param name="runAsync">Whether to run this script on a new thread or not.</param>
        public bool Run(bool runAsync = false)
        {
            if (this.IsRunning || this.ScriptFile == null || !this.ScriptFile.Exists) return false;
            this.IsRunning = true;
            if (runAsync)
            {
                this.DedicatedThread = new Thread(this.Execute);
                this.DedicatedThread.Start();
            }
            else this.Execute();
            return true;
        }
        /// <summary>
        /// Stops this script.
        /// </summary>
        public void Stop()
        {
            if (!this.IsRunning) return;
            this.IsRunning = false;
            this.CleanUp();
            if (this.DedicatedThread != null && this.DedicatedThread.IsAlive) this.DedicatedThread.Abort();
        }
        /// <summary>
        /// Preloads this script, to speed up execution.
        /// Useful for i.e. hotkeys, that runs the script several times in a short period of time.
        /// </summary>
        public void Preload()
        {
            if (this.Loaded && this.AssemblyHelper != null) return;
            this.ForcePreload();
        }
        /// <summary>
        /// Preloads regardless if this script has already been loaded.
        /// Useful for reloading scripts that have been altered since this object was created.
        /// </summary>
        public void ForcePreload()
        {
            this.AssemblyHelper = new AsmHelper(CSScript.Load(this.ScriptFile.FullName));
            this.AssemblyHelper.CachingEnabled = false;
            this.Loaded = true;
        }

        /// <summary>
        /// Executes this script.
        /// </summary>
        private void Execute()
        {
            try
            {
                // load script
                this.Preload();
                // set reset event, in case something is waiting for it
                this.ResetEventLoaded.Set();
                // raise event that script is about to start, if possible
                if (this.Started != null) this.Started(this);
                // run script and store the instance for later use
                this.Instance = this.AssemblyHelper.Invoke("*.Main", this.Client);
            }
            catch (Exception ex)
            {
                if (ex is System.Threading.ThreadAbortException) { }
                else
                {
                    System.Windows.Forms.MessageBox.Show(ex.Message + "\n" + ex.StackTrace,
                        this.ScriptFile != null ? this.ScriptFile.Name : string.Empty);
                }
            }
            finally
            {
                if (this.IsRunning)
                {
                    this.CleanUp();
                    this.IsRunning = false;
                }
                this.ResetEventLoaded.Reset();
                if (this.Finished != null) this.Finished(this);
            }
        }
        private bool CleanUp()
        {
            try
            {
                if (this.AssemblyHelper == null) return false;
                if (this.Instance != null) this.AssemblyHelper.InvokeInst(this.Instance, "*.Cleanup", this.Client);
                else this.AssemblyHelper.Invoke("*.Cleanup", this.Client);
                return true;
            }
            catch { return false; }
        }
        #endregion
    }
}
