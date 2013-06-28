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
        foreach (var vip in client.Vip.GetCharacters())
        {
            client.Vip.Remove(vip);
        }
    }
}