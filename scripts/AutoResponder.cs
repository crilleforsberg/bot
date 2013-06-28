using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Linq;
using System.Text.RegularExpressions;
using KarelazisBot;
using KarelazisBot.Objects;

public class ExternalScript
{
    public static void Main(Client client)
    {
        bool logConversations = true, ignoreSameMessages = true, onlyConsiderLocalMessages = true;

        Dictionary<List<string>, List<string>> keywords = new Dictionary<List<string>, List<string>>();
        keywords.Add(new List<string>()
        {
            "hi", "helo", "hello", "howdy", "heyo", "yo", "yoo", "siema", "hey", "greetings", "salutations"
        },
        new List<string>()
        {
            "hi", "hello", "howdy", "heyo", "yo", "hey", "greetings"
        });
        keywords.Add(new List<string>()
        {
            "howlong", "long"
        },
        new List<string>()
        {
            "a couple of hours", "I dont know", "dunno, I can message you later though"
        });
        // if the FIRST message from a player is unmatched,
        // use this keyword
        string unknownFirstKeyword = "hi";

        List<Conversation> convos = new List<Conversation>();
        Random rand = new Random();
        Regex regex = new Regex("[^a-zA-ZåäöÅÄÖ]");
        uint highestReadIndex = 0, highestNextIndex = 0;
        while (true)
        {
            Thread.Sleep(1000);

            if (!client.Player.Connected) continue;

            string playerName = client.Player.Name,
                response = string.Empty;
            Location playerLoc = client.Player.Location;
            bool responded = false;

            uint nextIndex = client.Window.GameWindow.NextIndex;
            if (highestNextIndex > nextIndex) highestReadIndex = 0;
            highestNextIndex = nextIndex;

            foreach (var message in client.Window.GameWindow.GetMessages())
            {
                if (highestReadIndex >= message.Index) continue;
                if (message.Sender == playerName) continue;
                if (onlyConsiderLocalMessages &&
                    message.Type != GameWindow.Message.Types.PrivateMessage &&
                    !playerLoc.IsOnScreen(message.Location))
                {
                    continue;
                }

                highestReadIndex = message.Index;

                switch (message.Type)
                {
                    case GameWindow.Message.Types.Say:
                    case GameWindow.Message.Types.Yell:
                    case GameWindow.Message.Types.PrivateMessage:
                        string strippedMessage = regex.Replace(message.Text, string.Empty);

                        response = GetResponse(keywords, strippedMessage.ToLower(), rand, unknownFirstKeyword);
                        if (string.IsNullOrEmpty(response)) break;

                        Thread.Sleep(Math.Max(1000, strippedMessage.Length * 100)); // simulate reading
                        Conversation convo = GetConvo(convos, playerName, message.Sender);
                        if (convo == null) convo = new Conversation(client, playerName, message.Sender, logConversations);
                        convo.AddMessage(message.Sender,
                            message.Type == GameWindow.Message.Types.PrivateMessage ? playerName : "Default chat",
                            message.Text);

                        if (!string.IsNullOrEmpty(unknownFirstKeyword) && convo.MessageCount > 1)
                        {
                            bool found = false;
                            foreach (var keypair in keywords)
                            {
                                if (!keypair.Key.Contains(unknownFirstKeyword)) continue;
                                found = true;
                                break;
                            }
                            if (found) break;
                        }

                        if (ignoreSameMessages)
                        {
                            var convoMsg = convo.GetLastReceivedMessage();
                            if (convoMsg != null)
                            {
                                string convoStrippedMessage = regex.Replace(convoMsg.Text, string.Empty);
                                bool foundCurrent = false, foundOld = false;
                                foreach (var keypair in keywords)
                                {
                                    if (keypair.Key.Contains(strippedMessage)) foundCurrent = true;
                                    if (keypair.Key.Contains(convoStrippedMessage)) foundOld = true;

                                    if (foundCurrent && foundOld) break;
                                }
                                if (foundCurrent && foundOld) break;
                            }
                        }

                        // check if sender is visible on screen
                        Creature sender = client.BattleList.GetPlayer(message.Sender);
                        if (sender == null || !sender.Location.IsOnScreen(playerLoc))
                        {
                            Thread.Sleep(rand.Next(2000, 3500)); // simulate opening/focusing channel
                            DelayedPrivateMessage(client, message.Sender, response);
                            convo.AddMessage(playerName, message.Sender, response);
                        }
                        else
                        {
                            DelayedSay(client, response);
                            convo.AddMessage(playerName, "Default chat", response);
                        }

                        responded = true;
                        break;
                    default:
                        break;
                }

                if (responded) break;
            }
        }
    }

