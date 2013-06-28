using System;
using System.Collections.Generic;

namespace KarelazisBot.Objects
{
    /// <summary>
    /// A class that acts somewhat like the in-game battlelist, contains information about nearby creatures, players and NPCs.
    /// </summary>
    public class BattleList
    {
        /// <summary>
        /// Constructor for this class.
        /// </summary>
        /// <param name="c">The client to be associated with this object.</param>
        public BattleList(Objects.Client c)
        {
            this.Client = c;
            this.CachedCreatures = new Objects.Creature[this.Client.Addresses.BattleList.MaxCreatures];
            for (int i = 0; i < this.CachedCreatures.Length; i++)
            {
                this.CachedCreatures[i] = new Objects.Creature(this.Client,
                    this.Client.Addresses.BattleList.Start + i * this.Client.Addresses.BattleList.Step);
            }
            this.CachedCount = 0;
        }

        /// <summary>
        /// Gets the client associated with this object.
        /// </summary>
        public Objects.Client Client { get; private set; }
        private Objects.Creature[] CachedCreatures { get; set; }
        /// <summary>
        /// Gets or sets the current count of cached creatures in memory.
        /// Useful to avoid unneeded winapi calls.
        /// </summary>
        private int CachedCount { get; set; }

        /// <summary>
        /// Gets a collection of creatures.
        /// </summary>
        /// <param name="visibleOnly">Whether to only include visible creatures.</param>
        /// <param name="sameFloorOnly">Whether to only include creatures on the same floor as the player.</param>
        /// <returns></returns>
        public IEnumerable<Objects.Creature> GetCreatures(bool visibleOnly = true, bool sameFloorOnly = true)
        {
            byte z = this.Client.Player.Z;
            int index = 0;
            foreach (Objects.Creature creature in this.CachedCreatures)
            {
                if (index >= this.CachedCount)
                {
                    if (creature.ID == 0) break;
                    this.CachedCount = index;
                }
                index++;

                if (creature.Type != Creature.Types.Creature) continue;
                if (visibleOnly && !creature.IsVisible) continue;
                if (sameFloorOnly && creature.Z != z) continue;

                yield return creature;
            }
        }
        /// <summary>
        /// Gets a collection of players.
        /// </summary>
        /// <param name="visibleOnly">Whether to only include visible players.</param>
        /// <param name="sameFloorOnly">Whether to only include players on the same floor as the player.</param>
        /// <returns></returns>
        public IEnumerable<Objects.Creature> GetPlayers(bool visibleOnly = true, bool sameFloorOnly = true)
        {
            byte z = this.Client.Player.Z;
            int index = 0;
            foreach (Objects.Creature creature in this.CachedCreatures)
            {
                if (index >= this.CachedCount)
                {
                    if (creature.ID == 0) break;
                    this.CachedCount = index;
                }
                index++;

                if (creature.Type != Creature.Types.Player) continue;
                if (visibleOnly && !creature.IsVisible) continue;
                if (sameFloorOnly && creature.Z != z) continue;

                yield return creature;
            }
        }
        /// <summary>
        /// Gets all creatures, players and NPCs.
        /// </summary>
        /// <param name="visibleOnly">Whether to only include visible entities.</param>
        /// <param name="sameFloorOnly">Whether to only include entities that are on the same floor as the player.</param>
        /// <returns></returns>
        public IEnumerable<Objects.Creature> GetAll(bool visibleOnly = true, bool sameFloorOnly = true)
        {
            byte z = this.Client.Player.Z;
            int index = 0;
            foreach (Objects.Creature creature in this.CachedCreatures)
            {
                if (index >= this.CachedCount)
                {
                    if (creature.ID == 0) break;
                    this.CachedCount = index;
                }
                index++;

                if (visibleOnly && !creature.IsVisible) continue;
                if (sameFloorOnly && creature.Z != z) continue;

                yield return creature;
            }
        }
        /// <summary>
        /// Gets a creature using a given ID. Returns null if unsuccessful.
        /// </summary>
        /// <param name="id">The ID of the creature to find.</param>
        /// <returns></returns>
        public Objects.Creature GetCreature(uint id)
        {
            int index = 0;
            foreach (Objects.Creature creature in this.CachedCreatures)
            {
                if (index >= this.CachedCount)
                {
                    if (creature.ID == 0) break;
                    this.CachedCount = index;
                }
                index++;

                if (creature.Type != Creature.Types.Creature) continue;
                if (creature.ID == id) return creature;
            }
            return null;
        }
        /// <summary>
        /// Gets a player using a given ID. Returns null if unsuccessful.
        /// </summary>
        /// <param name="id">The ID of the player to find.</param>
        /// <returns></returns>
        public Objects.Creature GetPlayer(uint id)
        {
            int index = 0;
            foreach (Objects.Creature creature in this.CachedCreatures)
            {
                if (index >= this.CachedCount)
                {
                    if (creature.ID == 0) break;
                    this.CachedCount = index;
                }
                index++;

                if (creature.Type != Creature.Types.Player) continue;
                if (creature.ID == id) return creature;
            }
            return null;
        }
        /// <summary>
        /// Gets a player using a given name. Case sensitive. Returns null if unsuccessful.
        /// </summary>
        /// <param name="name">The name of the player to find.</param>
        /// <returns></returns>
        public Objects.Creature GetPlayer(string name)
        {
            int index = 0;
            foreach (Objects.Creature creature in this.CachedCreatures)
            {
                if (index >= this.CachedCount)
                {
                    if (creature.ID == 0) break;
                    this.CachedCount = index;
                }
                index++;

                if (creature.Type != Creature.Types.Player) continue;
                if (creature.Name == name) return creature;
            }
            return null;
        }
        /// <summary>
        /// Gets any entity based on its ID. Returns null if unsuccessful.
        /// </summary>
        /// <param name="id">The ID of the entity to find.</param>
        /// <returns></returns>
        public Objects.Creature GetAny(uint id)
        {
            int index = 0;
            foreach (Objects.Creature creature in this.CachedCreatures)
            {
                if (index >= this.CachedCount)
                {
                    if (creature.ID == 0) break;
                    this.CachedCount = index;
                }
                index++;

                if (creature.ID == id) return creature;
            }
            return null;
        }
    }
}
