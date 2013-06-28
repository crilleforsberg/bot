using System;

namespace KarelazisBot.Modules
{
    /// <summary>
    /// A class used to to calculate values related to the player's experience and level.
    /// </summary>
    public class ExperienceCounter
    {
        /// <summary>
        /// Constructor for this class.
        /// </summary>
        /// <param name="c">The client to be associated with this object.</param>
        public ExperienceCounter(Objects.Client c)
        {
            this.Client = c;
            this.Stopwatch = new System.Diagnostics.Stopwatch();
            this.TNLSource = TnlSource.Formula;
        }

        #region get-sets
        /// <summary>
        /// Gets the client associated with this object.
        /// </summary>
        public Objects.Client Client { get; private set; }
        /// <summary>
        /// Gets whether the counter is currently running.
        /// </summary>
        public bool IsRunning { get; private set; }
        /// <summary>
        /// Gets or sets the source for calculating time to next level.
        /// </summary>
        public TnlSource TNLSource { get; set; }
        /// <summary>
        /// Gets or sets the stopwatch used to count values such as experience per hour.
        /// </summary>
        private System.Diagnostics.Stopwatch Stopwatch { get; set; }
        /// <summary>
        /// Placeholder for the player's experience when the counter first started.
        /// </summary>
        private uint OldExperience { get; set; }
        /// <summary>
        /// Placeholder for the player's level when the counter first started.
        /// </summary>
        private uint OldLevel { get; set; }
        /// <summary>
        /// Placeholder for the player's level percent when the counter first started.
        /// </summary>
        private int OldLevelPercent { get; set; }
        /// <summary>
        /// Needed when levelling up.
        /// </summary>
        private uint TotalGainedLevelPercent { get; set; }
        #endregion

        public enum TnlSource
        {
            Formula,
            LevelPercent
        }

