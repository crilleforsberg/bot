using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KarelazisBot.Objects
{
    public partial class CreatureData
    {
        #region constructors
        public CreatureData(string name, ushort hitPoints, uint experiencePoints, ushort summonMana, ushort convinceMana,
            bool canSeeInvisible, ushort speed, AbilityTypes abilities, DamageTypes immunities, IEnumerable<Damage> damages,
            IEnumerable<CreatureData> minions, Dictionary<Loot, Loot.Chance> loot)
        {
            this.Name = name;
            this.HitPoints = hitPoints;
            this.ExperiencePoints = experiencePoints;
            this.SummonMana = summonMana;
            this.ConvinceMana = convinceMana;
            this.CanSeeInvisible = canSeeInvisible;
            this.Speed = speed;
            this.Abilities = abilities;
            this.Immunities = immunities;
            this.Damages = damages.ToList();
            this.Minions = minions.ToList();
            this.Loots = loot;
        }
        #endregion

        #region properties
        public string Name { get; private set; }
        public ushort HitPoints { get; set; }
        public uint ExperiencePoints { get; set; }
        public ushort SummonMana { get; set; }
        public ushort ConvinceMana { get; set; }
        public bool CanSeeInvisible { get; set; }
        public ushort Speed { get; set; }
        public AbilityTypes Abilities { get; set; }
        public DamageTypes Immunities { get; set; }

        private List<CreatureData> Minions { get; set; }
        private List<Damage> Damages { get; set; }
        private Dictionary<Loot, Loot.Chance> Loots { get; set; }
        #endregion

        public override string ToString()
        {
            return this.Name;
        }

        public IEnumerable<Damage> GetDamages()
        {
            return this.Damages.ToArray();
        }
        public IEnumerable<CreatureData> GetMinions()
        {
            return this.Minions.ToArray();
        }
        public IEnumerable<KeyValuePair<Loot, Loot.Chance>> GetLoot()
        {
            return this.Loots.ToArray();
        }

        #region enums
        [Flags]
        public enum AbilityTypes
        {
            None = 1,
            Paralysis = 1 << 1,
            Haste = 1 << 2,
            StrongHaste = 1 << 3,
            CanPushObjects = 1 << 4
        }
        [Flags]
        public enum DamageTypes
        {
            None = 1,
            Physical = 1 << 1,
            Poison = 1 << 2,
            Fire = 1 << 3,
            Energy = 1 << 4,
            LifeDrain = 1 << 5,
            ManaDrain = 1 << 6
        }
        [Flags]
        public enum AttackTypes
        {
            Melee = 1,
            Wave = 1 << 1,
            Beam = 1 << 2,
            Missile = 1 << 3,
            AreaOfEffect = 1 << 4,
            Self = 1 << 5
        }
        [Flags]
        public enum CombatBehaviourTypes
        {
            KeepAway = 1,
            Follow = 1 << 1,
            RunWhenLowHealth = 1 << 2
        }
        #endregion
    }
}
