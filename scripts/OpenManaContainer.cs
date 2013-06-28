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
        while (true)
        {
            Thread.Sleep(500);
            foreach (Container container in client.Inventory.GetContainers())
            {
                if (!container.IsFull || !container.IsOpen) continue;
                // uncomment to only open inside backpacks
                // if (!container.Name.Contains("Backpack")) continue;
                int count = 0;
                foreach (Item item in container.GetItems())
                {
                    if (item.HasFlag(Enums.ObjectPropertiesFlags.IsFluidContainer) &&
                        item.Count != (byte)Enums.FluidType.Nothing)
                    {
                        count++;
                    }
                }
                if (count != 0) continue;

                // find new container
                foreach (Item item in container.GetItems())
                {
                    if (item.HasFlag(Enums.ObjectPropertiesFlags.IsContainer))
                    {
                        item.Use();
                        break;
                    }
                }
            }
        }
    }
}