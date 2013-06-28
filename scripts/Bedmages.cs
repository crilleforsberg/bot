using System;
using System.Collections.Generic;
using System.Threading;
using System.Linq;
using KarelazisBot;
using KarelazisBot.Objects;
using System.Windows.Forms;

public class Test
{
    static AutoResetEvent resetEvent = new AutoResetEvent(false);
    static int manaPerHour = 4 * 60, // 4 mana/min
        maxRegenTime = 1000 * 60 * 100; // max 100 minutes of regen while sleeping 

    public static void Main(Client client)
    {
        // bed food blanks, finished runes
        // x, y, z
        // x+ = ->, x- = <-
        // y+ = \/, y- = /\
        List<Bedmage> bedmages = new List<Bedmage>()
        {
            new Bedmage(client, 600789, "oldani778", "Darkoz", "adori gran", 70,
                new Location(32178, 31659, 7), new Location(32177, 31659, 7), new Location(32177, 31658, 7),
                new Location(32177, 31660, 7)),
            new Bedmage(client, 600789, "oldani778", "Maloz", "adori gran", 70,
                new Location(32178, 31658, 7), new Location(32177, 31659, 7), new Location(32177, 31658, 7),
                new Location(32177, 31660, 7))
        };
        List<Food> foods = new List<Food>()
        {
            new Food() { ItemID = client.ItemList.Food.Ham, RegenerationTime = 360 * 5 * 1000 },
            new Food() { ItemID = client.ItemList.Food.Meat, RegenerationTime = 180 * 5* 1000 },
            new Food() { ItemID = client.ItemList.Food.Fish, RegenerationTime = 144 * 5 * 1000 }
        };

        TimeSpan timeOfServerSave = new TimeSpan(4, 0, 0); // 4 AM UTC
        Random rand = new Random();

        while (true)
        {
            #region logic
            if (resetEvent.WaitOne(rand.Next(1000 * 3, 1000 * 7))) break;

            TimeSpan currentTime = DateTime.UtcNow.TimeOfDay;
            // check if server save is about to happen
            if (currentTime < timeOfServerSave &&
                (timeOfServerSave - currentTime).TotalMinutes <= 5)
            {
                System.IO.File.AppendAllText("bedmagedebug.txt", "waiting for server save\n");
                continue;
            }
            // check if server save has happened
            // wait until server save + 30 mins before continuing
            if (currentTime >= timeOfServerSave &&
                (currentTime - timeOfServerSave).TotalMinutes <= 30)
            {
                System.IO.File.AppendAllText("bedmagedebug.txt", "waiting for server save\n");
                continue;
            }

            long time = Environment.TickCount;
            foreach (Bedmage bmage in bedmages)
            {
                try
                {
                    // check if one hour has passed
                    // if not, skip this bedmage
                    if (bmage.TimeOfLastLogin != 0 && bmage.TimeOfLastLogin + bmage.TimeNeededToRegen > time) continue;

                    // skip if login failed
                    if (!bmage.Login())
                    {
                        System.IO.File.AppendAllText("bedmagedebug.txt", "login failed\n");
                        if (client.Player.Connected) bmage.GoToBed();
                        continue;
                    }

                    if (!client.Player.Connected)
                    {
                        System.IO.File.AppendAllText("bedmagedebug.txt", "login failed\n");
                        continue;
                    }

                    // check if food and rune locations are visible
                    if (!client.Player.Location.IsOnScreen(bmage.FoodLocation) ||
                        (!bmage.IsInstantSpell && !client.Player.Location.IsOnScreen(bmage.BlankRuneLocation)))
                    {
                        System.IO.File.AppendAllText("bedmagedebug.txt", "locations offscreen\n");
                        bmage.GoToBed();
                        continue;
                    }

                    TileCollection tiles = client.Map.GetTilesOnScreen();

                    // get tiles and check their validity
                    Tile foodTile = tiles.GetTile(bmage.FoodLocation),
                        runeTile = bmage.IsInstantSpell ? null : tiles.GetTile(bmage.BlankRuneLocation);
                    if (foodTile == null || (!bmage.IsInstantSpell && runeTile == null))
                    {
                        System.IO.File.AppendAllText("bedmagedebug.txt", "tile null\n");
                        bmage.GoToBed();
                        continue;
                    }

                    // get top items and check their validity
                    TileObject topItemFood = foodTile.GetTopUseItem(false),
                        topItemRune = bmage.IsInstantSpell ? null : runeTile.GetTopUseItem(false);
                    if (topItemFood == null || (!bmage.IsInstantSpell && topItemRune == null))
                    {
                        System.IO.File.AppendAllText("bedmagedebug.txt", "top item null\n");
                        bmage.GoToBed();
                        continue;
                    }
                    if (!topItemFood.HasFlag(Enums.ObjectPropertiesFlags.IsPickupable) ||
                        (!bmage.IsInstantSpell && !topItemRune.HasFlag(Enums.ObjectPropertiesFlags.IsContainer)))
                    {
                        System.IO.File.AppendAllText("bedmagedebug.txt", "bad flags\n");
                        bmage.GoToBed();
                        continue;
                    }

                    // open blank rune container
                    while (client.Player.Connected && client.Player.Mana >= bmage.SpellMana)
                    {
                        Thread.Sleep(1000);

                        if (!client.Player.Connected) break;

                        if (bmage.IsInstantSpell)
                        {
                            client.Packets.Say(bmage.SpellName);
                            break;
                        }

                        tiles = client.Map.GetTilesOnScreen();

                        Container container = bmage.OpenContainer(tiles, bmage.BlankRuneLocation);
                        if (container == null || !container.IsOpen)
                        {
                            bmage.GoToBed();
                            continue;
                        }
                        var blankRunes = container.GetItems(client.ItemList.Runes.Blank).ToArray();
                        if (blankRunes.Length > 0) client.Player.MakeRune(bmage.SpellName, bmage.SpellMana);

                        // check if there still are blank runes
                        blankRunes = container.GetItems(client.ItemList.Runes.Blank).ToArray();
                        if (blankRunes.Length > 0) break;

                        // no blanks, move container
                        if (!client.Player.Location.IsOnScreen(bmage.FinishedRunesLocation)) break;
                        runeTile = tiles.GetTile(bmage.BlankRuneLocation);
                        if (runeTile == null) break;
                        topItemRune = runeTile.GetTopMoveItem();
                        if (topItemRune == null || !topItemRune.HasFlag(Enums.ObjectPropertiesFlags.IsContainer)) break;
                        topItemRune.Move(new ItemLocation(bmage.FinishedRunesLocation));
                    }

                    Food foodEaten = null;
                    // eat food
                    while (client.Player.Connected)
                    {
                        Thread.Sleep(500);

                        if (!client.Player.Connected) break;

                        if (client.Window.StatusBar.GetText() == Enums.StatusBar.YouAreFull) break;

                        if (!client.Player.Location.IsOnScreen(bmage.FoodLocation)) break;

                        // walk to food if we're not there
                        if (client.Player.Location.DistanceTo(bmage.FoodLocation) >= 2)
                        {
                            if (client.Player.IsWalking) continue;
                            client.Player.GoTo = bmage.FoodLocation;
                            continue;
                        }

                        foodTile = client.Map.GetTile(bmage.FoodLocation);
                        if (foodTile == null) break;
                        topItemFood = foodTile.GetTopUseItem(false);
                        if (topItemFood == null) break;
                        bool found = false;
                        foreach (ushort foodID in client.ItemList.Food.All)
                        {
                            if (topItemFood.ID != foodID) continue;

                            foreach (Food food in foods)
                            {
                                if (food.ItemID != foodID) continue;
                                foodEaten = food;
                                break;
                            }
                            found = true;
                            break;
                        }
                        if (!found) break;
                        topItemFood.Use();
                    }

                    if (!client.Player.Connected)
                    {
                        System.IO.File.AppendAllText("bedmagedebug.txt", "disconnected\n");
                        continue;
                    }

                    bmage.GoToBed(foodEaten);

                    break;
                }
                catch (Exception ex)
                {
                    DateTime utcNow = DateTime.UtcNow;
                    System.IO.File.AppendAllText("bedmagedebug.txt", "[" + utcNow.Year + "." + utcNow.Month + "." + utcNow.Day + " " +
                        utcNow.Hour + ":" + utcNow.Minute + ":" + utcNow.Second + "]\n" +
                        ex.Message + "\nSource: " + ex.Source + "\n" + ex.StackTrace + "\n\n");
                }
            }
            #endregion
        }
    }
    public static void Cleanup()
    {
        resetEvent.Set();
    }

