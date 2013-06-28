using System; // access to basic stuff in .NET
using System.Collections.Generic; // access to things like List<T> (T = value type) in .NET
using KarelazisBot; // base access to enumerators, Windows API and such
using KarelazisBot.Objects; // access to all the objects
using System.Threading;

// this can be named whatever, basically only useful if you're writing libraries
// you can also use namespaces!
public class Test
{
    // entry point of all scripts must be called Main
    // libriares does not require an entry point
    public static void Main(Client client)
    {
        while (true)
        {
			client.Packets.Turn(client.Player.Direction);
            Thread.Sleep(new Random().Next(1000 * 60 * 5, 1000 * 60 * 9));
        }
    }
}
