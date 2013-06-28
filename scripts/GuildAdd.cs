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
    public static void Main(Client client)
    {
        string[] guildNames = new string[]
        {
            "Intouchables", "Omnislash", "Mc Gregors"
        };
        var icon = Vip.Character.VipIcon.Skull;

        List<string> addedNames = new List<string>();
        var currentVIP = client.Vip.GetCharacters();
        WebClient wc = new WebClient();
        var guilds = client.Modules.Website.GetGuilds(wc);
        foreach (string s in guildNames)
        {
            var guild = client.Modules.Website.GetGuild(s, wc, guilds);
            if (guild == null) continue;

            foreach (var member in guild.Members)
            {
                bool found = false;
                foreach (var vip in currentVIP)
                {
                    if (member.Name != vip.Name) continue;

                    if (vip.Icon != icon) vip.SetIcon(icon);

                    found = true;
                    break;
                }
                if (found) continue;

                client.Vip.Add(member.Name);
                addedNames.Add(member.Name);
                Thread.Sleep(200);
            }
        }

        foreach (var vip in client.Vip.GetCharacters())
        {
            if (!addedNames.Contains(vip.Name)) continue;

            vip.SetIcon(icon);
            Thread.Sleep(100);
        }
    }
}
