using System.Collections.Generic;

namespace KarelazisBot.Objects
{
    public class ItemList
    {
        public ItemList()
        {
            this.InitializeClasses();
        }

        public void InitializeClasses()
        {
            this.Containers = new ContainersClass();
            this.Amulets = new AmuletsClass();
            this.Food = new FoodClass();
            this.Rings = new RingsClass();
            this.Runes = new RunesClass();
            this.Tools = new ToolsClass();
            this.Valuables = new ValuablesClass();
            this.DepotLockers = new List<ushort>()
            {

            };
        }

        public ContainersClass Containers { get; set; }
        public AmuletsClass Amulets { get; set; }
        public FoodClass Food { get; set; }
        public RingsClass Rings { get; set; }
        public RunesClass Runes { get; set; }
        public ToolsClass Tools { get; set; }
        public ValuablesClass Valuables { get; set; }

        public List<ushort> DepotLockers { get; set; }
        public class ContainersClass
        {
            public ushort Bag = 1987;//2853;
            public List<ushort> All
            {
                get { return new List<ushort>() { this.Bag }; }
            }
        }
        public class FoodClass
        {
            public ushort Fish = 2667,//3578,//2667,
                            Meat = 2666,//3577,//2666,
                            BrownMushroom = 2789,
                            WhiteMushroom = 2787,
                            Ham = 2671,
                            Bread = 2689,
                            BrownBread = 3602,
                            Cheese = 2696,
                            DragonHam = 2672,
                            Egg = 2685;
            public List<ushort> All
            {
                get { return new List<ushort>() { this.Fish, this.Meat, this.BrownMushroom, this.WhiteMushroom, this.Ham, this.Bread, this.Cheese, this.DragonHam, this.Egg }; }
            }
        }
        public class ToolsClass
        {
            public ushort Rope = 2120,//3003,//2120,
                            Shovel = 2554,//3457,//2554,
                            Pick = 2553,//3456,//2553,
                            Machete = 2420,//3308,//2420,
                            Pot = 2562,
                            FishingRod = 2580;//3483;//2580;
            public List<ushort> All
            {
                get { return new List<ushort>() { this.Rope, this.Shovel, this.Pick, this.Machete, this.Pot, this.FishingRod }; }
            }
        }
        public class RunesClass
        {
            public ushort SuddenDeath = 2268,//3155,//3150,
                            Explosion = 2313,
                            GreatFireball = 2304,//3191,//2304,
                            Fireball = 2302,
                            Soulfire = 2308,
                            HeavyMagicMissile = 2311,//3198,//2311,
                            UltimateHealing = 2273,
                            IntenseHealing = 2265,
                            Blank = 2260,//3147,//2260,
                            DestroyField = 2261,
                            MagicWall = 2293,//3180,//2293,
                            FireBomb = 2305,
                            EnergyBomb = 2262,
                            LightMagicMissile = 2287,
                            PoisonBomb = 2286,
                            Paralyze = 2278,
                            Vial = 2006,//2874,//2006;
                            ManaPotion = 268,
                            StrongManaPotion = 237,
                            GreatManaPotion = 238,
                            HealthPotion = 266,
                            StrongHealthPotion = 236,
                            GreatHealthPotion = 239,
                            GreatSpiritPotion = 7642,
                            UltimateHealthPotion = 7643;

            public List<ushort> All
            {
                get
                {
                    return new List<ushort>() { SuddenDeath, Explosion, GreatFireball, Fireball, Soulfire, HeavyMagicMissile,
                        UltimateHealing, IntenseHealing, Blank, DestroyField, MagicWall, FireBomb,
                        EnergyBomb, LightMagicMissile, PoisonBomb, Paralyze, Vial };
                }
            }
        }
        public class ValuablesClass
        {
            public ushort GoldCoin = 2148,//3031,//2148,
                            PlatinumCoin = 2152,//3035,//2152,
                            SmallAmethyst = 2150;

            public List<ushort> All
            {
                get
                {
                    return new List<ushort>() { GoldCoin, PlatinumCoin, SmallAmethyst };
                }
            }
        }
        public class RingsClass
        {
            public ushort Axe = 2208,//3092,//2208,
                            Dwarven = 2213,//3097,//2213,
                            Energy = 2167,//3051,//2167,
                            Life = 2168,//3052,//2168,
                            Might = 2164,//3048,//2164,
                            Healing = 2214,//3098,//2214,
                            Stealth = 2165,//3049,//2165,
                            Sword = 2207,//3091,//2207,
                            Time = 2169,//3053,//2169,
                            Club = 3093;//0;

            public List<ushort> All
            {
                get
                {
                    return new List<ushort>() { Axe, Dwarven, Energy, Life, Might, Healing,
                        Stealth, Sword, Time };
                }
            }
        }
        public class AmuletsClass
        {
            public ushort Elven = 2198,
                            Stoneskin = 2197,
                            Platinum = 2171;

            public List<ushort> All
            {
                get
                {
                    return new List<ushort>() { Elven, Stoneskin, Platinum };
                }
            }
        }
    }
}
