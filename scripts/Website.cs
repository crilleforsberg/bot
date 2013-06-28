using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Text.RegularExpressions;
using System.Net;
using KarelazisBot;
using KarelazisBot.Objects;
using KarelazisBot.Modules;

public class ExternalScript
{
    class GuildInfo
    {
        public string Name;
        public uint SdDamage = 0;
        public List<Website.Character> Members = new List<Website.Character>(),
            OnlineMembers = new List<Website.Character>();
    }

    static ushort EstimateMagicLevel(ushort level)
    {
        return (ushort)(Math.Round(30 * Math.Log(level)));
    }
    static uint GetBaseDamage(ushort level, ushort mlvl)
    {
        return (uint)(level * 2 + mlvl * 3);
    }
    static uint GetSdDamage(ushort level, ushort mlvl)
    {
        return (uint)(Math.Max(0, GetBaseDamage(level, mlvl) * 1.5 - 20));
    }

    public static void Main(Client client)
    {
        List<string> guildEnemies = new List<string>() { "Intouchables", "Mc Gregors" },
            guildAllies = new List<string>() { "Rambo Style", "Nameless", "Ruthless Seven", "Midsommar" };

        while (!client.Player.Connected) Thread.Sleep(500);

        GameWindow.Message msg = new GameWindow.Message()
        {
            ForgeIndex = 9,
            IsVisible = true,
            Location = new Location(1, 1, 1),
            Type = GameWindow.Message.Types.DarkYellowMessage,
            Time = 20000,
            Text = "Gathering data..."
        };

        client.Window.GameWindow.ForgeMessage(msg);

        WebClient wc = new WebClient();
        string oldHtml = string.Empty;
        while (true)
        {
            List<Website.Character> onlineCharacters = new List<Website.Character>();
            try { onlineCharacters = client.Modules.Website.GetOnlineCharacters(wc).ToList(); }
            catch
            {
                msg.Text = "Failed to parse online list";
                client.Window.GameWindow.ForgeMessage(msg);
                continue;
            }

            if (onlineCharacters.Count == 0) continue;

            Dictionary<string, int> guilds = new Dictionary<string, int>();
            try { guilds = client.Modules.Website.GetGuilds(wc); }
            catch
            {
                msg.Text = "Failed to parse guild list";
                client.Window.GameWindow.ForgeMessage(msg);
                continue;
            }

            try
            {
                List<GuildInfo> allies = new List<GuildInfo>(),
                    enemies = new List<GuildInfo>();

                foreach (string guild in guildAllies)
                {
                    allies.Add(new GuildInfo()
                    {
                        Name = guild,
                        Members = client.Modules.Website.GetCharactersInGuild(guilds[guild], wc).ToList()
                    });
                }
                foreach (string guild in guildEnemies)
                {
                    enemies.Add(new GuildInfo()
                    {
                        Name = guild,
                        Members = client.Modules.Website.GetCharactersInGuild(guilds[guild], wc).ToList()
                    });
                }

                // get online members
                uint onlineTotalAllies = 0, onlineTotalEnemies = 0;
                foreach (var character in onlineCharacters)
                {
                    bool found = false;

                    // check allied guilds
                    foreach (var guildInfo in allies)
                    {
                        foreach (var member in guildInfo.Members)
                        {
                            if (member.Name != character.Name) continue;

                            guildInfo.OnlineMembers.Add(member);
                            onlineTotalAllies++;
                            switch (member.Vocation)
                            {
                                case "Master Sorcerer":
                                case "Sorcerer":
                                case "Elder Druid":
                                case "Druid":
                                    guildInfo.SdDamage += GetSdDamage(member.Level, EstimateMagicLevel(member.Level)) / 2;
                                    break;
                                case "Paladin":
                                case "Royal Paladin":
                                    guildInfo.SdDamage += GetSdDamage(member.Level, 15) / 2;
                                    break;
                            }
                            found = true;
                            break;
                        }
                        if (found) break;
                    }
                    if (found) continue;

                    // check enemy guilds
                    foreach (var guildInfo in enemies)
                    {
                        foreach (var member in guildInfo.Members)
                        {
                            if (member.Name != character.Name) continue;

                            guildInfo.OnlineMembers.Add(member);
                            onlineTotalEnemies++;
                            switch (member.Vocation)
                            {
                                case "Master Sorcerer":
                                case "Sorcerer":
                                case "Elder Druid":
                                case "Druid":
                                    guildInfo.SdDamage += GetSdDamage(member.Level, EstimateMagicLevel(member.Level)) / 2;
                                    break;
                                case "Paladin":
                                case "Royal Paladin":
                                    guildInfo.SdDamage += GetSdDamage(member.Level, 15) / 2;
                                    break;
                            }
                            found = true;
                            break;
                        }
                        if (found) break;
                    }
                }

                // get some values
                uint totalAllies = 0, alliesSdDamage = 0,
                    totalEnemies = 0, enemiesSdDamage = 0;
                foreach (var guildInfo in allies)
                {
                    totalAllies += (uint)guildInfo.Members.Count;
                    alliesSdDamage += guildInfo.SdDamage;
                }
                foreach (var guildInfo in enemies)
                {
                    totalEnemies += (uint)guildInfo.Members.Count;
                    enemiesSdDamage += guildInfo.SdDamage;
                }

                msg.Text = "Allies (" + onlineTotalAllies + "/" + totalAllies + ", " + alliesSdDamage + " SD dmg):\n";
                foreach (var guildInfo in allies)
                {
                    msg.Text += guildInfo.Name + " (" + guildInfo.OnlineMembers.Count + "/" + guildInfo.Members.Count + ", " +
                        guildInfo.SdDamage + " SD dmg)\n";
                }
                msg.Text += "\nEnemies (" + onlineTotalEnemies + "/" + totalEnemies + ", " + enemiesSdDamage + " SD dmg):\n";
                foreach (var guildInfo in enemies)
                {
                    msg.Text += guildInfo.Name + " (" + guildInfo.OnlineMembers.Count + "/" + guildInfo.Members.Count + ", " +
                        guildInfo.SdDamage + " SD dmg)\n";
                }

                client.Window.GameWindow.ForgeMessage(msg);
            }
            catch (Exception ex)
            {
                System.IO.File.AppendAllText("debug-website.txt", ex.Message + "\n" + ex.StackTrace + "\n");
                msg.Text = "Parsing guilds failed";
                client.Window.GameWindow.ForgeMessage(msg);
                continue;
            }

            Thread.Sleep(10000);
        }
    }
}