    public class Bedmage
    {
        public Bedmage(Client client, uint accountNumber, string password, string characterName,
            string spellName, ushort spellMana, Location bedLocation, Location foodLocation, Location blankRuneLocation,
            Location finishedRunesLocation, bool isInstantSpell = false)
        {
            this.Parent = client;
            this.AccountNumber = accountNumber;
            this.Password = password;
            this.CharacterName = characterName;
            this.TimeOfLastLogin = 0;
            this.SpellName = spellName;
            this.SpellMana = spellMana;
            this.BedLocation = bedLocation;
            this.FoodLocation = foodLocation;
            this.BlankRuneLocation = blankRuneLocation;
            this.FinishedRunesLocation = finishedRunesLocation;
            this.IsInstantSpell = isInstantSpell;
        }

        public Client Parent { get; private set; }
        public uint AccountNumber { get; private set; }
        public string Password { get; private set; }
        public string CharacterName { get; private set; }

        /// <summary>
        /// Gets or sets the spell words, i.e. "adura vita".
        /// </summary>
        public string SpellName { get; set; }
        /// <summary>
        /// Gets or sets the minimum amount of mana for the spell.
        /// </summary>
        public ushort SpellMana { get; set; }
        /// <summary>
        /// Gets or sets the world location of a tile containing food in some way (either food or container with food in it).
        /// </summary>
        public Location FoodLocation { get; set; }
        /// <summary>
        /// Gets or sets the world location of a container containing blank runes.
        /// </summary>
        public Location BlankRuneLocation { get; set; }
        public Location BedLocation { get; set; }
        public Location FinishedRunesLocation { get; set; }
        public bool IsInstantSpell { get; set; }
        /// <summary>
        /// Gets or sets time of last login.
        /// </summary>
        public long TimeOfLastLogin { get; set; }
        /// <summary>
        /// Gets or sets the time needed to regen to SpellMana.
        /// </summary>
        public long TimeNeededToRegen { get; set; }