        #region methods
        /// <summary>
        /// Starts the counter.
        /// </summary>
        public void Start()
        {
            if (this.IsRunning) return;
            this.IsRunning = true;
            this.Reset();
        }
        /// <summary>
        /// Stops the counter.
        /// </summary>
        public void Stop()
        {
            if (!this.IsRunning) return;
            this.Reset();
            this.Stopwatch.Stop();
        }
        /// <summary>
        /// Resets placeholder values and the counter.
        /// </summary>
        public void Reset()
        {
            this.OldExperience = this.Client.Player.Experience;
            this.OldLevel = this.Client.Player.Level;
            this.OldLevelPercent = 100 - this.Client.Player.LevelPercent;
            this.Stopwatch.Reset();
            this.Stopwatch.Start();
        }
        /// <summary>
        /// Pauses the counter.
        /// </summary>
        public void Pause()
        {
            if (this.Stopwatch.IsRunning) this.Stopwatch.Stop();
        }
        /// <summary>
        /// Resume the counter.
        /// </summary>
        public void Resume()
        {
            if (!this.Stopwatch.IsRunning) this.Stopwatch.Start();
        }
        /// <summary>
        /// Gets the amount of gained experience since the counter started.
        /// </summary>
        /// <returns></returns>
        public uint GetGainedExperience()
        {
            uint expNew = this.Client.Player.Experience;
            long expDiff = expNew - this.OldExperience;
            if (expDiff <= 0)
            {
                this.Reset();
                return 0;
            }
            return (uint)expDiff;
        }
        /// <summary>
        /// Gets the amount of level percent gained since the counter started.
        /// </summary>
        /// <returns></returns>
        public uint GetGainedLevelPercent()
        {
            int levelPercentNew = 100 - this.Client.Player.LevelPercent;
            uint levelNew = this.Client.Player.Level;
            if (this.OldLevel < levelNew) // levelled up, time to adjust shiz
            {
                int levelDiff = (int)(levelNew - this.OldLevel);
                if (levelDiff > 1) // gained more than a single level
                {
                    this.TotalGainedLevelPercent += (uint)(levelDiff * 100 + this.OldLevelPercent - levelPercentNew);
                }
                this.OldLevelPercent = levelPercentNew;
                this.OldLevel = levelNew;
            }
            return (uint)(this.OldLevelPercent - levelPercentNew + this.TotalGainedLevelPercent);
        }
        /// <summary>
        /// Gets the amount of estimated experience gained per hour.
        /// </summary>
        /// <returns></returns>
        public uint GetExperiencePerHour()
        {
            // check if elapsed seconds is less than 1, to prevent division by zero errors
            if (this.Stopwatch.Elapsed.TotalSeconds < 1) return 0;
            uint expGained = this.GetGainedExperience();
            uint expPerHour = (uint)Math.Ceiling((double)expGained / this.Stopwatch.Elapsed.TotalSeconds * 3600);
            return expPerHour;
        }
        /// <summary>
        /// Gets the amount of estimated level percent gained per hour.
        /// </summary>
        /// <returns></returns>
        public uint GetLevelPercentPerHour()
        {
            uint levelPercentGained = this.GetGainedLevelPercent();
            if (levelPercentGained == 0) return 0;
            return (uint)Math.Ceiling((double)levelPercentGained / this.Stopwatch.Elapsed.TotalSeconds * 3600);
        }
        /// <summary>
        /// Gets the amount of experience to the player's next level.
        /// </summary>
        /// <returns></returns>
        public uint GetExperienceTNL()
        {
            uint level = this.Client.Player.Level;
            switch (this.TNLSource)
            {
                case TnlSource.Formula:
                    return (uint)((50 * (level + 1) * (level + 1) * (level + 1) - 150 * (level + 1) * (level + 1) + 400 * (level + 1)) / 3 - this.Client.Player.Experience);
                case TnlSource.LevelPercent:
                    uint expGained = this.GetGainedExperience();
                    uint levelPercentGained = this.GetGainedLevelPercent();
                    if (expGained <= 0 || levelPercentGained <= 0) return 0;
                    uint estimatedExperiencePerLevelPercent = expGained / levelPercentGained;
                    uint levelPercentTNL = (uint)(100 - this.Client.Player.LevelPercent);
                    return estimatedExperiencePerLevelPercent * levelPercentTNL;
                default:
                    return 0;
            }
        }
        /// <summary>
        /// Gets the amount of seconds until the player's next level. Returns 0 if unsucessful or infinite.
        /// </summary>
        /// <returns></returns>
        public uint GetTimeTNL()
        {
            uint expPerHour = this.GetExperiencePerHour();
            if (expPerHour <= 0) return 0;
            uint timeLeft = 0;
            switch (this.TNLSource)
            {
                case TnlSource.Formula:
                    timeLeft = (uint)(Math.Round((double)this.GetExperienceTNL() * 3600 / (double)expPerHour));
                    return timeLeft;
                case TnlSource.LevelPercent:
                    uint levelPercentNew = (uint)(100 - this.Client.Player.LevelPercent);
                    uint levelPercentPerHour = this.GetLevelPercentPerHour();
                    if (levelPercentPerHour == 0) break;
                    return (uint)(Math.Round((double)levelPercentNew * 3600 / (double)levelPercentPerHour));
                default:
                    return 0;
            }
            return 0;
        }
        /// <summary>
        /// Gets the time left until next level up as a formatted string (hh:mm:ss).
        /// </summary>
        /// <returns></returns>
        public string GetTimeTNLString()
        {
            TimeSpan ts = TimeSpan.FromSeconds(this.GetTimeTNL());
            return string.Format("{0:D2}:{1:D2}:{2:D2}", ts.Hours, ts.Minutes, ts.Seconds);
        }
        /// <summary>
        /// Gets the time elapsed so far in seconds.
        /// </summary>
        /// <returns></returns>
        public uint GetTimeElapsed()
        {
            return (uint)this.Stopwatch.Elapsed.TotalSeconds;
        }
        /// <summary>
        /// Gets the time elapsed as a formatted string (hh:mm:ss).
        /// </summary>
        /// <returns></returns>
        public string GetTimeElapsedString()
        {
            TimeSpan ts = this.Stopwatch.Elapsed;
            return string.Format("{0:D2}:{1:D2}:{2:D2}", ts.Hours, ts.Minutes, ts.Seconds);
        }
        #endregion
    }
}
