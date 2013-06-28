using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using KarelazisBot.Objects;

namespace KarelazisBot.Modules
{
    public partial class Cavebot
    {
        public class Looter
        {
            #region constructors
            public Looter(Cavebot parent)
            {
                this.Parent = parent;
                this.ResetEvent = new ManualResetEventSlim();
                this.ResetEventMultiUse = new AutoResetEvent(false);
            }
            #endregion

            #region properties
            public Cavebot Parent { get; private set; }
            public bool IsRunning
            {
                get { return this.ResetEvent.IsSet; }
            }
            private Thread DedicatedThread { get; set; }
            private ManualResetEventSlim ResetEvent { get; set; }
            private AutoResetEvent ResetEventMultiUse { get; set; }
            private Map.TileCollection CachedTiles { get; set; }
            private bool Cancel { get; set; }
            #endregion

            #region events
            private delegate void OpenCorpseCalledHandler(Map.TileCollection tileCollection, Objects.Map.Tile tile);
            private event OpenCorpseCalledHandler OpenCorpseCalled;
            private delegate void LootItemsCalledHandler(Objects.Container container, IEnumerable<Loot> loot);
            private event LootItemsCalledHandler LootItemsCalled;

            public delegate void CorpseOpenedHandler(Objects.Container container);
            public event CorpseOpenedHandler CorpseOpened;
            public delegate void LootingFinishedHandler(Objects.Container container);
            public event LootingFinishedHandler LootingFinished;
            public delegate void ErrorOccurredHandler(Exception ex);
            public event ErrorOccurredHandler ErrorOccurred;
            #endregion

            #region methods
            public bool OpenCorpse(Map.TileCollection tileCollection, Objects.Map.Tile tile)
            {
                if (!this.Parent.Client.Player.Location.IsOnScreen(tile.WorldLocation)) return false;

                if (this.DedicatedThread == null || !this.DedicatedThread.IsAlive)
                {
                    this.DedicatedThread = new Thread(this.Run);
                    this.DedicatedThread.Start();
                    this.ResetEventMultiUse.WaitOne();
                }

                if (this.OpenCorpseCalled != null) this.OpenCorpseCalled(tileCollection, tile);
                return true;
            }
            public bool LootItems(Objects.Container container, IEnumerable<Loot> loot, bool async)
            {
                if (!container.IsOpen) return false;

                if (async)
                {
                    //if (this.ResetEvent.IsSet) return false;

                    if (this.DedicatedThread == null || !this.DedicatedThread.IsAlive)
                    {
                        this.DedicatedThread = new Thread(this.Run);
                        this.DedicatedThread.Start();
                        this.ResetEventMultiUse.WaitOne();
                    }

                    if (this.LootItemsCalled != null) this.LootItemsCalled(container, loot);
                    return true;
                }

                this.LootItems(container, loot, this.CachedTiles);
                return true;
            }
            public void CancelExecution()
            {
                if (!this.ResetEvent.IsSet) return;
                this.Cancel = true;
                this.ResetEvent.Set();
            }
            public void UpdateCache(Map.TileCollection tileCollection)
            {
                this.CachedTiles = tileCollection;
                this.ResetEventMultiUse.Set();
            }

