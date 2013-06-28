using System; // access to basic stuff in .NET
using System.Collections.Generic; // access to things like List<T> (T = value type) in .NET
using System.Linq; // necessary to cast IEnumerables to i.e. a list or array
using System.Threading; // used for putting the thread to sleep
using KarelazisBot; // base access to enumerators, Windows API and such
using KarelazisBot.Objects; // access to all the objects

public class Test
{
    struct InvisibleCreature
    {
        public InvisibleCreature(string name, byte outfitType)
        {
            this.Name = name;
            this.OutfitType = outfitType;
        }

        public string Name;
        public byte OutfitType;
    }

    public static void Main(Client client)
    {
        InvisibleCreature[] creatures =
        {
            new InvisibleCreature("Stalker", 128),
            new InvisibleCreature("Warlock", 130)
        };

        while (true)
        {
            Thread.Sleep(500);

            foreach (Creature c in client.BattleList.GetCreatures(true,  true))
            {
                foreach (var ic in creatures)
                {
                    if (ic.OutfitType == c.OutfitType) continue;
                    if (ic.Name != c.Name) continue;
                    c.OutfitType = ic.OutfitType;
                }
            }
        }
    }
}
