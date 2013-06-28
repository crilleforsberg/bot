using System;
using System.Collections.Generic;
using System.Text;

namespace KarelazisBot.Objects
{
    class Settings
    {
        internal Settings(Objects.Client c)
        {
            this.InitializeClasses();
        }

        private Client _Client { get; set; }
        internal Client Client { get { return this._Client; } }

        internal readonly ushort BotVersion = 201;

        internal HealerClass Healer { get; set; }
        internal ExperienceCounterClass ExperienceCounter { get; set; }
        internal HotkeysClass Hotkeys { get; set; }
        internal MagebombClass Magebomb { get; set; }
        internal PvPClass PvP { get; set; }
        internal ScripterClass Scripter { get; set; }
        internal SecurityClass Security { get; set; }

        internal void InitializeClasses()
        {
            this.Healer = new HealerClass();
            this.ExperienceCounter = new ExperienceCounterClass();
            this.Hotkeys = new HotkeysClass();
            this.Magebomb = new MagebombClass();
            this.PvP = new PvPClass();
            this.Scripter = new ScripterClass();
            this.Security = new SecurityClass();
        }

        internal class HealerClass
        {
            internal HealerClass()
            {
                this.Stopwatch = new System.Diagnostics.Stopwatch();
            }

            internal void LoadDefaults()
            {
                this.Status = false;
                this.SleepMin = 400;
                this.SleepMax = 700;
                this.CheckInterval = 500;
            }

            private bool _Status { get; set; }
            internal bool Status
            {
                get { return this._Status; }
                set
                {
                    if (value)
                    {
                        this.Stopwatch.Reset();
                        this.Stopwatch.Start();
                        this._Status = true;
                    }
                    else
                    {
                        this.Stopwatch.Stop();
                        this._Status = false;
                    }
                }
            }
            internal System.Diagnostics.Stopwatch Stopwatch { get; set; }
            internal ushort SleepMin { get; set; }
            internal ushort SleepMax { get; set; }
            internal ushort CheckInterval { get; set; }
        }
        internal class ExperienceCounterClass
        {
            internal ExperienceCounterClass()
            {
                this.Stopwatch = new System.Diagnostics.Stopwatch();
            }

            internal void LoadDefaults()
            {
                this.Status = false;
            }

            private bool _Status { get; set; }
            internal bool Status
            {
                get { return this._Status; }
                set
                {
                    if (value)
                    {
                        this.Stopwatch.Reset();
                        this.Stopwatch.Start();
                        this._Status = true;
                    }
                    else
                    {
                        this.Stopwatch.Stop();
                        this._Status = false;
                    }
                }
            }
            internal System.Diagnostics.Stopwatch Stopwatch { get; set; }
        }
        internal class HotkeysClass
        {
            internal HotkeysClass()
            {
                this.Stopwatch = new System.Diagnostics.Stopwatch();
                this.KeysActions = new Dictionary<System.Windows.Forms.Keys, Objects.Script>();
                this.KeyboardHook = new globalKeyboardHook();
            }

            internal void LoadDefaults()
            {
                this.Status = false;
                this.KeysActions.Clear();
                this.KeyboardHook.HookedKeys.Clear();
            }

            private bool _Status { get; set; }
            internal bool Status
            {
                get { return this._Status; }
                set
                {
                    if (value)
                    {
                        this.Stopwatch.Reset();
                        this.Stopwatch.Start();
                        this._Status = true;
                    }
                    else
                    {
                        this.Stopwatch.Stop();
                        this._Status = false;
                    }
                }
            }
            internal Objects.globalKeyboardHook KeyboardHook { get; set; }
            internal Dictionary<System.Windows.Forms.Keys, Objects.Script> KeysActions { get; set; }
            internal System.Diagnostics.Stopwatch Stopwatch { get; set; }

            internal void Add(System.Windows.Forms.Keys key, Objects.Script script)
            {
                this.KeyboardHook.HookedKeys.Add(key);
                this.KeysActions.Add(key, script);
            }
            internal void Remove(System.Windows.Forms.Keys key)
            {
                this.KeyboardHook.HookedKeys.Remove(key);
                this.KeysActions.Remove(key);
            }
        }
        internal class MagebombClass
        {
            internal MagebombClass()
            {
                this.Stopwatch = new System.Diagnostics.Stopwatch();
            }

            internal void LoadDefaults()
            {
                this.Status = false;
            }

