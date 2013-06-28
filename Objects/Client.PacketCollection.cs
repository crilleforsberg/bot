using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KarelazisBot.Objects
{
    public partial class Client
    {
        /// <summary>
        /// A class that holds wrappers for sending packets.
        /// </summary>
        public class PacketCollection
        {
            /// <summary>
            /// Constructor for this class.
            /// </summary>
            /// <param name="c">The client that will be used to send packets from.</param>
            public PacketCollection(Objects.Client c)
            {
                this.Client = c;
                this.SetInternalFunctions(c.Addresses);
            }

            /// <summary>
            /// Gets the client associated with this object.
            /// </summary>
            public Objects.Client Client { get; private set; }

            #region events
            public delegate void GenericHandler();
            public event GenericHandler LoggedOut;
            public delegate void TurnedHandler(Enums.Direction direction);
            public event TurnedHandler Turned;
            public delegate void ItemUsedHandler(Objects.ItemLocation itemLocation);
            public event ItemUsedHandler ItemUsed;
            public delegate void ItemUsedOnHandler(Objects.ItemLocation from, Objects.ItemLocation to);
            public event ItemUsedOnHandler ItemUsedOn;
            public delegate void ItemUsedOnBattleListHandler(Objects.ItemLocation itemLocation, Objects.Creature c);
            public event ItemUsedOnBattleListHandler ItemUsedOnBattleList;
            public delegate void ItemMovedHandler(Objects.ItemLocation from, Objects.ItemLocation to);
            public event ItemMovedHandler ItemMoved;
            public delegate void GenericContainerHandler(Objects.Container container);
            public event GenericContainerHandler ContainerClosed;
            public event GenericContainerHandler ContainerOpenedParent;
            public delegate void ChannelMessageSentHandler(string message, Enums.ChatChannel channel);
            public event ChannelMessageSentHandler ChannelMessageSent;
            public delegate void DefaultMessageSentHandler(string message);
            public event DefaultMessageSentHandler DefaultMessageSent;
            public delegate void PrivateMessageSentHandler(string message, string recipient);
            public event PrivateMessageSentHandler PrivateMessageSent;
            public delegate void GenericCreatureHandler(uint id);
            public event GenericCreatureHandler CreatureAttacked;
            public event GenericCreatureHandler CreatureFollowed;
            public delegate void FightSettingsChangedHandler(Enums.FightMode fightMode, Enums.FightStance fightStance, Enums.SafeMode safeMode);
            public event FightSettingsChangedHandler FightSettingsChanged;
            #endregion

            #region Internal functions
            private InternalFunction FunctionLogout { get; set; }
            private InternalFunction FunctionTurnSouth { get; set; }
            private InternalFunction FunctionTurnEast { get; set; }
            private InternalFunction FunctionTurnWest { get; set; }
            private InternalFunction FunctionTurnNorth { get; set; }
            private InternalFunction FunctionUseItem { get; set; }
            private InternalFunction FunctionCloseContainer { get; set; }
            private InternalFunction FunctionVipAdd { get; set; }
            private InternalFunction FunctionVipRemove { get; set; }
            private InternalFunction FunctionMoveItem { get; set; }
            private InternalFunction FunctionUseItemEx { get; set; }
            private InternalFunction FunctionAttack { get; set; }
            private InternalFunction FunctionFollow { get; set; }
            private InternalFunction FunctionPrivateMessage { get; set; }
            private InternalFunction FunctionChannelMessage { get; set; }
            private InternalFunction FunctionSay { get; set; }
            private InternalFunction FunctionFightModes { get; set; }
            private InternalFunction FunctionWalkEast { get; set; }
            private InternalFunction FunctionWalkNorthEast { get; set; }
            private InternalFunction FunctionWalkNorth { get; set; }
            private InternalFunction FunctionWalkNorthWest { get; set; }
            private InternalFunction FunctionWalkWest { get; set; }
            private InternalFunction FunctionWalkSouthWest { get; set; }
            private InternalFunction FunctionWalkSouth { get; set; }
            private InternalFunction FunctionWalkSouthEast { get; set; }
            private InternalFunction FunctionGetObjectProperty { get; set; }
            private InternalFunction FunctionUseItemOnBattleList { get; set; }
            private InternalFunction FunctionOpenParentContainer { get; set; }
            private InternalFunction FunctionPing { get; set; }
            #endregion

            /// <summary>
            /// Sets the addresses for the internal functions.
            /// </summary>
            /// <param name="adrs">The addresses to set.</param>
            public void SetInternalFunctions(Objects.Addresses adrs)
            {
                if (!adrs.IsValid) return;
                InternalCaller caller = new InternalCaller(this.Client);
                this.FunctionLogout = new InternalFunction()
                {
                    Address = (IntPtr)adrs.InternalFunctions.Logout,
                    Caller = caller,
                    CallingConvention = CallConvention.Cdecl
                };
                this.FunctionAttack = new InternalFunction()
                {
                    Address = (IntPtr)adrs.InternalFunctions.Attack,
                    Caller = caller,
                    CallingConvention = CallConvention.Cdecl
                };
                this.FunctionChannelMessage = new InternalFunction()
                {
                    Address = (IntPtr)adrs.InternalFunctions.ChannelMessage,
                    Caller = caller,
                    CallingConvention = CallConvention.Cdecl
                };
                this.FunctionCloseContainer = new InternalFunction()
                {
                    Address = (IntPtr)adrs.InternalFunctions.CloseContainer,
                    Caller = caller,
                    CallingConvention = CallConvention.Cdecl
                };
                this.FunctionFightModes = new InternalFunction()
                {
                    Address = (IntPtr)adrs.InternalFunctions.FightModes,
                    Caller = caller,
                    CallingConvention = CallConvention.Cdecl
                };
                this.FunctionFollow = new InternalFunction()
                {
                    Address = (IntPtr)adrs.InternalFunctions.Follow,
                    Caller = caller,
                    CallingConvention = CallConvention.Cdecl
                };
                this.FunctionMoveItem = new InternalFunction()
                {
                    Address = (IntPtr)adrs.InternalFunctions.MoveItem,
                    Caller = caller,
                    CallingConvention = CallConvention.Cdecl
                };
                this.FunctionPrivateMessage = new InternalFunction()
                {
                    Address = (IntPtr)adrs.InternalFunctions.PrivateMessage,
                    Caller = caller,
                    CallingConvention = CallConvention.Cdecl
                };
                this.FunctionSay = new InternalFunction()
                {
                    Address = (IntPtr)adrs.InternalFunctions.Say,
                    Caller = caller,
                    CallingConvention = CallConvention.Cdecl
                };
                this.FunctionTurnEast = new InternalFunction()
                {
                    Address = (IntPtr)adrs.InternalFunctions.TurnEast,
                    Caller = caller,
                    CallingConvention = CallConvention.Cdecl
                };
                this.FunctionTurnNorth = new InternalFunction()
                {
                    Address = (IntPtr)adrs.InternalFunctions.TurnNorth,
                    Caller = caller,
                    CallingConvention = CallConvention.Cdecl
                };
                this.FunctionTurnSouth = new InternalFunction()
                {
                    Address = (IntPtr)adrs.InternalFunctions.TurnSouth,
                    Caller = caller,
                    CallingConvention = CallConvention.Cdecl
                };
                this.FunctionTurnWest = new InternalFunction()
                {
                    Address = (IntPtr)adrs.InternalFunctions.TurnWest,
                    Caller = caller,
                    CallingConvention = CallConvention.Cdecl
                };
                this.FunctionUseItem = new InternalFunction()
                {
                    Address = (IntPtr)adrs.InternalFunctions.UseItem,
                    Caller = caller,
                    CallingConvention = CallConvention.Cdecl
                };
                this.FunctionUseItemEx = new InternalFunction()
                {
                    Address = (IntPtr)adrs.InternalFunctions.UseItemOn,
                    Caller = caller,
                    CallingConvention = CallConvention.Cdecl
                };
                this.FunctionVipAdd = new InternalFunction()
                {
                    Address = (IntPtr)adrs.InternalFunctions.VipAdd,
                    Caller = caller,
                    CallingConvention = CallConvention.Cdecl
                };
                this.FunctionVipRemove = new InternalFunction()
                {
                    Address = (IntPtr)adrs.InternalFunctions.VipRemove,
                    Caller = caller,
                    CallingConvention = CallConvention.Cdecl
                };
                this.FunctionWalkEast = new InternalFunction()
                {
                    Address = (IntPtr)adrs.InternalFunctions.WalkEast,
                    Caller = caller,
                    CallingConvention = CallConvention.Cdecl
                };
                this.FunctionWalkNorthEast = new InternalFunction()
                {
                    Address = (IntPtr)adrs.InternalFunctions.WalkNorthEast,
                    Caller = caller,
                    CallingConvention = CallConvention.Cdecl
                };
                this.FunctionWalkNorth = new InternalFunction()
                {
                    Address = (IntPtr)adrs.InternalFunctions.WalkNorth,
                    Caller = caller,
                    CallingConvention = CallConvention.Cdecl
                };
                this.FunctionWalkNorthWest = new InternalFunction()
                {
                    Address = (IntPtr)adrs.InternalFunctions.WalkNorthWest,
                    Caller = caller,
                    CallingConvention = CallConvention.Cdecl
                };
                this.FunctionWalkWest = new InternalFunction()
                {
                    Address = (IntPtr)adrs.InternalFunctions.WalkWest,
                    Caller = caller,
                    CallingConvention = CallConvention.Cdecl
                };
                this.FunctionWalkSouthWest = new InternalFunction()
                {
                    Address = (IntPtr)adrs.InternalFunctions.WalkSouthWest,
                    Caller = caller,
                    CallingConvention = CallConvention.Cdecl
                };
                this.FunctionWalkSouth = new InternalFunction()
                {
                    Address = (IntPtr)adrs.InternalFunctions.WalkSouth,
                    Caller = caller,
                    CallingConvention = CallConvention.Cdecl
                };
                this.FunctionWalkSouthEast = new InternalFunction()
                {
                    Address = (IntPtr)adrs.InternalFunctions.WalkSouthEast,
                    Caller = caller,
                    CallingConvention = CallConvention.Cdecl
                };
                this.FunctionGetObjectProperty = new InternalFunction()
                {
                    Address = (IntPtr)adrs.InternalFunctions.GetObjectProperty,
                    Caller = caller,
                    CallingConvention = CallConvention.Cdecl
                };
                this.FunctionUseItemOnBattleList = new InternalFunction()
                {
                    Address = (IntPtr)adrs.InternalFunctions.UseItemOnBattleList,
                    Caller = caller,
                    CallingConvention = CallConvention.Cdecl
                };
                this.FunctionOpenParentContainer = new InternalFunction()
                {
                    Address = (IntPtr)adrs.InternalFunctions.OpenParentContainer,
                    Caller = caller,
                    CallingConvention = CallConvention.Cdecl
                };
                this.FunctionPing = new InternalFunction()
                {
                    Address = (IntPtr)adrs.InternalFunctions.Ping,
                    Caller = caller,
                    CallingConvention = CallConvention.Cdecl
                };
            }

            public void Ping()
            {
                if (!this.Client.Player.Connected) return;
                this.FunctionPing.Call();
            }
            /// <summary>
            /// Attempts to log out.
            /// </summary>
            public void Logout()
            {
                if (!this.Client.Player.Connected) return;
                this.FunctionLogout.Call();
                if (this.LoggedOut != null) this.LoggedOut();
            }
            /// <summary>
            /// Attempts to turn to a given direction.
            /// </summary>
            /// <param name="direction"></param>
            public void Turn(Enums.Direction direction)
            {
                switch (direction)
                {
                    case Enums.Direction.Down:
                        this.FunctionTurnSouth.Call(0x50F);
                        break;
                    case Enums.Direction.Left:
                        this.FunctionTurnWest.Call(0x50D);
                        break;
                    case Enums.Direction.Right:
                        this.FunctionTurnEast.Call(0x50E);
                        break;
                    case Enums.Direction.Up:
                        this.FunctionTurnNorth.Call(0x50C);
                        break;
                    default:
                        return;
                }
                if (this.Turned != null) this.Turned(direction);
            }
            /// <summary>
            /// Attempts to use an item.
            /// </summary>
            /// <param name="itemid">The item's ID to use.</param>
            public void HotkeyUseItem(ushort itemid)
            {
                if (itemid == 0) return;

            }
            /// <summary>
            /// Attempts to use an item on a world location.
            /// </summary>
            /// <param name="itemid">The item's ID.</param>
            /// <param name="loc">The world location to use the item on.</param>
            public void HotkeyUseItemOnLocation(ushort itemid, Objects.Location loc)
            {
                if (itemid == 0 || !loc.IsValid()) return;
            }
            /// <summary>
            /// Attempts to use an item on a creature, player or NPC.
            /// </summary>
            /// <param name="itemid">The item's ID.</param>
            /// <param name="targetid">The ID of the creature, player or NPC to use the item on.</param>
            public void HotkeyUseItemOnCreature(ushort itemid, uint targetid)
            {
                if (itemid == 0 || targetid == 0) return;
                //FunctionHotkeyUseItemOnCreature.Call(0xFFFF, 0, 0, itemid, 0, targetid);
            }
            /// <summary>
            /// Attempts to use an item on the player.
            /// </summary>
            /// <param name="id">The item's ID.</param>
            public void HotkeyUseItemOnSelf(ushort id)
            {
                this.HotkeyUseItemOnCreature(id, this.Client.Player.ID);
            }
            /// <summary>
            /// Attempts to close a container.
            /// </summary>
            /// <param name="c">The container to close.</param>
            public void CloseContainer(Objects.Container c)
            {
                this.FunctionCloseContainer.Call(c.OrderNumber);
                if (this.ContainerClosed != null) this.ContainerClosed(c);
            }
            public void OpenParentContainer(Objects.Container c)
            {
                this.FunctionOpenParentContainer.Call(c.OrderNumber);
                if (this.ContainerOpenedParent != null) this.ContainerOpenedParent(c);
            }
            /// <summary>
            /// Attempts to attack a creature, player or NPC.
            /// </summary>
            /// <param name="c">The Objects.Creature object that represents a creature, player or NPC.</param>
            public void Attack(Objects.Creature c)
            {
                if (c == null) return;
                uint targetID = c.ID;
                if (this.Client.Player.Target == targetID) targetID = 0;
                if (targetID != 0 && this.Client.Player.Z != c.Z) return;
                this.Attack(targetID);
            }
            /// <summary>
            /// Attempts to attack a creature, player or NPC.
            /// </summary>
            /// <param name="id">The ID of the creature, player or NPC to attack.</param>
            public void Attack(uint id)
            {
                this.Client.Memory.WriteUInt32(Client.Addresses.Player.Target, id);
                this.FunctionAttack.Call(id);
                if (this.CreatureAttacked != null) this.CreatureAttacked(id);
            }
            /// <summary>
            /// Attempts to follow a creature, player or NPC.
            /// </summary>
            /// <param name="ID">The ID of the creature, player or NPC.</param>
            public void Follow(uint ID)
            {
                uint id = ID;
                if (this.Client.Player.FollowID == ID) id = 0;
                this.FunctionFollow.Call(id);
                this.Client.Memory.WriteUInt32(this.Client.Addresses.Player.Following, id);
                if (this.CreatureFollowed != null) this.CreatureFollowed(id);
            }
            /// <summary>
            /// Attempts to move an item from one location to another.
            /// </summary>
            /// <param name="fromItem">The item to be moved.</param>
            /// <param name="toItem">The new location for the item.</param>
            public void MoveItem(Objects.ItemLocation fromItem, Objects.ItemLocation toItem)
            {
                if (fromItem == null || toItem == null) return;
                this.FunctionMoveItem.Call(fromItem.WorldLocation.X, fromItem.WorldLocation.Y, fromItem.WorldLocation.Z,
                    fromItem.ItemID, fromItem.StackIndex,
                    toItem.WorldLocation.X, toItem.WorldLocation.Y, toItem.WorldLocation.Z, fromItem.ItemCount);
                if (this.ItemMoved != null) this.ItemMoved(fromItem, toItem);
            }
            /// <summary>
            /// Attempts to use an item.
            /// </summary>
            /// <param name="itemLocation">The item location to use.</param>
            /// <param name="openInNewWindow">Set this to true if you intend to open an item on the ground, or open a carrying container in a new window.</param>
            public void UseItem(Objects.ItemLocation itemLocation, bool openInNewWindow)
            {
                if (itemLocation == null) return;

                this.FunctionUseItem.Call(itemLocation.WorldLocation.X, itemLocation.WorldLocation.Y, itemLocation.WorldLocation.Z,
                    itemLocation.ItemID, itemLocation.StackIndex,
                    !openInNewWindow ?
                        itemLocation.WorldLocation.Y - Constants.Inventory.MinimumContainerNumber :
                        this.Client.Inventory.GetClosedContainerNumber());
                if (this.ItemUsed != null) this.ItemUsed(itemLocation);
            }
            /// <summary>
            /// Attempts to use an item on a world location.
            /// If possible, use UseItemOnTile instead, as it provides better performance.
            /// </summary>
            /// <param name="itemLocation">The item's location.</param>
            /// <param name="loc">The world location to use the item on.</param>
            public void UseItemOnLocation(Objects.ItemLocation itemLocation, Objects.Location loc)
            {
                if (itemLocation == null || !loc.IsValid()) return;
                this.UseItemOnTile(itemLocation, this.Client.Map.GetTile(loc, null));
            }
            /// <summary>
            /// Attempts to use an item on a tile.
            /// </summary>
            /// <param name="itemLocation">The item's location.</param>
            /// <param name="tile">The tile to use the item on.</param>
            public void UseItemOnTile(Objects.ItemLocation itemLocation, Map.Tile tile)
            {
                if (itemLocation == null || tile == null) return;
                Objects.Map.TileObject tileObj = tile.GetTopUseItem(true);
                this.UseItemOnTileObject(itemLocation, tileObj);
            }
            /// <summary>
            /// Attempts to use an item on a tile object.
            /// Use this method with the utmost care, and only send this packet when getting a tile's top item.
            /// </summary>
            /// <param name="itemLocation"></param>
            /// <param name="tileObject"></param>
            public void UseItemOnTileObject(Objects.ItemLocation itemLocation, Map.TileObject tileObject)
            {
                if (itemLocation == null || tileObject == null) return;
                this.FunctionUseItemEx.Call(itemLocation.WorldLocation.X, itemLocation.WorldLocation.Y, itemLocation.WorldLocation.Z,
                    itemLocation.ItemID, itemLocation.StackIndex,
                    tileObject.Parent.WorldLocation.X, tileObject.Parent.WorldLocation.Y, tileObject.Parent.WorldLocation.Z,
                    tileObject.ID, tileObject.StackIndex);
                if (this.ItemUsedOn != null) this.ItemUsedOn(itemLocation, tileObject.ToItemLocation());
            }
            /// <summary>
            /// Attempts to use an item on the battlelist. Useful for shooting runes on creatures.
            /// </summary>
            /// <param name="itemLocation">The item's location.</param>
            /// <param name="c">The creature to use the item on..</param>
            public void UseItemOnBattleList(Objects.ItemLocation itemLocation, Objects.Creature c)
            {
                if (itemLocation == null || c == null) return;
                this.FunctionUseItemOnBattleList.Call(itemLocation.WorldLocation.X, itemLocation.WorldLocation.Y, itemLocation.WorldLocation.Z,
                    itemLocation.ItemID, itemLocation.StackIndex, c.ID);
                if (this.ItemUsedOnBattleList != null) this.ItemUsedOnBattleList(itemLocation, c);
            }
            /// <summary>
            /// Attempts to private message another player.
            /// </summary>
            /// <param name="recipient">The player who will receive the message.</param>
            /// <param name="message">The message to send.</param>
            public void PrivateMessage(string recipient, string message)
            {
                if (message.Length <= 0 || recipient.Length <= 0) return;
                this.FunctionPrivateMessage.Call((byte)Enums.MessageType.PrivateMessage, recipient, message);
                if (this.PrivateMessageSent != null) this.PrivateMessageSent(message, recipient);
            }
            /// <summary>
            /// Attempts to broadcast a message in a channel.
            /// </summary>
            /// <param name="channel">The channel to broadcast in.</param>
            /// <param name="message">The message to broadcast.</param>
            public void ChannelMessage(Enums.ChatChannel channel, string message)
            {
                if (message.Length <= 0) return;
                this.FunctionChannelMessage.Call((byte)Enums.MessageType.ChannelMessage, (byte)channel, message);
                if (this.ChannelMessageSent != null) this.ChannelMessageSent(message, channel);
            }
            /// <summary>
            /// Attempts to say a message in the default channel.
            /// </summary>
            /// <param name="message">The message to say.</param>
            public void Say(string message)
            {
                if (message.Length <= 0) return;
                this.FunctionSay.Call((byte)Enums.MessageType.Say, message);
                if (this.DefaultMessageSent != null) this.DefaultMessageSent(message);
            }
            /// <summary>
            /// Sets the player's battle settings.
            /// </summary>
            /// <param name="fightMode">The fight mode to use.</param>
            /// <param name="fightStance">The fight stance to use.</param>
            /// <param name="safeMode">Whether to use safe mode or not.</param>
            public void FightSettings(Enums.FightMode fightMode, Enums.FightStance fightStance, Enums.SafeMode safeMode)
            {
                this.FunctionFightModes.Call((byte)fightMode, (byte)fightStance, (byte)safeMode);
                if (this.FightSettingsChanged != null) this.FightSettingsChanged(fightMode, fightStance, safeMode);
            }
            /// <summary>
            /// Checks whether an object has a given property. Returns true if so.
            /// </summary>
            /// <param name="id">The object's ID.</param>
            /// <param name="property">The property to check for.</param>
            /// <returns></returns>
            public bool GetObjectProperty(ushort id, byte property)
            {
                uint result = this.FunctionGetObjectProperty.Call(id, property);
                if (result > 1) System.IO.File.AppendAllText("getobjproperty-debug.txt", "expected boolean, got " + property + " using id " + id + "\n");
                return result == 1;
            }
            public void VipAdd(string name)
            {
                this.FunctionVipAdd.Call(name);
            }
            public void VipRemove(uint id)
            {
                this.FunctionVipRemove.Call(id);
            }
        }
    }
}
