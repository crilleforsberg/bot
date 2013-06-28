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
            new InvisibleCreature("Dworc Voodoomaster", 128),
            new InvisibleCreature("Warlock", 130)
        };
        ushort runeID = 3174;

        while (true)
        {
            Thread.Sleep(500);

            Item rune = client.Inventory.GetItem(runeID);
            if (rune == null) continue;

            Location playerLoc = client.Player.Location;
            foreach (Creature c in client.BattleList.GetCreatures())
            {
                bool found = false;
                if (c.OutfitType != 0) continue;
                if (playerLoc.Z <= 7 && !playerLoc.IsOnScreen(c.Location)) continue;
                else if (playerLoc.Z > 7 && playerLoc.DistanceTo(c.Location) >= client.Player.Light) continue;
                foreach (var ic in creatures)
                {
                    //if (ic.OutfitType != 0) continue;//== c.OutfitType) continue;
                    if (ic.Name != c.Name) continue;
                    rune.UseOnCreature(c);
                    found = true;
                    Thread.Sleep(2000);
                    break;
                }
                if (found) break;
            }
        }
    }
}
