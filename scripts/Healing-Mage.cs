using System; // access to basic stuff in .NET
using System.Collections.Generic; // access to things like List<T> (T = value type) in .NET
using System.Threading;
using System.Linq;
using KarelazisBot; // base access to enumerators, Windows API and such
using KarelazisBot.Objects; // access to all the objects

public class Test
{
    public static void Main(Client client)
    {
        Random rand = new Random();
        while (true)
        {
            Thread.Sleep(rand.Next(800, 1600));

            if (!client.Player.Connected) continue;
            
            // check if cavebot is running and if the target requires runes to kill, if so, continue
            if (client.Modules.Cavebot.IsRunning && client.Player.Target != 0)
            {
                Creature target = client.Player.TargetCreature;
                if (target != null)
                {
                    bool creatureRequiresRunes = false;
                    ushort runeID = 0;
                    string targetName = target.Name.ToLower();

                    foreach (var t in client.Modules.Cavebot.GetTargets())
                    {
                        if (t.Name.ToLower() == targetName)
                        {
                            var setting = t.GetSettingByIndex(t.CurrentSettingIndex);
                            if (setting == null || !setting.UseThisSetting) continue;
                            ushort id = setting.GetRuneID();
                            if (id != 0)
                            {
                                creatureRequiresRunes = true;
                                runeID = id;
                                break;
                            }
                        }
                    }
                    if (creatureRequiresRunes)
                    {
                        if (client.Inventory.GetItem(runeID) != null) continue;
                    }
                }
            }

            if (client.Player.HealthPercent <= 70 && client.Player.Mana >= 40)
            {
                client.Packets.Say("exura gran");
                Thread.Sleep(rand.Next(600, 900));
                continue;
            }
            else if (client.Player.HealthPercent <= 50 && client.Player.Mana >= 80)
            {
                client.Packets.Say("exura vita");
                continue;
            }
            else if (client.Player.ManaPercent >= 98)
            {
                client.Packets.Say("utani hur");
                Thread.Sleep(rand.Next(500, 750));
            }
        }
    }
}
