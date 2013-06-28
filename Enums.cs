namespace KarelazisBot
{
    public class Enums
    {
        public static class StatusBar
        {
            public const string YouAreFull = "You are full.",
                YouCanNotUseThisObject = "You can not use this object.",
                YouArePoisoned = "You are poisoned.",
                ThisObjectIsTooHeavy = "This object is too heavy.",
                YouCannotThrowThere = "You cannot throw there.",
                ThereIsNoWay = "There is no way.",
                YouCannotPutMoreObjectsInThisContainer = "You cannot put more objects in this container.";
        }

        public enum MiniMapSpeedValues
        {
            Unexplored = 0,
            Unwalkable = 0xFF
        }

        public enum MiniMapColorValues
        {
            Unexplored = 0,
            /// <summary>
            /// I.e. bush. Unwalkabe.
            /// </summary>
            Foliage = 0x0C,
            /// <summary>
            /// Unwalkable.
            /// </summary>
            Swamp = 0x1E,
            /// <summary>
            /// Unwalkable.
            /// </summary>
            Water = 0x28,
            /// <summary>
            /// Unwalkable.
            /// </summary>
            StoneWall = 0x56,
            /// <summary>
            /// Unwalkable.
            /// </summary>
            UnknownUnwalkable = 0x72,
            /// <summary>
            /// Unwalkable.
            /// </summary>
            Wall = 0xBA,
            /// <summary>
            /// Unwalkable.
            /// </summary>
            Lava = 0xC0,
            /// <summary>
            /// Walkable.
            /// </summary>
            Grass = 0x18,
            /// <summary>
            /// Walkable.
            /// </summary>
            Dirt = 0x79,
            /// <summary>
            /// Walkable.
            /// </summary>
            Path = 0x81,
            /// <summary>
            /// Walkable.
            /// </summary>
            Ice = 0xB3,
            /// <summary>
            /// Walkable.
            /// </summary>
            Sand = 0xCF,
            /// <summary>
            /// Walkable.
            /// </summary>
            FloorChange = 0xD2
        }

        public enum ActionStates
        {
            None = 0,
            LeftMouseButtonDown = 1,
            RightMouseButtonDown = 2,
            BothMouseButtonsDown1 = 3,
            BothMouseButtonsDown2 = 4,
            BothMouseButtonsDown3 = 5,
            MoveItemCrosshair = 6,
            UseItemCrosshair = 7,
            HelpLookingGlass = 9,
            Dialog = 10
        }

        /// <summary>
        /// Represents the client's connection status.
        /// </summary>
        public enum Connection
        {
            Offline = 0,
            ConnectingToLoginServer = 3,
            ConnectingToGameServer = 6,
            Online = 8
        }

        public enum CavebotDatType : byte
        {
            Settings = 1,
            Targetting = 2,
            Loot = 3,
            Waypoints = 4
        }

        public enum MessageType
        {
            Say = 1,
            Whisper = 2,
            Yell = 3,
            PrivateMessage = 4,
            ChannelMessage = 5
        }

        public enum ChatChannel : byte
        {
            Guild = 0x00,
            Party = 0x01,
            Game = 0x04,
            Trade = 0x05,
            OwnPrivate = 0x64
        }

        public enum Direction : byte
        {
            Up = 0,
            Right = 1,
            Down = 2,
            Left = 3,
            UpRight = 5,
            DownRight = 6,
            DownLeft = 7,
            UpLeft = 8
        }

        public enum FluidTypes : byte
        {
            Nothing = 0,
            Water = 1,
            Mana = 7
        }

        public enum EquipmentSlots : byte
        {
            Head = 0x01,
            Neck = 0x02,
            Container = 0x03,
            Torso = 0x04,
            RightHand = 0x05,
            LeftHand = 0x06,
            Legs = 0x07,
            Feet = 0x08,
            Ring = 0x09,
            Ammo = 0x0A
        }

        public enum PartyShield : byte
        {
            None = 0,
            /// <summary>
            /// A party leader inviting the player. Only seen by the player.
            /// </summary>
            Host = 1,
            /// <summary>
            /// An invited player. Only seen by the party leader.
            /// </summary>
            Guest = 2,
            /// <summary>
            /// A party member. Seen by everyone in the party.
            /// </summary>
            Member = 3,
            /// <summary>
            /// The party leader. Seen by everyone in the party and those invited.
            /// </summary>
            Leader = 4
        }

        public enum Skull : byte
        {
            None = 0,
            Yellow = 1,
            Green = 2,
            White = 3,
            Red = 4
        }

        public enum FightMode : byte
        {
            Offensive = 1,
            Balanced = 2,
            Defensive = 3
        }

        public enum FightStance : byte
        {
            Stand = 0,
            Follow = 1,
            FollowDiagonalOnly = 2,
            FollowStrike = 3,
            /// <summary>
            /// Tries to use the surroundings to get hit as little as possible
            /// </summary>
            FollowEconomic = 4,
            /// <summary>
            /// Waits for the target to come in range, useful against monsters who does not run on low health
            /// </summary>
            DistanceWait = 5,
            /// <summary>
            /// Actively stays out of range, useful against monsters who tend to run on low health
            /// </summary>
            DistanceFollow = 6
        }

        public enum SafeMode : byte
        {
            Enabled = 0,
            Disabled = 1
        }

        public enum ObjectPropertiesFlags : uint
        {
            IsGround = 1,
            /// <summary>
            /// Formerly IsTopOrder1
            /// </summary>
            Unknown1 = 1 << 1,//2,
            /// <summary>
            /// Items like mailboxes, doors etc, that always is the item used for crowbars and other tools.
            /// Trumps ground if there are no items when using an item.
            /// Formerly IsTopOrder2.
            /// </summary>
            IsTopUseIfOnly = 1 << 2,//4,
            /// <summary>
            /// Items like arches, which are seated at a height. Trumps ground if there are no items when using an item.
            /// Formerly IsTopOrder3.
            /// </summary>
            IsWalkThrough = 1 << 3,//8,
            IsContainer = 1 << 4, //16,
            IsStackable = 1 << 5,//32,
            /// <summary>
            /// Items like ladders, 7.6+ rope holes etc.
            /// </summary>
            IsAlwaysTopUse = 1 << 6,//64,
            IsUsable = 1 << 7,//128,
            IsFluidContainer = 1 << 8,//256,
            IsSplash = 1 << 9,//512,
            IsBlocking = 1 << 10,//1024,
            IsImmobile = 1 << 11,//2048,
            IsMissileBlocking = 1 << 12,//4096,
            IsPathBlocking = 1 << 13,//8192,
            IsPickupable = 1 << 14,//16384,
            IsHangable = 1 << 15,//32768,
            IsFloorChange = 1 << 16,//65536,
            HasAutomapColor = 1 << 17,//131072,
            HasHelpLens = 1 << 18 //262144
        }

        public enum ObjectProperties772 : byte
        {
            IsGround = 0,
            IsTopOrder1 = 1,
            IsTopOrder2 = 2,
            IsTopOrder3 = 3,
            IsContainer = 4,
            IsStackable = 5,
            IsAlwaysTopUse = 6,
            IsUsable = 7,
            //IsWritable = 8,
            IsFluidContainer = 10,
            IsSplash = 11,
            IsBlocking = 12,
            IsImmobile = 13,
            IsMissileBlocking = 14,
            IsPathBlocking = 15,
            IsPickupable = 16,
            IsHangable = 18,
            IsLightSource = 21,
            IsFloorChange = 23,
            HasAutomapColor = 28,
            HasHelpLens = 29
        }

        public enum SpeechType : byte
        {
            Say = 1,
            Whisper = 2,
            Yell = 3,
            Private = 4,
            Channel = 7
        }

        public enum OutgoingPacketTypes : byte
        {
            LoginServerLogin = 0x01,
            Logout = 0x14,
            GameServerLogin = 0x0A,
            ChatMessage = 0x96,
            Attack = 0xA1,
            Follow = 0xA2,
            Ping = 0x1E,
            Hotkey = 0x83
        }

        public enum IncomingPacketTypes : byte
        {
            // GameServer
            SelfAppear = 0x0A,
            GMAction = 0x0B,
            ErrorMessage = 0x14,
            FyiMessage = 0x15,
            WaitingList = 0x16,
            Ping = 0x1E,
            Death = 0x28,
            CanReportBugs = 0x32,
            MapDescription = 0x64,
            MoveNorth = 0x65,
            MoveEast = 0x66,
            MoveSouth = 0x67,
            MoveWest = 0x68,
            TileUpdate = 0x69,
            TileAddThing = 0x6A,
            TileTransformThing = 0x6B,
            TileRemoveThing = 0x6C,
            CreatureMove = 0x6D,
            ContainerOpen = 0x6E,
            ContainerClose = 0x6F,
            ContainerAddItem = 0x70,
            ContainerUpdateItem = 0x71,
            ContainerRemoveItem = 0x72,
            InventorySetSlot = 0x78,
            InventoryResetSlot = 0x79,
            ShopWindowOpen = 0x7A,
            ShopSaleGoldCount = 0x7B,
            ShopWindowClose = 0x7C,
            SafeTradeRequestAck = 0x7D,
            SafeTradeRequestNoAck = 0x7E,
            SafeTradeClose = 0x7F,
            WorldLight = 0x82,
            MagicEffect = 0x83,
            AnimatedText = 0x84,
            Projectile = 0x85,
            CreatureSquare = 0x86,
            CreatureHealth = 0x8C,
            CreatureLight = 0x8D,
            CreatureOutfit = 0x8E,
            CreatureSpeed = 0x8F,
            CreatureSkull = 0x90,
            CreatureShield = 0x91,
            ItemTextWindow = 0x96,
            HouseTextWindow = 0x97,
            PlayerStatus = 0xA0,
            PlayerSkillsUpdate = 0xA1,
            PlayerFlags = 0xA2,
            CancelTarget = 0xA3,
            CreatureSpeech = 0xAA,
            ChannelList = 0xAB,
            ChannelOpen = 0xAC,
            ChannelOpenPrivate = 0xAD,
            RuleViolationOpen = 0xAE,
            RuleViolationRemove = 0xAF,
            RuleViolationCancel = 0xB0,
            RuleViolationLock = 0xB1,
            PrivateChannelCreate = 0xB2,
            ChannelClosePrivate = 0xB3,
            TextMessage = 0xB4,
            PlayerWalkCancel = 0xB5,
            FloorChangeUp = 0xBE,
            FloorChangeDown = 0xBF,
            OutfitWindow = 0xC8,
            VipState = 0xD2,
            VipLogin = 0xD3,
            VipLogout = 0xD4,
            QuestList = 0xF0,
            QuestPartList = 0xF1,
            ShowTutorial = 0xDC,
            AddMapMarker = 0xDD,
        }

        public enum ClientlessCharacterStatus : byte
        {
            Offline = 0,
            Prepared = 1,
            LoggedIn = 2
        }
    }
}
