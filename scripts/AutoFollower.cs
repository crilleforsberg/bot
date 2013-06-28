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
        string targetName = "NameGoesHere";

        while (true)
        {
            Thread.Sleep(1000);

            if (!client.Player.Connected) continue;

            Location playerLoc = client.Player.Location;
            uint followingID = client.Player.FollowID;
            foreach (Creature target in client.BattleList.GetPlayers(true, true))
            {
                if (!playerLoc.IsOnScreen(target.Location)) continue;
                if (target.Name == targetName && target.ID != client.Player.Follow)
                {
                    target.Follow();
                    break;
                }
            }
        }
    }
}