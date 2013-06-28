using System;
using System.Collections.Generic;
using System.Threading;
using System.Linq;
using KarelazisBot;
using KarelazisBot.Objects;

public class KarelazisLib
{
    public static string GetClassName() { return "KarelazisLib"; }
	
	public static void DelayedSay(Client client, string message)
	{
		//Thread.Sleep(message.Length * 260);
		client.Packets.Say(message);
	}
}