        public bool Login()
        {
            switch ((Enums.Connection)this.Parent.Memory.ReadByte(this.Parent.Addresses.Misc.Connection))
            {
                default:
                    Thread.Sleep(1000);
                    return this.Login();
                case Enums.Connection.Offline:
                    string dialogTitle = this.Parent.Window.GetCurrentDialogTitle();

                    if (string.IsNullOrEmpty(dialogTitle))
                    {
                        var size = this.Parent.Window.GetWindowSize();
                        this.Parent.Modules.Hotkeys.SendLeftMouseClick(120, size.Height - 215);
                        Thread.Sleep(500);
                        return this.Login();
                    }

                    switch (dialogTitle)
                    {
                        default:
                            this.Parent.Modules.Hotkeys.Press(Keys.Escape);
                            Thread.Sleep(500);
                            return this.Login();
                        case "Enter Game":
                            this.Parent.Modules.Hotkeys.Write(this.AccountNumber.ToString());
                            Thread.Sleep(500);
                            this.Parent.Modules.Hotkeys.Press(Keys.Tab);
                            Thread.Sleep(500);
                            this.Parent.Modules.Hotkeys.Write(this.Password);
                            Thread.Sleep(500);
                            this.Parent.Modules.Hotkeys.Press(Keys.Enter);
                            Thread.Sleep(500);
                            return this.Login();
                        case "Sorry": // bad credentials or some other error
                            this.Parent.Modules.Hotkeys.Press(Keys.Escape);
                            Thread.Sleep(200);
                            return false;
                        case "Select Character":
                            // check if our character exists in the current character list
                            int foundIndex = -1;
                            foreach (CharacterList.Character c in this.Parent.CharacterList.GetCharacters())
                            {
                                if (c.Name != this.CharacterName) continue;

                                foundIndex = c.Index;
                                break;
                            }

                            if (foundIndex == -1)
                            {
                                for (int i = 0; i < 3; i++)
                                {
                                    this.Parent.Modules.Hotkeys.Press(Keys.Escape);
                                    Thread.Sleep(200);
                                }
                                return this.Login();
                            }

                            for (int i = 0; i < this.Parent.CharacterList.Count; i++)
                            {
                                this.Parent.Modules.Hotkeys.Press(Keys.Up);
                                Thread.Sleep(200);
                            }
                            for (int i = 0; i < foundIndex; i++)
                            {
                                this.Parent.Modules.Hotkeys.Press(Keys.Down);
                                Thread.Sleep(200);
                            }
                            this.Parent.Modules.Hotkeys.Press(Keys.Return);
                            Thread.Sleep(500);
                            return this.Login();
                    }
                case Enums.Connection.Online:
                    if (this.Parent.Player.Name == this.CharacterName) return true;
                    while (this.Parent.Player.Connected)
                    {
                        this.Parent.Packets.Logout();
                        Thread.Sleep(500);
                    }
                    return this.Login();
            }
        }
        /// <summary>
        /// Attempts to open a container on the ground. Returns null if unsuccessful.
        /// </summary>
        /// <param name="client">The client to use.</param>
        /// <param name="tiles">A collection of tiles.</param>
        /// <param name="loc">The world location of the container.</param>
        /// <returns></returns>
        public Container OpenContainer(TileCollection tiles, Location loc)
        {
            Tile tile = tiles.GetTile(loc);
            if (tile == null) return null;
            TileObject topItem = tile.GetTopUseItem(false);
            if (topItem == null || !topItem.HasFlag(Enums.ObjectPropertiesFlags.IsContainer)) return null;

            while (!this.Parent.Player.Location.IsAdjacentTo(loc) &&
                    this.Parent.Player.Location != loc)
            {
                Thread.Sleep(500);

                if (this.Parent.Player.IsWalking) continue;

                var tilesToLocation = this.Parent.Player.Location.GetTilesToLocation(this.Parent,
                    loc, tiles, this.Parent.PathFinder, true, true).ToArray();
                if (tilesToLocation.Length == 0) return null;

                // get closest adjacent tile
                Tile playerTile = tiles.GetTile(count: this.Parent.Player.ID);
                if (playerTile == null) break;
                Tile closestTile = tiles.GetClosestNearbyTile(playerTile, tile);
                if (closestTile == null) break;
                this.Parent.Player.GoTo = closestTile.WorldLocation;
            }
            if (!this.Parent.Player.Location.IsAdjacentTo(loc) &&
                this.Parent.Player.Location != loc)
            {
                return null;
            }

            // open container on ground
            Container closedContainer = this.Parent.Inventory.GetFirstClosedContainer();
            if (closedContainer == null) return null;
            for (int i = 0; i < 3; i++)
            {
                topItem.Use();
                for (int j = 0; j < 5; j++)
                {
                    Thread.Sleep(100);
                    if (closedContainer.IsOpen) break;
                }
                if (closedContainer.IsOpen) break;
            }
            if (!closedContainer.IsOpen) return null;

            return closedContainer;
        }
        public bool GoToBed(Food foodEaten = null)
        {
            this.TimeOfLastLogin = Environment.TickCount;

            if (!this.Parent.Player.Connected) return true;

            int oneHour = 1000 * 60 * 60,
                regenTime = foodEaten != null ? (int)(maxRegenTime - foodEaten.RegenerationTime) : 0;
            int timeToSleep = this.Parent.Player.Mana >= this.SpellMana
                ? 1000 * 60 * 45 // wait 45 minutes if no blanks are present
                : (int)Math.Ceiling(((float)(this.SpellMana - this.Parent.Player.Mana) / (float)manaPerHour) * oneHour + 1000 * 3);
            if (regenTime > 0 && timeToSleep > regenTime) timeToSleep = regenTime;
            if (timeToSleep > maxRegenTime) timeToSleep = maxRegenTime;
            this.TimeNeededToRegen = timeToSleep;
            System.IO.File.AppendAllText("bedmagedebug.txt", DateTime.UtcNow.TimeOfDay.ToString() +
                " - " + this.CharacterName + " - sleep time: " + TimeSpan.FromMilliseconds(this.TimeNeededToRegen).ToString() +
                " ate food: " + (foodEaten != null ? "yes" : "no") + "\n");

            TileCollection tiles = null;

            while (!this.Parent.Player.Location.IsAdjacentTo(this.BedLocation))
            {
                Thread.Sleep(500);

                if (this.Parent.Player.IsWalking) continue;
                if (!this.Parent.Player.Location.IsOnScreen(this.BedLocation)) return false;

                tiles = this.Parent.Map.GetTilesOnScreen();
                // find closest adjacent tile
                int closestDistance = 50;
                Location closestLocation = Location.Invalid;
                foreach (Tile adjTile in tiles.GetAdjacentTiles(this.BedLocation))
                {
                    var tilesToAdjLocation = this.Parent.Player.Location.GetTilesToLocation(this.Parent,
                        adjTile.WorldLocation, tiles, this.Parent.PathFinder).ToArray();
                    if (tilesToAdjLocation.Length == 0) continue;

                    if (closestDistance <= tilesToAdjLocation.Length) continue;

                    closestDistance = tilesToAdjLocation.Length;
                    closestLocation = adjTile.WorldLocation;
                }
                if (!closestLocation.IsValid()) return false;
                this.Parent.Player.GoTo = closestLocation;
            }

            tiles = this.Parent.Map.GetTilesOnScreen();
            Tile tile = tiles.GetTile(this.BedLocation);
            if (tile == null) return false;
            TileObject topItem = tile.GetTopUseItem(false);
            if (topItem == null) return false;
            while (this.Parent.Player.Connected)
            {
                topItem.Use();
                Thread.Sleep(500);
            }
            return true;
        }
    }
    public class Food
    {
        public ushort ItemID;
        public uint RegenerationTime;
    }
}