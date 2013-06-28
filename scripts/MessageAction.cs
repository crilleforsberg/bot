using System; // access to basic stuff in .NET
using System.Collections.Generic; // access to things like List<T> (T = value type) in .NET
using System.Threading;
using System.Linq;
using KarelazisBot; // base access to enumerators, Windows API and such
using KarelazisBot.Objects; // access to all the objects

// this can be named whatever, basically only useful if you're writing libraries
// you can also use namespaces!
public class Test
{
    // entry point of all scripts must be called Main
    // libriares does not require an entry point
    public static void Main(Client client)
    {
        List<string> names = new List<string>()
        {
            "Vivec"
        };

        while (true)
        {
            Thread.Sleep(100);
            if (!client.Player.Connected) continue;

            foreach (var msg in client.Window.GameWindow.GetMessages())
            {
                if (msg.Type != 4 || msg.Message.Length == 0) continue;

                string sender = msg.Message[0];
                sender = sender.Substring(0, sender.LastIndexOf(':')); // grab name (i.e. Name: becomes Name)
                if (!names.Contains(sender)) continue;

                string message = string.Join("\n", msg.Message, 1, msg.Message.Length - 1);
                if (message.ToLower() != "rope") continue;

                Item rope = client.Inventory.GetItem(client.ItemList.Tools.Rope);
                if (rope == null) break;
                rope.UseOnLocation(client.Player.Location.Offset(y: 1));
                client.Memory.WriteByte(msg.Address + client.Addresses.GameWindowMessageDistances.IsVisible, 0);
                break;
            }
        }
    }
}