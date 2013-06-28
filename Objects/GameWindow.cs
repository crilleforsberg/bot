using System;
using System.Collections.Generic;
using System.Threading;

namespace KarelazisBot.Objects
{
    public partial class GameWindow
    {
        public GameWindow(Objects.Client c)
        {
            this.Client = c;
            new Thread(this.UpdateCache).Start();
        }

        public Objects.Client Client { get; private set; }

        private Queue<Message> CachedMessages = new Queue<Message>();
        private int CacheCount = 100;
        private object SyncObject = new object();
        private readonly uint ForgeIndex = uint.MaxValue;
        public uint NextIndex
        {
            get { return this.Client.Memory.ReadUInt32(this.Client.Addresses.UI.GameWindow.Messages.NextIndex); }
            private set { this.Client.Memory.WriteUInt32(this.Client.Addresses.UI.GameWindow.Messages.NextIndex, value); }
        }

        private void UpdateCache()
        {
            try
            {
                uint oldIndex = 1,
                    highestReadIndex = 0;
                List<Message> activeMessages = new List<Message>();

                while (true)
                {
                    Thread.Sleep(500);

                    if (!this.Client.Player.Connected) continue;

                    uint nextIndex = this.NextIndex;
                    if (nextIndex < oldIndex) // player logged in
                    {
                        this.CachedMessages.Clear();
                        oldIndex = nextIndex;
                        highestReadIndex = 0;
                    }
                    else if (nextIndex == oldIndex) continue; // no new messages

                    foreach (Message msg in activeMessages.ToArray())
                    {
                        msg.UpdateTime();
                        if (msg.Time <= 0) activeMessages.Remove(msg);
                    }

                    List<Message> sortedMsgs = new List<Message>();
                    for (int i = this.Client.Addresses.UI.GameWindow.Messages.Start;
                        i < this.Client.Addresses.UI.GameWindow.Messages.Start +
                            this.Client.Addresses.UI.GameWindow.Messages.Step *
                            this.Client.Addresses.UI.GameWindow.Messages.MaxMessages;
                        i += this.Client.Addresses.UI.GameWindow.Messages.Step)
                    {
                        // read a few individual values to avoid parsing the whole message
                        uint index = this.Client.Memory.ReadUInt32(i + this.Client.Addresses.UI.GameWindow.Messages.Distances.Index);
                        if (index == this.ForgeIndex) continue;
                        bool isVisible = this.Client.Memory.ReadBool(i + this.Client.Addresses.UI.GameWindow.Messages.Distances.IsVisible);
                        int time = isVisible ? this.Client.Memory.ReadInt32(i + this.Client.Addresses.UI.GameWindow.Messages.Distances.Time) : 0;
                        if (time <= 0) continue;
                        if (highestReadIndex != 0 && index <= highestReadIndex) continue;

                        var msg = new Message(this.Client, i);
                        sortedMsgs.Add(msg);
                    }

                    if (sortedMsgs.Count == 0) continue;

                    sortedMsgs.Sort(delegate(Message first, Message second)
                    {
                        // dont bother checking more than one compare
                        // as indexes will be unique
                        return first.Index.CompareTo(second.Index);
                    });
                    highestReadIndex = sortedMsgs[sortedMsgs.Count - 1].Index;
                    activeMessages.AddRange(sortedMsgs);

                    lock (this.SyncObject)
                    {
                        for (int i = sortedMsgs.Count - 1; i >= 0; i--) this.CachedMessages.Enqueue(sortedMsgs[i]);

                        // delete old cached messages
                        while (this.CachedMessages.Count > this.CacheCount) this.CachedMessages.Dequeue();
                    }
                }
            }
            catch (Exception ex)
            {
                System.IO.File.AppendAllText("debug-gamewindow.txt",
                    ex.Message + "\n" + ex.StackTrace + "\n\n");
            }
        }

        public IEnumerable<Message> GetMessages()
        {
            lock (this.SyncObject)
            {
                foreach (Message msg in this.CachedMessages)
                {
                    if (!msg.IsVisible) continue;
                    yield return msg;
                }
            }
        }
        public IEnumerable<Message> GetAllCachedMessages()
        {
            lock (this.SyncObject)
            {
                return this.CachedMessages.ToArray();
            }
        }
        public void ForgeMessage(Message msg)
        {
            int address = this.Client.Addresses.UI.GameWindow.Messages.Start +
                this.Client.Addresses.UI.GameWindow.Messages.Step * (int)msg.ForgeIndex;

            this.Client.Memory.WriteBool(address + this.Client.Addresses.UI.GameWindow.Messages.Distances.IsVisible,
                msg.IsVisible);
            this.Client.Memory.WriteInt32(address + this.Client.Addresses.UI.GameWindow.Messages.Distances.Time,
                msg.Time);
            this.Client.Memory.WriteByte(address + this.Client.Addresses.UI.GameWindow.Messages.Distances.MessageType,
                (byte)msg.Type);
            this.Client.Memory.WriteUInt16(address + this.Client.Addresses.UI.GameWindow.Messages.Distances.X,
                (ushort)msg.Location.X);
            this.Client.Memory.WriteUInt16(address + this.Client.Addresses.UI.GameWindow.Messages.Distances.Y,
                (ushort)msg.Location.Y);
            this.Client.Memory.WriteByte(address + this.Client.Addresses.UI.GameWindow.Messages.Distances.Z,
                (byte)msg.Location.Z);
            this.Client.Memory.WriteUInt32(address + this.Client.Addresses.UI.GameWindow.Messages.Distances.Index,
                this.ForgeIndex);

            string[] split = msg.Text.Split('\n');
            this.Client.Memory.WriteByte(address + this.Client.Addresses.UI.GameWindow.Messages.Distances.LineCount,
                (byte)split.Length);
            for (int i = 0; i < split.Length; i++)
            {
                this.Client.Memory.WriteString(address + this.Client.Addresses.UI.GameWindow.Messages.Distances.LineText +
                    this.Client.Addresses.UI.GameWindow.Messages.LineStep * i,
                    split[i]);
            }
        }
    }
}