            private bool _Status { get; set; }
            internal bool Status
            {
                get { return this._Status; }
                set
                {
                    if (value)
                    {
                        this.Stopwatch.Reset();
                        this.Stopwatch.Start();
                        this._Status = true;
                    }
                    else
                    {
                        this.Stopwatch.Stop();
                        this._Status = false;
                    }
                }
            }
            internal System.Diagnostics.Stopwatch Stopwatch { get; set; }

        }
        internal class PvPClass
        {
            internal PvPClass()
            {
                this.CombobotClient = new CombobotServerClass();
                this.CombobotServer = new CombobotClientClass();
                this.CombobotClient.Stopwatch = new System.Diagnostics.Stopwatch();
                this.CombobotServer.Stopwatch = new System.Diagnostics.Stopwatch();
            }

            internal void LoadDefaults()
            {
                this.CombobotClient.Status = false;
                this.CombobotServer.Status = false;
            }

            internal bool KeepAttack { get; set; }
            internal uint KeepAttackID { get; set; }
            internal CombobotClientClass CombobotServer { get; set; }
            internal CombobotServerClass CombobotClient { get; set; }

            internal class CombobotClientClass
            {
                private bool _Status { get; set; }
                internal bool Status
                {
                    get { return this._Status; }
                    set
                    {
                        if (value)
                        {
                            this.Stopwatch.Reset();
                            this.Stopwatch.Start();
                            this._Status = true;
                        }
                        else
                        {
                            this.Stopwatch.Stop();
                            this._Status = false;
                        }
                    }
                }
                internal System.Diagnostics.Stopwatch Stopwatch { get; set; }
                internal string ServerIP { get; set; }
                internal ushort ServerPort { get; set; }
            }
            internal class CombobotServerClass
            {
                private bool _Status { get; set; }
                internal bool Status
                {
                    get { return this._Status; }
                    set
                    {
                        if (value)
                        {
                            this.Stopwatch.Reset();
                            this.Stopwatch.Start();
                            this._Status = true;
                        }
                        else
                        {
                            this.Stopwatch.Stop();
                            this._Status = false;
                        }
                    }
                }
                internal System.Diagnostics.Stopwatch Stopwatch { get; set; }
                internal ushort ListenerPort { get; set; }
            }
        }
        internal class ScripterClass
        {
            internal ScripterClass()
            {
                this.Stopwatch = new System.Diagnostics.Stopwatch();
            }

            internal void LoadDefaults()
            {
                this.Status = false;
            }

            private bool _Status { get; set; }
            internal bool Status
            {
                get { return this._Status; }
                set
                {
                    if (value)
                    {
                        this.Stopwatch.Reset();
                        this.Stopwatch.Start();
                        this._Status = true;
                    }
                    else
                    {
                        this.Stopwatch.Stop();
                        this._Status = false;
                    }
                }
            }
            internal System.Diagnostics.Stopwatch Stopwatch { get; set; }
        }
        internal class SecurityClass
        {
            internal SecurityClass()
            {
                this.Alarms = new AlarmsClass();
                this.Actions = new ActionsClass();
                this.Scheduler = new SchedulerClass();
                this.Alarms.Stopwatch = new System.Diagnostics.Stopwatch();
                this.Actions.Stopwatch = new System.Diagnostics.Stopwatch();
                this.Scheduler.Stopwatch = new System.Diagnostics.Stopwatch();
            }

            internal AlarmsClass Alarms { get; set; }
            internal ActionsClass Actions { get; set; }
            internal SchedulerClass Scheduler { get; set; }

            internal class AlarmsClass
            {
                private bool _Status { get; set; }
                internal bool Status
                {
                    get { return this._Status; }
                    set
                    {
                        if (value)
                        {
                            this.Stopwatch.Reset();
                            this.Stopwatch.Start();
                            this._Status = true;
                        }
                        else
                        {
                            this.Stopwatch.Stop();
                            this._Status = false;
                        }
                    }
                }
                internal System.Diagnostics.Stopwatch Stopwatch { get; set; }
            }
            internal class ActionsClass
            {
                private bool _Status { get; set; }
                internal bool Status
                {
                    get { return this._Status; }
                    set
                    {
                        if (value)
                        {
                            this.Stopwatch.Reset();
                            this.Stopwatch.Start();
                            this._Status = true;
                        }
                        else
                        {
                            this.Stopwatch.Stop();
                            this._Status = false;
                        }
                    }
                }
                internal System.Diagnostics.Stopwatch Stopwatch { get; set; }
            }
            internal class SchedulerClass
            {
                private bool _Status { get; set; }
                internal bool Status
                {
                    get { return this._Status; }
                    set
                    {
                        if (value)
                        {
                            this.Stopwatch.Reset();
                            this.Stopwatch.Start();
                            this._Status = true;
                        }
                        else
                        {
                            this.Stopwatch.Stop();
                            this._Status = false;
                        }
                    }
                }
                internal System.Diagnostics.Stopwatch Stopwatch { get; set; }
            }
        }
    }
}
