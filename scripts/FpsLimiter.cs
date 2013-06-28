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
        bool limitedFps = false;
        double oldFpsLimit = 0;
        IntPtr handle = client.TibiaProcess.MainWindowHandle;
        while (true)
        {
            bool isMinimized = WinAPI.IsIconic(handle);
            if (!limitedFps && isMinimized)
            {
                oldFpsLimit = GetFpsLimit(client);
                SetFpsLimit(client, 1);
                limitedFps = true;
            }
            else if (limitedFps && !isMinimized)
            {
                SetFpsLimit(client, oldFpsLimit);
                limitedFps = false;
            }
            Thread.Sleep(100);
        }
    }

    static ushort GetFpsLimit(Client client)
    {
        return (ushort)(1000 / client.Memory.ReadDouble(client.Memory.ReadInt32(client.Addresses.Fps.Pointer) +
            client.Addresses.Fps.LimitOffset));
    }
    static void SetFpsLimit(Client client, double fps)
    {
        if (fps < 1) fps = 1;
        client.Memory.WriteDouble(client.Memory.ReadInt32(client.Addresses.Fps.Pointer) +
            client.Addresses.Fps.LimitOffset, 1000 / fps);
    }
    static double GetFps(Client client)
    {
        return Math.Round(client.Memory.ReadDouble(client.Memory.ReadInt32(client.Addresses.Fps.Pointer) +
            client.Addresses.Fps.CurrentFramerateOffset), 2);
    }
}