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
        //byte property = 2;
        var property = Enums.ObjectPropertiesFlags.IsTopOrder1;

        string datPath = client.TibiaProcess.MainModule.FileName;
        datPath = datPath.Substring(0, datPath.LastIndexOf('\\') + 1) + "Tibia.dat";
        if (!File.Exists(datPath)) return;
        ushort count = 0;
        using (FileStream fstream = File.OpenRead(datPath))
        {
            System.IO.BinaryReader reader = new System.IO.BinaryReader(fstream);
            reader.ReadUInt32(); // file signature
            count = (ushort)(reader.ReadUInt16() - 100); // item ids start at 100
        }
        string s = string.Empty;
        for (int i = 0; i < count; i++)
        {
            var flag = client.GetObjectProperty((ushort)(i + 100));
            if (!flag.HasFlag(property)) continue;
            s += (i + 100) + "\n";
            //Thread.Sleep(2);
        }
        File.WriteAllText("items with " + property + ".txt", s);
    }
}