    static Conversation GetConvo(IEnumerable<Conversation> convos, string playerName, string otherName)
    {
        foreach (var convo in convos)
        {
            if (convo.OtherName == otherName) return convo;
        }
        return null;
    }
    static void DelayedSay(Client client, string text, ushort timePerChar = 350)
    {
        if (string.IsNullOrEmpty(text)) return;
        Thread.Sleep(text.Length * timePerChar);
        client.Packets.Say(text);
    }
    static void DelayedPrivateMessage(Client client, string recipient, string text, ushort timePerChar = 300)
    {
        if (string.IsNullOrEmpty(recipient) || string.IsNullOrEmpty(text)) return;
        Thread.Sleep(text.Length * timePerChar);
        client.Packets.PrivateMessage(recipient, text);
    }
    static string GetResponse(Dictionary<List<string>, List<string>> dictionary, string message,
        Random rand, string unknownFirstKeyword = "")
    {
        foreach (var keypair in dictionary)
        {
            if (keypair.Key.Contains(message)) return keypair.Value[rand.Next(keypair.Value.Count)];
        }

        if (string.IsNullOrEmpty(unknownFirstKeyword)) return string.Empty;

        foreach (var keypair in dictionary)
        {
            if (keypair.Key.Contains(unknownFirstKeyword)) return keypair.Value[rand.Next(keypair.Value.Count)];
        }

        return string.Empty;
    }

    class Conversation
    {
        public Conversation(Client client, string playerName, string otherName, bool logMessages = false)
        {
            this.Client = client;
            this.PlayerName = playerName;
            this.OtherName = otherName;
            this.Messages = new List<Message>();
            this.LogMessages = logMessages;
            this.SyncObject = new object();
        }

        public Client Client { get; private set; }
        public string PlayerName { get; private set; }
        public string OtherName { get; private set; }
        public bool LogMessages { get; set; }
        public int MessageCount
        {
            get { return this.Messages.Count; }
        }
        private List<Message> Messages { get; set; }
        private object SyncObject { get; set; }

        public void AddMessage(string sender, string recipient, string text)
        {
            var msg = new Message(sender, recipient, text);
            lock (this.SyncObject)
            {
                this.Messages.Add(msg);
            }

            if (this.LogMessages)
            {
                try
                {
                    string dirName = "conversation logs";
                    string dirPath = Path.Combine(this.Client.BotDirectory.FullName, dirName);
                    if (!Directory.Exists(dirPath)) this.Client.BotDirectory.CreateSubdirectory(dirName);
                    File.AppendAllText(Path.Combine(dirPath, this.PlayerName + " - " + this.OtherName + ".txt"),
                        msg.ToString() + "\n");
                }
                catch { }
            }
        }
        public Message GetLastReceivedMessage()
        {
            if (this.Messages.Count == 0) return null;

            lock (this.SyncObject)
            {
                for (int i = this.Messages.Count - 1; i >= 0; i--)
                {
                    if (this.Messages[i].Sender == this.OtherName) return this.Messages[i];
                }
            }

            return null;
        }
        public IEnumerable<Message> GetMessages()
        {
            return this.Messages.ToArray();
        }

        public class Message
        {
            public Message(string sender, string recipient, string text)
            {
                this.Sender = sender;
                this.Recipient = recipient;
                this.Text = text;
                this.Time = DateTime.Now;
            }

            public string Sender { get; private set; }
            public string Recipient { get; private set; }
            public string Text { get; private set; }
            public DateTime Time { get; private set; }

            public override string ToString()
            {
                return string.Format("[{0:yy/MM/dd HH:mm:ss}]", this.Time) + " " + Sender + " -> " +
                    this.Recipient + ": " + this.Text;
            }
        }
    }
}