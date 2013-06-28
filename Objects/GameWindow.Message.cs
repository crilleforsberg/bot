using System;

namespace KarelazisBot.Objects
{
    public partial class GameWindow
    {
        public class Message
        {
            /// <summary>
            /// Constructor for creating a new message.
            /// </summary>
            public Message() { }
            /// <summary>
            /// Constructor for an existing message.
            /// </summary>
            /// <param name="c"></param>
            /// <param name="address"></param>
            public Message(Objects.Client c, int address)
            {
                this.Client = c;
                this.Address = address;
                this.TimeCreated = DateTime.Now;

                this.Index = c.Memory.ReadUInt32(address + c.Addresses.UI.GameWindow.Messages.Distances.Index);
                this.IsVisible = c.Memory.ReadByte(address + c.Addresses.UI.GameWindow.Messages.Distances.IsVisible) == 1;
                this.Time = this.IsVisible ? c.Memory.ReadInt32(address + c.Addresses.UI.GameWindow.Messages.Distances.Time) : 0;
                this.Type = Types.Unspecified;
                if (this.Time <= 0) return;

                this.Location = new Objects.Location(c.Memory.ReadUInt16(address + c.Addresses.UI.GameWindow.Messages.Distances.X),
                    c.Memory.ReadUInt16(address + c.Addresses.UI.GameWindow.Messages.Distances.Y),
                    c.Memory.ReadByte(address + c.Addresses.UI.GameWindow.Messages.Distances.Z));
                this.Type = (Types)c.Memory.ReadByte(address + c.Addresses.UI.GameWindow.Messages.Distances.MessageType);

                string[] split = new string[c.Memory.ReadByte(address + c.Addresses.UI.GameWindow.Messages.Distances.LineCount)];
                for (int i = 0; i < split.Length; i++)
                {
                    split[i] = c.Memory.ReadString(address + c.Addresses.UI.GameWindow.Messages.Distances.LineText +
                        i * c.Addresses.UI.GameWindow.Messages.LineStep, 40, true);
                }
                if (split.Length > 0)
                {
                    string str = split[0];
                    switch (this.Type)
                    {
                        case Types.Say:
                            this.Sender = str.Remove(str.LastIndexOf(" says:"));
                            if (split.Length > 1) this.Text = string.Join(" ", split, 1, split.Length - 1);
                            break;
                        case Types.Whisper:
                            this.Sender = str.Remove(str.LastIndexOf(" whispers:"));
                            if (split.Length > 1) this.Text = string.Join(" ", split, 1, split.Length - 1);
                            break;
                        case Types.Yell:
                            this.Sender = str.Remove(str.LastIndexOf(" yells:"));
                            if (split.Length > 1) this.Text = string.Join(" ", split, 1, split.Length - 1);
                            break;
                        case Types.PrivateMessage:
                            this.Sender = str.Remove(str.LastIndexOf(":"));
                            this.Text = string.Join(" ", split, 1, split.Length - 1);
                            break;
                        default:
                            this.Sender = string.Empty;
                            this.Text = string.Join(" ", split);
                            break;
                    }
                }
            }

            public Objects.Client Client { get; private set; }
            public int Address { get; set; }
            public int Time { get; set; }
            public DateTime TimeCreated { get; private set; }
            public Types Type { get; set; }
            public string Text { get; set; }
            public string Sender { get; set; }
            public Location Location { get; set; }
            public bool IsVisible { get; set; }
            public uint Index { get; set; }
            public uint ForgeIndex { get; set; }

            public enum Types
            {
                Unspecified = 0,
                Say = 1,
                Whisper = 2,
                Yell = 3,
                PrivateMessage = 4,
                /// <summary>
                /// A white message.
                /// Can use locations.
                /// </summary>
                WhiteMessage = 6,
                /// <summary>
                /// A static red message.
                /// Can not use locations.
                /// </summary>
                RedMessageStatic = 9,
                /// <summary>
                /// A red message.
                /// Can use locations.
                /// </summary>
                RedMessage = 10,
                /// <summary>
                /// A dark yellow message.
                /// Can use locations.
                /// </summary>
                DarkYellowMessage = 12,
                /// <summary>
                /// An orange message.
                /// Can use locations.
                /// </summary>
                OrangeMessage = 16,
                /// <summary>
                /// A static white message. Typically used to display raid information.
                /// </summary>
                WhiteMessageStatic = 19,
                /// <summary>
                /// A green message that typically appears when you look at items.
                /// </summary>
                GreenMessage = 22,
                /// <summary>
                /// A blue message.
                /// Can use locations.
                /// </summary>
                BlueMessage = 24
            }

            public void UpdateTime()
            {
                if (!this.IsVisible) return;

                this.IsVisible = this.Client.Memory.ReadByte(this.Address +
                    this.Client.Addresses.UI.GameWindow.Messages.Distances.IsVisible) == 1;
                this.Time = this.IsVisible ?
                    this.Client.Memory.ReadInt32(this.Address + this.Client.Addresses.UI.GameWindow.Messages.Distances.Time) :
                    0;
            }
        }
    }
}
