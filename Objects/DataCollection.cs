using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KarelazisBot.Objects
{
    public class DataCollection
    {
        public DataCollection(float experienceRate = 1u)
        {
            this.Creatures = new List<CreatureData>();
            this.Loot = new List<CreatureData.Loot>();
            this.Foods = new List<ItemData.Food>();

            #region food
            this.Foods.Add(new ItemData.Food("Fish", 2667, 5.2f, true, 144));
            this.Foods.Add(new ItemData.Food("Meat", 2666, 13, true, 180));
            this.Foods.Add(new ItemData.Food("Brown Mushroom", 2789, 0.20f, true, 264));
            this.Foods.Add(new ItemData.Food("White Mushroom", 2787, 0.40f, true, 108));
            this.Foods.Add(new ItemData.Food("Ham", 2671, 20, true, 360));
            this.Foods.Add(new ItemData.Food("Bread", 2689, 5, true, 120));
            this.Foods.Add(new ItemData.Food("Brown Bread", 3602, 4, true, 96));
            this.Foods.Add(new ItemData.Food("Cheese", 2696, 4, false, 108));
            this.Foods.Add(new ItemData.Food("Dragon Ham", 2672, 30, true, 720));
            this.Foods.Add(new ItemData.Food("Egg", 2685, 0.30f, true, 72));
            #endregion

            #region loot
            this.Loot.Add(new CreatureData.Loot("Gold Coin", 2148, true, 1));
            this.Loot.Add(new CreatureData.Loot("Platinum Coin", 2149, true, 100));
            this.Loot.Add(new CreatureData.Loot("Crystal Coin", 2150, true, 10000));
            this.Loot.Add(new CreatureData.Loot("Black Pearl", 2144, true, 280));
            this.Loot.Add(new CreatureData.Loot("Scarab Coin", 2159, true, 0));
            this.Loot.Add(new CreatureData.Loot("Small Amethyst", 2150, true, 200));
            #endregion

            #region creatures
            this.Creatures.Add(new CreatureData("Amazon", 110, (uint)(60 * experienceRate), 390, 390, true, 0,
                CreatureData.AbilityTypes.Haste | CreatureData.AbilityTypes.Paralysis | CreatureData.AbilityTypes.CanPushObjects,
                CreatureData.DamageTypes.Poison, new CreatureData.Damage[]
                {
                    new CreatureData.Damage(CreatureData.AttackTypes.Melee, CreatureData.DamageTypes.Physical, 0, 45),
                    new CreatureData.Damage(CreatureData.AttackTypes.Missile, CreatureData.DamageTypes.Poison, 15, 145),
                    new CreatureData.Damage(CreatureData.AttackTypes.AreaOfEffect, CreatureData.DamageTypes.Poison, 22, 26)
                },
                Enumerable.Empty<CreatureData>(),
                new Dictionary<CreatureData.Loot, CreatureData.Loot.Chance>
                {
                    
                }));

            this.Creatures.Add(new CreatureData("Ancient Scarab", 1000, (uint)(720 * experienceRate), 0, 0, false, 0,
                CreatureData.AbilityTypes.None, CreatureData.DamageTypes.None, new CreatureData.Damage[]
                {
                    new CreatureData.Damage(CreatureData.AttackTypes.Melee, CreatureData.DamageTypes.Physical, 0, 210),
                    new CreatureData.Damage(CreatureData.AttackTypes.Missile, CreatureData.DamageTypes.Physical, 0, 40)
                },
                Enumerable.Empty<CreatureData>(),
                new Dictionary<CreatureData.Loot, CreatureData.Loot.Chance>
                {
                    
                }));
            this.Creatures.Add(new CreatureData("Rat", 20, (uint)(5 * experienceRate), 200, 200, false, 0, CreatureData.AbilityTypes.None,
                CreatureData.DamageTypes.None, new CreatureData.Damage[]
                {
                    new CreatureData.Damage(CreatureData.AttackTypes.Melee, CreatureData.DamageTypes.Physical, 0, 8)
                },
                Enumerable.Empty<CreatureData>(),
                new Dictionary<CreatureData.Loot, CreatureData.Loot.Chance>
                {
                    
                }));
            this.Creatures.Add(new CreatureData("Cave Rat", 30, (uint)(10 * experienceRate), 250, 250, false, 0, CreatureData.AbilityTypes.None,
                CreatureData.DamageTypes.None, new CreatureData.Damage[]
                {
                    new CreatureData.Damage(CreatureData.AttackTypes.Melee, CreatureData.DamageTypes.Physical, 0, 10)
                },
                Enumerable.Empty<CreatureData>(),
                new Dictionary<CreatureData.Loot, CreatureData.Loot.Chance>
                {
                    
                }));
            this.Creatures.Add(new CreatureData("Bug", 29, (uint)(18 * experienceRate), 250, 250, false, 0,
                CreatureData.AbilityTypes.None, CreatureData.DamageTypes.None, new CreatureData.Damage[]
                {
                    new CreatureData.Damage(CreatureData.AttackTypes.Melee, CreatureData.DamageTypes.Physical, 0, 23)
                },
                Enumerable.Empty<CreatureData>(),
                new Dictionary<CreatureData.Loot, CreatureData.Loot.Chance>
                {
                    
                }));
            this.Creatures.Add(new CreatureData("Bug", 29, (uint)(18 * experienceRate), 250, 250, false, 0,
                CreatureData.AbilityTypes.None, CreatureData.DamageTypes.None, new CreatureData.Damage[]
                {
                    new CreatureData.Damage(CreatureData.AttackTypes.Melee, CreatureData.DamageTypes.Physical, 0, 23)
                },
                Enumerable.Empty<CreatureData>(),
                new Dictionary<CreatureData.Loot, CreatureData.Loot.Chance>
                {
                    
                }));
            #endregion
        }

        private List<CreatureData> Creatures { get; set; }
        private List<CreatureData.Loot> Loot { get; set; }
        private List<ItemData.Food> Foods { get; set; }
    }
}
