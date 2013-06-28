using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Windows.Forms;
using KarelazisBot;
using KarelazisBot.Objects;

public class ExternalScript
{
    public static void Main(Client client)
    {
        Login(client, "1234567", "password", "Name");
    }

    static bool Login(Client client, string account, string password, string characterName)
    {
        while (true)
        {
            switch ((Enums.Connection)client.Memory.ReadByte(client.Addresses.Misc.Connection))
            {
                default:
                    Thread.Sleep(1000);
                    break;
                case Enums.Connection.Offline:
                    string dialogTitle = client.Window.GetCurrentDialogTitle();

                    if (string.IsNullOrEmpty(dialogTitle))
                    {
                        var size = client.Window.GetWindowSize();
                        client.Modules.Hotkeys.SendLeftMouseClick(120, size.Height - 215);
                        Thread.Sleep(500);
                        break;
                    }

                    switch (dialogTitle)
                    {
                        default:
                            client.Modules.Hotkeys.Press(Keys.Escape);
                            Thread.Sleep(500);
                            return Login(client, account, password, characterName);
                        case "Enter Game":
                            client.Modules.Hotkeys.Write(account);
                            Thread.Sleep(500);
                            client.Modules.Hotkeys.Press(Keys.Tab);
                            Thread.Sleep(500);
                            client.Modules.Hotkeys.Write(password);
                            Thread.Sleep(500);
                            client.Modules.Hotkeys.Press(Keys.Enter);
                            Thread.Sleep(500);
                            return Login(client, account, password, characterName);
                        case "Sorry": // bad credentials or some other error
                            client.Modules.Hotkeys.Press(Keys.Escape);
                            Thread.Sleep(200);
                            return false;
                        case "Select Character":
                            // check if our character exists in the current character list
                            int foundIndex = -1;
                            foreach (CharacterList.Character c in client.CharacterList.GetCharacters())
                            {
                                if (c.Name != characterName) continue;

                                foundIndex = c.Index;
                                break;
                            }

                            if (foundIndex == -1)
                            {
                                for (int i = 0; i < 3; i++)
                                {
                                    client.Modules.Hotkeys.Press(Keys.Escape);
                                    Thread.Sleep(200);
                                }
                                break;
                            }

                            for (int i = 0; i < client.CharacterList.Count; i++)
                            {
                                client.Modules.Hotkeys.Press(Keys.Up);
                                Thread.Sleep(200);
                            }
                            for (int i = 0; i < foundIndex; i++)
                            {
                                client.Modules.Hotkeys.Press(Keys.Down);
                                Thread.Sleep(200);
                            }
                            client.Modules.Hotkeys.Press(Keys.Return);
                            Thread.Sleep(500);
                            break;
                    }
                    break;
                case Enums.Connection.Online:
                    client.Player.SetBattleListAddress();
                    if (client.Player.Name == characterName) return true;
                    while (client.Player.Connected)
                    {
                        client.Packets.Logout();
                        Thread.Sleep(500);
                    }
                    break;
            }
        }
    }
}
