using System;
using System.Threading;

namespace KarelazisBot.Objects
{
    /// <summary>
    /// A class used to hold information about the player.
    /// </summary>
    public class Player : Objects.Creature
    {
        /// <summary>
        /// Constructor for this class.
        /// </summary>
        /// <param name="c">The client associated with the player.</param>
        public Player(Objects.Client c)
            : base(c, 0)
        {
            this.SetBattleListAddress();
        }

        /// <summary>
        /// Sets this player's battlelist memory address.
        /// </summary>
        public void SetBattleListAddress()
        {
            if (!this.Connected) return;

            Creature tempPlayer = this.Client.BattleList.GetPlayer(this.ID);
            if (tempPlayer != null && tempPlayer.BattleListAddress != this.BattleListAddress) this.BattleListAddress = tempPlayer.BattleListAddress;
        }

        public enum Flags
        {
            Poisoned = 1,
            Burning = 1 << 1, // 2^1
            Energized = 1 << 2, // 2^2
            Drunken = 1 << 3, // 2^3 etc
            Manashielded = 1 << 4,
            Paralyzed = 1 << 5,
            Hasted = 1 << 6,
            Battle = 1 << 7
        }

        #region properties
        public uint TibianicPointer
        {
            get { return this.Client.Memory.ReadUInt32(this.Client.Addresses.Misc.TibianicPointer); }
        }
        /// <summary>
        /// Gets whether the player is connected or not.
        /// </summary>
        public bool Connected
        {
            get { return this.Client.Memory.ReadByte(this.Client.Addresses.Misc.Connection) == 8; }
        }
        /// <summary>
        /// Gets the player's ID.
        /// </summary>
        public new uint ID
        {
            get
            {
                return this.Client.Memory.ReadUInt32(this.Client.Addresses.Player.ID);
                //return this.Client.Memory.ReadUInt32(this.TibianicPointer +
                //    this.Client.Addresses.Misc.TibianicOffsetPlayerID);
            }
        }
        /// <summary>
        /// Gets the player's name.
        /// </summary>
        public new string Name
        {
            get
            {
                if (!this.Connected) return "Offline";
                return base.Name;
            }
            set { base.Name = value; }
        }
        /// <summary>
        /// Gets or sets the player's experience points.
        /// </summary>
        public uint Experience
        {
            get { return this.Client.Memory.ReadUInt32(this.Client.Addresses.Player.Experience); }
            set { this.Client.Memory.WriteUInt32(this.Client.Addresses.Player.Experience, value); }
        }
        /// <summary>
        /// Gets or sets the player's level.
        /// </summary>
        public uint Level
        {
            get { return this.Client.Memory.ReadUInt32(this.Client.Addresses.Player.Level); }
            set { this.Client.Memory.WriteUInt32(this.Client.Addresses.Player.Level, value); }
        }
        /// <summary>
        /// Gets or sets the player's level percent.
        /// </summary>
        public int LevelPercent
        {
            get { return this.Client.Memory.ReadInt32(this.Client.Addresses.Player.LevelPercent); }
            set { this.Client.Memory.WriteInt32(this.Client.Addresses.Player.LevelPercent, value); }
        }
        /// <summary>
        /// Gets the player's flags.
        /// </summary>
        public uint CurrentFlags
        {
            get { return this.Client.Memory.ReadUInt32(this.Client.Addresses.Player.Flags); }
        }
        /// <summary>
        /// Gets or sets the player's health.
        /// </summary>
        public ushort Health
        {
            //get { return this.Client.Memory.ReadUInt16(this.TibianicPointer + this.Client.Addresses.Misc.TibianicOffsetHealth); }
            get { return this.Client.Memory.ReadUInt16(this.Client.Addresses.Player.Health); }
            set { this.Client.Memory.WriteUInt16(this.Client.Addresses.Player.Health, value); }
        }
        /// <summary>
        /// Gets or sets the player's maximum health.
        /// </summary>
        public ushort HealthMax
        {
            get { return this.Client.Memory.ReadUInt16(this.Client.Addresses.Player.HealthMax); }
            set { this.Client.Memory.WriteUInt16(this.Client.Addresses.Player.HealthMax, value); }
        }
        /// <summary>
        /// Gets or sets the player's mana.
        /// </summary>
        public ushort Mana
        {
            //get { return this.Client.Memory.ReadUInt16(this.TibianicPointer + this.Client.Addresses.Misc.TibianicOffsetMana); }
            get { return this.Client.Memory.ReadUInt16(this.Client.Addresses.Player.Mana); }
            set { this.Client.Memory.WriteUInt16(this.Client.Addresses.Player.Mana, value); }
        }
        /// <summary>
        /// Gets or sets the player's maximum mana.
        /// </summary>
        public ushort ManaMax
        {
            get { return this.Client.Memory.ReadUInt16(this.Client.Addresses.Player.ManaMax); }
            set { this.Client.Memory.WriteUInt16(this.Client.Addresses.Player.ManaMax, value); }
        }
        /// <summary>
        /// Gets the player's mana percent.
        /// </summary>
        public ushort ManaPercent
        {
            get { return (ushort)((this.Mana * 100) / this.ManaMax); }
        }
        /// <summary>
        /// Gets or sets the player's soul points.
        /// </summary>
        public uint Soul
        {
            get { return this.Client.Memory.ReadUInt32(this.Client.Addresses.Player.SoulPoints); }
            set { this.Client.Memory.WriteUInt32(this.Client.Addresses.Player.SoulPoints, value); }
        }
        /// <summary>
        /// Gets or sets the player's capacity.
        /// </summary>
        public uint Cap
        {
            get
            {
                if (this.Client.TibiaVersion >= 800) return this.Client.Memory.ReadUInt32(this.Client.Addresses.Player.Capacity) / 100;
                else return this.Client.Memory.ReadUInt32(this.Client.Addresses.Player.Capacity);
            }
            set { this.Client.Memory.WriteUInt32(this.Client.Addresses.Player.Capacity, value); }
        }
        /// <summary>
        /// Gets or sets the player's stamina.
        /// </summary>
        public uint Stamina
        {
            get { return this.Client.Memory.ReadUInt32(this.Client.Addresses.Player.Stamina); }
            set { this.Client.Memory.WriteUInt32(this.Client.Addresses.Player.Stamina, value); }
        }
        /// <summary>
        /// Gets or sets the player's GoTo X position.
        /// </summary>
        public ushort GoToX
        {
            get { return this.Client.Memory.ReadUInt16(this.Client.Addresses.Player.GoToX); }
            set { this.Client.Memory.WriteUInt16(this.Client.Addresses.Player.GoToX, value); }
        }
        /// <summary>
        /// Gets or sets the player's GoTo Y position.
        /// </summary>
        public ushort GoToY
        {
            get { return this.Client.Memory.ReadUInt16(this.Client.Addresses.Player.GoToY); }
            set { this.Client.Memory.WriteUInt16(this.Client.Addresses.Player.GoToY, value); }
        }
        /// <summary>
        /// Gets or sets the player's GoTo Z position.
        /// </summary>
        public byte GoToZ
        {
            get { return this.Client.Memory.ReadByte(this.Client.Addresses.Player.GoToZ); }
            set { this.Client.Memory.WriteByte(this.Client.Addresses.Player.GoToZ, value); }
        }
        /// <summary>
        /// Gets or sets the player's GoTo world location.
        /// </summary>
        public Objects.Location GoTo
        {
            get { return new Objects.Location(this.GoToX, this.GoToY, this.GoToZ); }
            set
            {
                this.GoToX = (ushort)value.X;
                this.GoToY = (ushort)value.Y;
                this.GoToZ = (byte)value.Z;
                this.IsWalking = true;
            }
        }
        /// <summary>
        /// Gets or sets the player's target ID (aka red square).
        /// </summary>
        public uint Target
        {
            get { return this.Client.Memory.ReadUInt32(this.Client.Addresses.Player.Target); }
            set { this.Client.Memory.WriteUInt32(this.Client.Addresses.Player.Target, value); }
        }
        /// <summary>
        /// Gets the creature that the player is currently attacking. Returns null if not found.
        /// </summary>
        public Objects.Creature TargetCreature
        {
            get { return this.Client.BattleList.GetAny(this.Target); }
        }
        /// <summary>
        /// Gets or sets the player's following ID (aka green square).
        /// </summary>
        public uint FollowID
        {
            get { return this.Client.Memory.ReadUInt32(this.Client.Addresses.Player.Following); }
            set { this.Client.Memory.WriteUInt32(this.Client.Addresses.Player.Following, value); }
        }
        
