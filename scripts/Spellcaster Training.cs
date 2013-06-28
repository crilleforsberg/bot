using System; // access to basic stuff in .NET
using System.Collections.Generic; // access to things like List<T> (T = value type) in .NET
using System.Linq; // necessary to cast IEnumerables to i.e. a list or array
using System.Threading; // used for putting the thread to sleep
using KarelazisBot; // base access to enumerators, Windows API and such
using KarelazisBot.Objects; // access to all the objects

public class Test
{
    public static void Main(Client client)
    {
        string spellName = "utevo gran lux";
        ushort spellMana = 60;
        float manaPerSecond = 1f / 6f;
        Random rand = new Random();
        while (true)
        {
            ushort manaLeft = (ushort)(client.Player.ManaMax - client.Player.Mana);
            if (manaLeft > 0)
            {
                float timeToSleep = (float)Math.Floor(manaLeft / manaPerSecond);
                Thread.Sleep(rand.Next((int)((timeToSleep / 2) * 1000), (int)(timeToSleep * 1000)));
            }

            if (client.Player.Mana < spellMana || client.Player.ManaPercent < 80) continue;
            int timesToCast = client.Player.Mana / spellMana;
            for (int i = 0; i < timesToCast; i++)
            {
                client.Packets.Say(spellName);
                Thread.Sleep(rand.Next(1000, 2000));
            }
        }
    }
}
