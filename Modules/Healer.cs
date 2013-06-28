using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace KarelazisBot.Modules
{
    /// <summary>
    /// A class that handles simple healing.
    /// </summary>
    public class Healer
    {
        /// <summary>
        /// A collection class that holds values for health, mana and action.
        /// </summary>
        public class Criteria
        {
            /// <summary>
            /// Constructor for a potion or rune criteria.
            /// How the given values are used depends on the action.
            /// </summary>
            /// <param name="health">At what health to use the potion or rune.</param>
            /// <param name="mana">At what mana to use the potion or rune.</param>
            /// <param name="action">The action to perform.</param>
            public Criteria(ushort health, ushort mana, Healer.ActionType action)
            {
                this.Health = health;
                this.Mana = mana;
                this.Action = action;
            }
            /// <summary>
            /// Constructor for a spell criteria.
            /// </summary>
            /// <param name="health">At what health to cast the spell.</param>
            /// <param name="mana">Required mana to cast the spell.</param>
            /// <param name="spell">The spell to cast.</param>
            public Criteria(ushort health, ushort mana, string spell)
            {
                this.Health = health;
                this.Mana = mana;
                this.Action = ActionType.Say;
                this.Spell = spell;
            }

            /// <summary>
            /// At what health to perform the action.
            /// </summary>
            public ushort Health { get; set; }
            /// <summary>
            /// At what mana to perform the action.
            /// </summary>
            public ushort Mana { get; set; }
            /// <summary>
            /// The action to perform.
            /// </summary>
            public Healer.ActionType Action { get; set; }
            /// <summary>
            /// The spell to cast.
            /// </summary>
            public string Spell { get; set; }
        }

        /// <summary>
        /// Constructor for this class.
        /// </summary>
        /// <param name="c">The client to be associated with this object.</param>
        /// <param name="checkInterval">How often to check the player's health and mana in milliseconds.</param>
        /// <param name="sleepMin">Minimum amount of milliseconds to sleep once an action is performed.</param>
        /// <param name="sleepMax">Maximum amount of milliseconds to sleep once an action is performed.</param>
        public Healer(Objects.Client c, ushort checkInterval, ushort sleepMin, ushort sleepMax)
        {
            this.Client = c;
            this.Criterias = new List<Criteria>();
            this.CheckInterval = checkInterval;
            this.SleepMin = sleepMin;
            this.SleepMax = sleepMax;
            this.rand = new Random();
        }

        /// <summary>
        /// Gets the client associated with this object.
        /// </summary>
        public Objects.Client Client { get; private set; }
        /// <summary>
        /// Gets whether the healer is running or not.
        /// </summary>
        public bool IsRunning { get; private set; }
        /// <summary>
        /// Gets or sets how often the player's health and mana values are checked in milliseconds.
        /// </summary>
        public ushort CheckInterval { get; set; }
        /// <summary>
        /// Gets or sets the minimum amount of time in milliseconds to sleep once an action is performed.
        /// </summary>
        public ushort SleepMin { get; set; }
        /// <summary>
        /// Gets or sets the maximum amount of time in milliseconds to sleep once an action is performed.
        /// </summary>
        public ushort SleepMax { get; set; }
        /// <summary>
        /// Gets or sets the list of criterias.
        /// </summary>
        private List<Criteria> Criterias { get; set; }
        private Random rand { get; set; }

        public enum ActionType
        {
            ManaFluid,
            ManaPotion,
            StrongManaPotion,
            GreatManaPotion,
            HealthPotion,
            StrongHealthPotion,
            GreatHealthPotion,
            UltimateHealthPotion,
            UltimateHealingRune,
            IntenseHealingRune,
            GreatSpiritPotion,
            Say
        }

        #region events
        public delegate void CriteriaAddedHandler(Criteria c);
        /// <summary>
        /// An event that gets fired when a criteria is added.
        /// </summary>
        public event CriteriaAddedHandler CriteriaAdded;
        public delegate void CriteriaInsertedHandler(Criteria c, int index);
        /// <summary>
        /// An event that gets fired when a criteria is inserted at a given index.
        /// </summary>
        public event CriteriaInsertedHandler CriteriaInserted;
        public delegate void CriteriaRemovedHandler(Criteria c, int index);
        /// <summary>
        /// An event that gets fired when a criteria is removed.
        /// </summary>
        public event CriteriaRemovedHandler CriteriaRemoved;
        public delegate void StatusChangedHandler(bool status);
        /// <summary>
        /// An event that gets fired when IsRunning is changed.
        /// </summary>
        public event StatusChangedHandler StatusChanged;
        #endregion

        /// <summary>
        /// Starts the healer.
        /// </summary>
        public void Start()
        {
            if (this.IsRunning) return;
            this.IsRunning = true;
            Thread t = new Thread(new ThreadStart(this.HealerLogic));
            t.Start();
        }
        /// <summary>
        /// Stops the healer.
        /// </summary>
        public void Stop()
        {
            if (!this.IsRunning) return;
            this.IsRunning = false;
        }
        /// <summary>
        /// Gets an array of healer criterias.
        /// </summary>
        /// <returns></returns>
        public IEnumerable<Criteria> GetCriterias()
        {
            return this.Criterias;
        }
        public void AddCriteria(Criteria c)
        {
            if (c == null) return;
            this.Criterias.Add(c);
            if (this.CriteriaAdded != null) this.CriteriaAdded(c);
        }
        /// <summary>
        /// Adds a range of criterias.
        /// </summary>
        /// <param name="criterias">A collection of criterias.</param>
        public void AddCriterias(IEnumerable<Criteria> criterias)
        {
            foreach (Criteria c in criterias) this.AddCriteria(c);
        }
        /// <summary>
        /// Removes a given criteria.
        /// </summary>
        /// <param name="c">The criteria to remove.</param>
        /// <returns></returns>
        public bool RemoveCriteria(Criteria c)
        {
            int index = 0;
            foreach (Criteria criteria in this.GetCriterias())
            {
                if (criteria != c)
                {
                    index++;
                    continue;
                }
                if (!this.Criterias.Remove(c)) return false;
                if (this.CriteriaRemoved != null) this.CriteriaRemoved(c, index);
                return true;
            }
            return false;
        }
        /// <summary>
        /// Removes all criterias.
        /// </summary>
        public void RemoveAllCriterias()
        {
            foreach (Criteria c in this.GetCriterias()) this.RemoveCriteria(c);
        }
        /// <summary>
        /// Inserts a given criteria.
        /// </summary>
        /// <param name="c">The criteria to insert.</param>
        /// <param name="index">The index to insert at.</param>
        public void InsertCriteria(Criteria c, int index)
        {
            if (c == null) return;
            if (index >= this.Criterias.Count || index < this.Criterias.Count) throw new IndexOutOfRangeException();
            this.Criterias.Insert(index, c);
            if (this.CriteriaInserted != null) this.CriteriaInserted(c, index);
        }
        /// <summary>
        /// Moves a criteria to a new index.
        /// </summary>
        /// <param name="c">The criteria to move.</param>
        /// <param name="toIndex">The index to move the criteria to.</param>
        public void MoveCriteria(Criteria c, int toIndex)
        {
            if (c == null) return;
            if (toIndex >= this.Criterias.Count || toIndex < this.Criterias.Count) throw new IndexOutOfRangeException();
            this.RemoveCriteria(c);
            this.InsertCriteria(c, toIndex);
        }
        /// <summary>
        /// Moves a criteria to a new index.
        /// </summary>
        /// <param name="fromIndex">The index to move a criteria from.</param>
        /// <param name="toIndex">The index to move a criteria to.</param>
        public void MoveCriteria(int fromIndex, int toIndex)
        {
            if (fromIndex >= this.Criterias.Count || fromIndex < this.Criterias.Count ||
                toIndex >= this.Criterias.Count || toIndex < this.Criterias.Count)
            {
                throw new IndexOutOfRangeException();
            }
            Criteria c = this.Criterias[fromIndex];
            this.MoveCriteria(c, toIndex);
        }

        /// <summary>
        /// Handles checking the player's values against the loaded criterias, and executes an action if a criteria is met.
        /// Run this method on a seperate thread.
        /// </summary>
        private void HealerLogic()
        {
            try
            {
                if (this.StatusChanged != null) this.StatusChanged(this.IsRunning);
                while (this.IsRunning)
                {
                    Thread.Sleep(this.CheckInterval);
                    if (!this.Client.Player.Connected) continue;
                    ushort health = this.Client.Player.Health,
                        mana = this.Client.Player.Mana;
                    foreach (Criteria c in this.GetCriterias())
                    {
                        if (c.Mana <= mana)
                        {
                            switch (c.Action)
                            {
                                case ActionType.ManaFluid:
                                    if (this.Client.Player.IsWalking) break;
                                    Objects.Item manafluid = this.Client.Inventory.GetItem(this.Client.ItemList.Runes.Vial, 7);
                                    if (manafluid == null) break;
                                    manafluid.UseOnSelf();
                                    Thread.Sleep(rand.Next(Math.Min(this.SleepMin, this.SleepMax),
                                                           Math.Max(this.SleepMin, this.SleepMax)));
                                    break;
                                case ActionType.ManaPotion:
                                    this.Client.Packets.HotkeyUseItemOnSelf(this.Client.ItemList.Runes.ManaPotion);
                                    Thread.Sleep(rand.Next(Math.Min(this.SleepMin, this.SleepMax),
                                                           Math.Max(this.SleepMin, this.SleepMax)));
                                    break;
                                case ActionType.StrongManaPotion:
                                    this.Client.Packets.HotkeyUseItemOnSelf(this.Client.ItemList.Runes.StrongManaPotion);
                                    Thread.Sleep(rand.Next(Math.Min(this.SleepMin, this.SleepMax),
                                                           Math.Max(this.SleepMin, this.SleepMax)));
                                    break;
                                case ActionType.GreatManaPotion:
                                    this.Client.Packets.HotkeyUseItemOnSelf(this.Client.ItemList.Runes.GreatManaPotion);
                                    Thread.Sleep(rand.Next(Math.Min(this.SleepMin, this.SleepMax),
                                                           Math.Max(this.SleepMin, this.SleepMax)));
                                    break;
                                case ActionType.GreatSpiritPotion:
                                    this.Client.Packets.HotkeyUseItemOnSelf(this.Client.ItemList.Runes.GreatSpiritPotion);
                                    Thread.Sleep(rand.Next(Math.Min(this.SleepMin, this.SleepMax),
                                                           Math.Max(this.SleepMin, this.SleepMax)));
                                    break;
                            }
                        }
                        if (health < c.Health && mana >= c.Mana)
                        {
                            Objects.Item item = null;
                            switch (c.Action)
                            {
                                case ActionType.UltimateHealingRune:
                                    item = this.Client.Inventory.GetItem(this.Client.ItemList.Runes.UltimateHealing);
                                    if (item != null)
                                    {
                                        item.UseOnSelf();
                                        Thread.Sleep(rand.Next(Math.Min(this.SleepMin, this.SleepMax),
                                            Math.Max(this.SleepMin, this.SleepMax)));
                                    }
                                    break;
                                case ActionType.IntenseHealingRune:
                                    item = this.Client.Inventory.GetItem(this.Client.ItemList.Runes.IntenseHealing);
                                    if (item != null)
                                    {
                                        item.UseOnSelf();
                                        Thread.Sleep(rand.Next(Math.Min(this.SleepMin, this.SleepMax),
                                            Math.Max(this.SleepMin, this.SleepMax)));
                                    }
                                    break;
                                case ActionType.HealthPotion:
                                    this.Client.Packets.HotkeyUseItemOnSelf(this.Client.ItemList.Runes.HealthPotion);
                                    Thread.Sleep(rand.Next(Math.Min(this.SleepMin, this.SleepMax),
                                                           Math.Max(this.SleepMin, this.SleepMax)));
                                    break;
                                case ActionType.StrongHealthPotion:
                                    this.Client.Packets.HotkeyUseItemOnSelf(this.Client.ItemList.Runes.StrongHealthPotion);
                                    Thread.Sleep(rand.Next(Math.Min(this.SleepMin, this.SleepMax),
                                                           Math.Max(this.SleepMin, this.SleepMax)));
                                    break;
                                case ActionType.GreatHealthPotion:
                                    this.Client.Packets.HotkeyUseItemOnSelf(this.Client.ItemList.Runes.GreatHealthPotion);
                                    Thread.Sleep(rand.Next(Math.Min(this.SleepMin, this.SleepMax),
                                                           Math.Max(this.SleepMin, this.SleepMax)));
                                    break;
                                case ActionType.UltimateHealthPotion:
                                    this.Client.Packets.HotkeyUseItemOnSelf(this.Client.ItemList.Runes.UltimateHealthPotion);
                                    Thread.Sleep(rand.Next(Math.Min(this.SleepMin, this.SleepMax),
                                                           Math.Max(this.SleepMin, this.SleepMax)));
                                    break;
                                case ActionType.GreatSpiritPotion:
                                    this.Client.Packets.HotkeyUseItemOnSelf(this.Client.ItemList.Runes.GreatSpiritPotion);
                                    Thread.Sleep(rand.Next(Math.Min(this.SleepMin, this.SleepMax),
                                                           Math.Max(this.SleepMin, this.SleepMax)));
                                    break;
                                case ActionType.Say:
                                    if (string.IsNullOrEmpty(c.Spell)) break;
                                    this.Client.Packets.Say(c.Spell);
                                    Thread.Sleep(rand.Next(Math.Min(this.SleepMin, this.SleepMax),
                                                           Math.Max(this.SleepMin, this.SleepMax)));
                                    break;
                            }
                            break;
                        }
                    }
                }
            }
            catch { }
            finally
            {
                this.IsRunning = false;
                if (this.StatusChanged != null) this.StatusChanged(this.IsRunning);
            }
        }
    }
}
