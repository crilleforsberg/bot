using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using KarelazisBot;
using KarelazisBot.Objects;

public class Test
{
    public static void Main(Client client)
    {
        Dictionary<string, Item> itemCollection = new Dictionary<string, Item>();
        itemCollection.Add("bread", new Item(client) { ID = 2689 });
        bool simple = true;

        while (client.Player.IsWalking) Thread.Sleep(500);

        Container container = client.Inventory.GetContainer(0);
        if (!container.IsOpen) return;

        foreach (Item item in container.GetItems())
        {
            if (simple)
            {
                foreach (Item storedItem in itemCollection.Values)
                {
                    if (item.ID != storedItem.ID) continue;

                    storedItem.Count += item.Count == 0 ? (ushort)1 : item.Count;
                    break;
                }
            }
            else
            {
                if (!item.HasFlag(Enums.ObjectPropertiesFlags.IsContainer)) continue;

                Container newContainer = client.Inventory.GetFirstClosedContainer();
                if (newContainer == null) return; // no free container slots
                while (container.IsOpen && !newContainer.IsOpen)
                {
                    item.OpenInNewWindow();
                    Thread.Sleep(200);
                }
                if (!newContainer.IsOpen) continue;

                foreach (Item newItem in newContainer.GetItems())
                {
                    foreach (Item storedItem in itemCollection.Values)
                    {
                        if (newItem.ID != storedItem.ID) continue;

                        storedItem.Count += newItem.Count == 0 ? (ushort)1 : newItem.Count;
                        break;
                    }
                }
                newContainer.Close();
            }
        }

        string npcMessage = "Hello " + client.Player.Name +
            "! Welcome to our humble farm.";
        do
        {
            Say(client, "hi");
        }
        while (!WaitForResponse(client, npcMessage, "Sherry McRonald", 10000));
        foreach (var keypair in itemCollection)
        {
            if (keypair.Value.Count == 0) continue;
            Say(client, "sell " + keypair.Value.Count + " " + keypair.Key);
            Say(client, "yes");
            Thread.Sleep(1000);
        }
        Say(client, "bye");
        client.Inventory.GroupItems();
    }

    static bool WaitForResponse(Client client, string expectedResponse, string npcName = "", ushort time = 3000)
    {
        int tickStart = Environment.TickCount;
        while (Environment.TickCount < tickStart + time)
        {
            foreach (var msg in client.Window.GameWindow.GetMessages())
            {
                if (msg.Text != expectedResponse) continue;
                if (!string.IsNullOrEmpty(npcName) && npcName != msg.Sender) continue;
                return true;
            }

            Thread.Sleep(500);
        }
        return false;
    }
    static void Say(Client client, string message)
    {
        Thread.Sleep(message.Length * 300);
        client.Packets.Say(message);
    }
}
