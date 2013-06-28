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
        string monster = "rotworms";

        DelayedSay("hi");
        DelayedSay("yes");
        for (int i = 0; i < 20; i++)
        {
            if (client.Memory.ReadByte(client.Addresses.UI.ActionState) != (byte)Enums.ActionStates.Dialog) continue;

            client.Memory.WriteByte(client.Addresses.UI.ActionState, 0);
            break;
        }
        DelayedSay("reward");
    }

    static void DelayedSay(string text, int timePerChar = 300)
    {
        if (string.IsNullOrEmpty(text)) return;
        Thread.Sleep(text.Length * timePerChar);
        client.Packets.Say(text);
    }
}
