using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KarelazisBot.Objects
{
    public partial class CreatureData
    {
        public class Damage
        {
            public Damage(AttackTypes attackType, DamageTypes damageType, ushort minDamage, ushort maxDamage)
            {
                this.AttackType = attackType;
                this.DamageType = damageType;
                this.MinDamage = Math.Min(minDamage, maxDamage);
                this.MaxDamage = Math.Max(minDamage, maxDamage);
            }

            public AttackTypes AttackType { get; private set; }
            public DamageTypes DamageType { get; private set; }
            public ushort MinDamage { get; private set; }
            public ushort MaxDamage { get; private set; }
        }
    }
}
