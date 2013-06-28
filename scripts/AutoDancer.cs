using System;
using System.Collections.Generic;
using System.Threading;
using System.Linq;
using KarelazisBot;
using KarelazisBot.Objects;

public class Test
{
    public static void Main(Client client)
    {
        Random rand = new Random();
        while (true)
        {
            Thread.Sleep(rand.Next(200, 600));
            if (!client.Player.Connected) continue;

            int direction = rand.Next(4);
            switch (direction)
            {
                case 0:
                    client.Packets.Turn(Enums.Direction.Up);
                    break;
                case 1:
                    client.Packets.Turn(Enums.Direction.Right);
                    break;
                case 2:
                    client.Packets.Turn(Enums.Direction.Down);
                    break;
                case 3:
                    client.Packets.Turn(Enums.Direction.Left);
                    break;
            }
        }
    }
}