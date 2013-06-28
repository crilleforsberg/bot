using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using KarelazisBot;
using KarelazisBot.Objects;

public class ExternalScript
{
    public static void Main(Client client)
    {
        // how many seconds to wait until re-attack
        int secondsThreshold = 5;

        uint id = 0;
        Creature target = null;
        Check check = null;

        while (true)
        {
            Thread.Sleep(1000);

            if (!client.Player.Connected) continue;

            uint playerTargetId = client.Player.Target;
            if (playerTargetId == 0) continue;

            if (playerTargetId != id)
            {
                id = playerTargetId;
                target = client.BattleList.GetCreature(id);
                check = null;
            }
            if (target == null || !client.Player.Location.IsOnScreen(target.Location)) continue;

            if (check == null) check = new Check();

            int tick = Environment.TickCount;
            if (!check.IsValid())
            {
                check.Set(target.HealthPercent, tick);
                continue;
            }
            if (check.Time + secondsThreshold * 1000 > tick) continue;
            else if (check.HealthPercent != target.HealthPercent) continue;

            target.Attack();
            check.Set(target.HealthPercent, tick);
        }
    }

    class Check
    {
        public ushort HealthPercent;
        public int Time;

        public bool IsValid()
        {
            return this.Time != 0 && this.HealthPercent != 0;
        }
        public void Set(ushort hppc, int time)
        {
            this.HealthPercent = hppc;
            this.Time = time;
        }
    }
}