            private void Run()
            {
                Map.TileCollection tiles = null;
                Objects.Map.Tile corpseTile = null;
                Objects.Container lootContainer = null;
                Loot[] loots = null;
                Random rand = new Random();

                this.OpenCorpseCalled += new OpenCorpseCalledHandler(delegate(Map.TileCollection tileCollection, Objects.Map.Tile tile)
                {
                    tiles = tileCollection;
                    corpseTile = tile;
                    this.ResetEvent.Set();
                });
                this.LootItemsCalled += new LootItemsCalledHandler(delegate(Objects.Container container, IEnumerable<Loot> loot)
                {
                    lootContainer = container;
                    loots = loot.ToArray();
                    this.ResetEvent.Set();
                });

                // signal parent thread that this thread is ready
                this.ResetEventMultiUse.Set();

                while (true)
                {
                    try
                    {
                        this.Cancel = false;
                        this.ResetEvent.Wait();

                        #region open corpse
                        if (corpseTile != null)
                        {
                            while (!this.Cancel)
                            {
                                this.ResetEventMultiUse.WaitOne();

                                if (this.Cancel || corpseTile == null) break;
                                if (!this.Parent.Client.Player.Location.IsOnScreen(corpseTile.WorldLocation)) break;
                                if (this.Parent.Client.Player.IsWalking) continue;

                                Map.TileObject topItem = corpseTile.GetTopUseItem(false);
                                if (topItem == null || !topItem.HasFlag(Enums.ObjectPropertiesFlags.IsContainer)) break;
                                Objects.Map.Tile playerTile = this.CachedTiles.GetTile(count: this.Parent.Client.Player.ID);
                                if (playerTile == null) break;

                                if (playerTile.WorldLocation.DistanceTo(corpseTile.WorldLocation) >= 2)
                                {
                                    int distance = int.MaxValue;
                                    Objects.Map.Tile bestTile = corpseTile;
                                    foreach (Objects.Map.Tile tile in this.CachedTiles.GetAdjacentTileCollection(corpseTile).GetTiles())
                                    {
                                        if (!tile.IsWalkable()) continue;

                                        var pathToTile = playerTile.WorldLocation.GetTilesToLocation(this.Parent.Client,
                                            tile.WorldLocation, this.CachedTiles, this.Parent.PathFinder, true).ToArray();
                                        if (pathToTile.Length == 0) continue;

                                        if (distance > pathToTile.Length)
                                        {
                                            distance = pathToTile.Length;
                                            bestTile = tile;
                                        }
                                    }

                                    if (bestTile == corpseTile &&
                                        !playerTile.WorldLocation.CanReachLocation(this.Parent.Client, bestTile.WorldLocation))
                                    {
                                        break;
                                    }

                                    this.Parent.Client.Player.GoTo = bestTile.WorldLocation;
                                    continue;
                                }

                                for (int i = 0; i < 2; i++)
                                {
                                    if (this.Cancel) break;
                                    Objects.Container container = this.Parent.Client.Inventory.GetContainer(this.Parent.Client.Inventory.GetClosedContainerNumber());
                                    topItem.Use();
                                    for (int j = 0; j < 5; j++)
                                    {
                                        Thread.Sleep(100);
                                        if (container.IsOpen) break;
                                    }
                                    if (!container.IsOpen) continue;
                                    if (this.CorpseOpened != null) this.CorpseOpened(container);
                                    break;
                                }
                                break;
                            }

                            corpseTile = null;
                        }
                        #endregion

                        #region loot item(s)
                        if (lootContainer != null)
                        {
                            if (!lootContainer.IsOpen || lootContainer.ItemsAmount == 0)
                            {
                                if (!this.Cancel && this.LootingFinished != null) this.LootingFinished(lootContainer);
                                lootContainer = null;
                                continue;
                            }

                            this.LootItems(lootContainer, loots, tiles);

                            if (!this.Cancel && this.LootingFinished != null) this.LootingFinished(lootContainer);

                            lootContainer = null;
                            loots = null;
                        }
                        #endregion

                        this.ResetEvent.Reset();
                    }
                    catch (Exception ex)
                    {
                        if (this.ErrorOccurred != null) this.ErrorOccurred(ex);
                    }
                }
            }
            private void LootItems(Objects.Container lootContainer, IEnumerable<Loot> loots,
                Map.TileCollection tiles)
            {
                Random rand = new Random();
                if (!this.Parent.StopwatchFoodEater.IsRunning) this.Parent.StopwatchFoodEater.Start();
                int index = lootContainer.ItemsAmount - 1, retryCount = 0;
                while (index >= 0 && !this.Cancel)
                {
                    // sanity checks
                    if (lootContainer.ItemsAmount == 0 || !lootContainer.IsOpen) break;
                    if (retryCount >= 3)
                    {
                        retryCount = 0;
                        index--;
                        continue;
                    }

                    // get item
                    Objects.Item item = lootContainer.GetItemInSlot((byte)index);
                    if (item == null)
                    {
                        index--;
                        retryCount = 0;
                        continue;
                    }

                    // check if it's food, eat it if so
                    if (this.Parent.CurrentSettings.EatFood &&
                        this.Parent.StopwatchFoodEater.Elapsed.TotalSeconds > 20 &&
                        this.Parent.Client.ItemList.Food.All.Contains(item.ID))
                    {
                        if (item.Count <= 1) item.Use();
                        else
                        {
                            for (int i = 0; i < Math.Min(item.Count, (ushort)3); i++)
                            {
                                item.Use();
                                Thread.Sleep(rand.Next(200, 325));
                            }
                        }
                        this.Parent.StopwatchFoodEater.Restart();
                        Thread.Sleep(rand.Next(200, 350));
                        index--;
                        continue;
                    }

                    // check if we want to loot this item
                    Loot loot = null;
                    foreach (Loot l in loots)
                    {
                        if (l.ID == item.ID)
                        {
                            loot = l;
                            break;
                        }
                    }
                    if (loot == null)
                    {
                        index--;
                        continue;
                    }

                    // loot this item
                    bool successful = false;
                    switch (loot.Destination)
                    {
                        case Loot.Destinations.Ground:
                            Objects.Map.Tile playerTile = tiles.GetTile(count: this.Parent.Client.Player.ID);
                            if (playerTile == null) break;
                            List<Map.Tile> adjacentTiles = tiles.GetAdjacentTileCollection(playerTile).GetTiles().ToList();
                            adjacentTiles.Shuffle();
                            foreach (Objects.Map.Tile tile in adjacentTiles)
                            {
                                if (!tile.IsWalkable()) continue;

                                item.Move(new Objects.ItemLocation(tile.WorldLocation));
                                successful = true;
                                break;
                            }
                            break;
                        case Loot.Destinations.EmptyContainer:
                            Objects.ItemLocation toItem = this.Parent.Client.Inventory.GetFirstSuitableSlot(item, loot.Index);
                            if (toItem == null) break;
                            item.Move(toItem);
                            successful = true;
                            break;
                    }

                    // if successful, check if it's looted
                    // if it wasn't looted, try again
                    if (successful)
                    {
                        if (!item.WaitForInteraction(800))
                        {
                            retryCount++;
                            continue;
                        }
                        if (this.Parent.ItemLooted != null) this.Parent.ItemLooted(item);
                        if (!this.Parent.CurrentSettings.FastLooting) Thread.Sleep(rand.Next(400, 700));
                    }

                    retryCount = 0;
                    index--;
                }

                if (this.Parent.CurrentSettings.OpenContainers &&
                    !this.Cancel &&
                    lootContainer.IsOpen &&
                    lootContainer.ItemsAmount > 0)
                {
                    bool found = false;
                    foreach (Objects.Item item in lootContainer.GetItems().Reverse())
                    {
                        if (item.HasFlag(Enums.ObjectPropertiesFlags.IsContainer))
                        {
                            for (int i = 0; i < 3; i++)
                            {
                                item.Use();
                                if (item.WaitForInteraction(800)) break;
                            }
                            found = true;
                            break;
                        }
                    }
                    if (found) this.LootItems(lootContainer, loots, this.CachedTiles);
                }
            }
            #endregion
        }
    }
}