        public uint Head
        {
            get { return this.Client.Memory.ReadUInt16(this.BattleListAddress + this.Client.Addresses.BattleList.Distances.OutfitHead); }
        }

        public uint Torso
        {
            get { return this.Client.Memory.ReadUInt16(this.BattleListAddress + this.Client.Addresses.BattleList.Distances.OutfitTorso); }
        }
        /// <summary>
        /// Gets or sets the player's fighting stance.
        /// </summary>
        public Enums.FightStance FightStance
        {
            get { return (Enums.FightStance)this.Client.Memory.ReadByte(this.Client.Addresses.Player.FightStance); }
            set
            {
                if (value != this.FightStance)
                {
                    this.Client.Memory.WriteByte(this.Client.Addresses.Player.FightStance, (byte)value);
                    this.Client.Packets.FightSettings(this.FightMode, value, this.SafeMode);
                }
            }
        }
        /// <summary>
        /// Gets or sets the player's fighting mode.
        /// </summary>
        public Enums.FightMode FightMode
        {
            get { return (Enums.FightMode)this.Client.Memory.ReadByte(this.Client.Addresses.Player.FightMode); }
            set
            {
                if (value != this.FightMode)
                {
                    this.Client.Memory.WriteByte(this.Client.Addresses.Player.FightMode, (byte)value);
                    this.Client.Packets.FightSettings(value, this.FightStance, this.SafeMode);
                }
            }
        }
        /// <summary>
        /// Gets or sets the player's safe mode.
        /// </summary>
        public Enums.SafeMode SafeMode
        {
            get { return (Enums.SafeMode)Client.Memory.ReadByte(this.Client.Addresses.Player.SafeMode); }
            set
            {
                if (value != this.SafeMode)
                {
                    this.Client.Memory.WriteByte(this.Client.Addresses.Player.SafeMode, (byte)value);
                    this.Client.Packets.FightSettings(this.FightMode, this.FightStance, value);
                }
            }
        }
        #endregion

