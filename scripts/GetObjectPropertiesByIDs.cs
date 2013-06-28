using System;
using System.Collections.Generic;
using System.Threading;
using System.Linq;
using System.IO;
using KarelazisBot;
using KarelazisBot.Objects;

public class Test
{
    public static void Main(Client client)
    {
        string filePath = "items with 1.txt";
        if (!File.Exists(filePath)) return;
        List<ushort> ids = new List<ushort>();
        using (FileStream fstream = File.OpenRead(filePath))
        {
            using (StreamReader reader = new StreamReader(fstream))
            {
                string line = string.Empty;
                while ((line = reader.ReadLine()) != null)
                {
                    ids.Add(ushort.Parse(line));
                }
            }
        }
        string s = string.Empty;
        foreach (ushort id in ids)
        {
            s += "\n ~ " + id + " ~\n";
            for (byte i = 0; i < 30; i++)
            {
                if (client.Packets.GetObjectProperty(id, i)) s += i + " ";
            }
        }
        File.WriteAllText("items.txt", s);
    }
}