        #region methods
        /// <summary>
        /// Checks whether the player has one or more flags.
        /// </summary>
        /// <param name="playerFlag">The flag(s) to check for.</param>
        /// <returns></returns>
        public bool HasFlag(Flags playerFlag)
        {
            return (this.CurrentFlags & (uint)playerFlag) == (uint)playerFlag;
        }
        /// <summary>
        /// Attempts to make one or more runes.
        /// </summary>
        /// <param name="spellName">The name of the spell.</param>
        /// <param name="runeMana">Minimum amount of mana required to make a rune.</param>
        /// <param name="doMakeUntilMana">Maximum mana to stop recursively making runes.</param>
        /// <param name="makeUntilMana">Whether to recursively make runes.</param>
        public void MakeRune(string spellName, ushort runeMana, bool doMakeUntilMana = false, int makeUntilMana = 0)
        {
            // check if player is connected
            if (!this.Connected) return;

            // check if the player has enough mana for the rune
            if (this.Mana < runeMana) return;

            // check if the player carries any blank runes
            Item blankRune = this.Client.Inventory.GetItem(this.Client.ItemList.Runes.Blank);
            if (blankRune == null) return;

            // check if there is an item in the right hand and move the item
            Item itemRightHand = this.Client.Inventory.GetItemInSlot(Enums.EquipmentSlots.RightHand);
            if (itemRightHand != null)
            {
                Objects.ItemLocation targetItem = this.Client.Inventory.GetFirstSuitableSlot(itemRightHand);
                if (targetItem == null) return;
                itemRightHand.Move(targetItem);
                for (int i = 0; i < 5; i++)
                {
                    if (itemRightHand.WaitForInteraction(1000)) break;
                    targetItem = this.Client.Inventory.GetFirstSuitableSlot(itemRightHand);
                    if (targetItem == null) return;
                    itemRightHand.Move(targetItem);
                }
                Thread.Sleep(500);
            }

            while (true) // initiate loop in case of recursive runemaking
            {
                // find a blank rune and move it to right hand
                blankRune = this.Client.Inventory.GetItem(this.Client.ItemList.Runes.Blank);
                if (blankRune == null) break;
                for (int i = 0; i < 5; i++)
                {
                    blankRune = this.Client.Inventory.GetItem(this.Client.ItemList.Runes.Blank);
                    if (blankRune == null) break;
                    Objects.Item temp = this.Client.Inventory.GetItemInSlot(Enums.EquipmentSlots.RightHand);
                    if (temp != null) break;
                    blankRune.Move(new ItemLocation(Enums.EquipmentSlots.RightHand));
                    if (blankRune.WaitForInteraction(1000)) break;
                }

                Thread.Sleep(1000);

                // say the spell and wait for the rune to transform
                for (int i = 0; i < 5; i++)
                {
                    Item tempItem = this.Client.Inventory.GetItemInSlot(Enums.EquipmentSlots.RightHand);
                    if (tempItem == null) break;
                    if (tempItem.ID != this.Client.ItemList.Runes.Blank) break;
                    this.Client.Packets.Say(spellName);
                    if (tempItem.WaitForInteraction(1000)) break;
                }
                Thread.Sleep(1000);

                // move the made rune back to its container
                // if the source container is full, try to find a free slot elsewhere
                Item transformedRune = this.Client.Inventory.GetItemInSlot(Enums.EquipmentSlots.RightHand);
                if (transformedRune == null) break;
                Objects.ItemLocation toItem = blankRune.GetParent() != null ? blankRune.GetParent().GetFirstEmptySlot() : null;
                if (toItem == null) toItem = this.Client.Inventory.GetFirstSuitableSlot(transformedRune);
                if (toItem == null) break;
                for (int i = 0; i < 5; i++)
                {
                    transformedRune = this.Client.Inventory.GetItemInSlot(Enums.EquipmentSlots.RightHand);
                    if (transformedRune == null) break;
                    transformedRune.Move(toItem);
                    if (transformedRune.WaitForInteraction(1000)) break;
                }
                Thread.Sleep(1000);

                // check if the player should make more runes
                if (doMakeUntilMana && this.Client.Player.Mana > makeUntilMana) continue;
                break;
            }

            // put the item the player carried in his right hand back to its slot
            if (itemRightHand == null) return;
            Item oldRHandItem = itemRightHand;
            while (this.Client.Inventory.GetItemInSlot(Enums.EquipmentSlots.RightHand) == null)
            {
                itemRightHand = this.Client.Inventory.GetItem(oldRHandItem.ID);
                if (itemRightHand == null) break;
                itemRightHand.Move(new ItemLocation(Enums.EquipmentSlots.RightHand));
                if (itemRightHand.WaitForInteraction(1000)) break;
            }
        }
        #endregion
    }
}
