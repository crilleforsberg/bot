using System;

namespace KarelazisBot.Objects
{
    public class Addresses
    {
        public Addresses(Objects.Client c)
        {
            this.Client = c;
            this.IsValid = this.SetAddresses(c);
        }

        public Objects.Client Client { get; private set; }
        /// <summary>
        /// Gets whether the memory addresses are supported by the bot or not.
        /// </summary>
        public bool IsValid { get; private set; }

        /// <summary>
        /// Memory addresses related to the player.
        /// </summary>
        public PlayerAddresses Player { get; private set; }
        public MiscAddresses Misc { get; private set; }
        /// <summary>
        /// Memory addresses related to UI dialogs.
        /// </summary>
        public DialogAddresses Dialog { get; private set; }
        /// <summary>
        /// Memory addresses related to the player's inventory.
        /// </summary>
        public ContainersAddresses Containers { get; private set; }
        /// <summary>
        /// Memory addresses related to the character list.
        /// </summary>
        public CharacterListAddresses CharacterList { get; private set; }
        /// <summary>
        /// Memory addresses related to the battlelist.
        /// </summary>
        public BattleListAddresses BattleList { get; private set; }
        /// <summary>
        /// Gets memory addresses related to the user interface.
        /// </summary>
        public UIAddresses UI { get; private set; }
        /// <summary>
        /// Memory addresses related to the in-game map.
        /// </summary>
        public MapAddresses Map { get; private set; }
        /// <summary>
        /// Memory addresses related to the client's internal functions.
        /// </summary>
        public InternalFunctionsAddresses InternalFunctions { get; private set; }
        /// <summary>
        /// Memory addresses related to the framerate.
        /// </summary>
        public FpsAddresses Fps { get; private set; }
        public LoginServerAddresses LoginServer { get; private set; }
        public MiniMapAddresses MiniMap { get; private set; }
        public ItemAddresses Item { get; private set; }
        public VipAddresses Vip { get; private set; }

        /// <summary>
        /// Sets addresses based on Tibia client version. Returns false if unsuccessful.
        /// </summary>
        /// <param name="c">The client to use as reference when setting addresses.</param>
        /// <returns></returns>
        public bool SetAddresses(Objects.Client c)
        {
            int baseAddress = c.TibiaProcess.MainModule.BaseAddress.ToInt32();

            this.Player = new PlayerAddresses();
            this.Misc = new MiscAddresses();
            this.Dialog = new DialogAddresses();
            this.Containers = new ContainersAddresses();
            this.CharacterList = new CharacterListAddresses();
            this.BattleList = new BattleListAddresses();
            this.Map = new MapAddresses();
            this.UI = new UIAddresses();
            this.InternalFunctions = new InternalFunctionsAddresses();
            this.Fps = new FpsAddresses();
            this.LoginServer = new LoginServerAddresses();
            this.MiniMap = new MiniMapAddresses();
            this.Item = new ItemAddresses();
            this.Vip = new VipAddresses();

            switch (c.TibiaVersion)
            {
                case 740:
                    #region 740 addresses
                    /*Player.Flags = 0x00499FA8;
                    Player.FistFightingPercent = Player.Flags + 4;
                    Player.ClubFightingPercent = Player.Flags + 8;
                    Player.SwordFightingPercent = Player.Flags + 12;
                    Player.AxeFightingPercent = Player.Flags + 16;
                    Player.DistanceFightingPercent = Player.Flags + 20;
                    Player.ShieldingPercent = Player.Flags + 24;
                    Player.FishingPercent = Player.Flags + 28;
                    Player.FistFighting = Player.Flags + 32;
                    Player.ClubFighting = Player.Flags + 36;
                    Player.SwordFighting = Player.Flags + 40;
                    Player.AxeFighting = Player.Flags + 44;
                    Player.DistanceFighting = Player.Flags + 48;
                    Player.Shielding = Player.Flags + 52;
                    Player.Fishing = Player.Flags + 56;
                    Player.Capacity = Player.Flags + 72;
                    Player.SoulPoints = 0;
                    Player.ManaMax = Player.Flags + 76;
                    Player.Mana = Player.Flags + 80;
                    Player.MagicLevelPercent = Player.Flags + 84;
                    Player.LevelPercent = Player.Flags + 88;
                    Player.MagicLevel = Player.Flags + 92;
                    Player.Level = Player.Flags + 96;
                    Player.Experience = Player.Flags + 100;
                    Player.HealthMax = Player.Flags + 104;
                    Player.Health = Player.Flags + 108;
                    Player.ID = Player.Flags + 112;
                    Player.GoToZ = Player.Flags + 172;
                    Player.GoToY = Player.Flags + 176;
                    Player.GoToX = Player.Flags + 180;
                    Player.Z = 0x004A4C88;
                    Player.Y = Player.Z + 4;
                    Player.X = Player.Z + 8;
                    Player.SafeMode = 0x005ED3FC;
                    Player.FightStance = Player.SafeMode + 4;
                    Player.FightMode = Player.SafeMode + 8;
                    Player.Target = 0x00499FEC;
                    Player.Following = Player.Target - 4;

                    Misc.Connection = 0x005EFA24;
                    //Misc.StatusBarText = 0x005EFD98;
                    //Misc.StatusBarTime = Misc.StatusBarText - 4;

                    Containers.EqHead = 0x004A2310;
                    Containers.EqNeck = Containers.EqHead + 12;
                    Containers.EqTorso = Containers.EqHead + 36;
                    Containers.EqRightHand = Containers.EqHead + 48;
                    Containers.EqLeftHand = Containers.EqHead + 60;
                    Containers.EqLegs = Containers.EqHead + 72;
                    Containers.EqFeet = Containers.EqHead + 84;
                    Containers.EqRing = Containers.EqHead + 96;
                    Containers.EqAmmo = Containers.EqHead + 108;
                    Containers.Start = 0x004A2388;
                    Containers.Step = 492;
                    Containers.MaxContainers = 16;
                    Containers.MinimumContainerNumber = 0x40;

                    Containers.End = Containers.Start + Containers.Step * Containers.MaxContainers;
                    Containers.ItemStep = 12;
                    ContainerDistances.IsOpen = 0;
                    ContainerDistances.ID = 4;
                    ContainerDistances.Name = 16;
                    ContainerDistances.Slots = 48;
                    ContainerDistances.Amount = 56;

                    CharacterList.Pointer = 0x005EF9F4;
                    CharacterList.PremiumDays = CharacterList.Pointer - 8;
                    CharacterList.SelectedIndex = CharacterList.Pointer - 4;
                    CharacterList.Amount = CharacterList.Pointer + 4;
                    CharacterList.Step = 0x54;
                    CharacterListDistances.CharacterName = 0;
                    CharacterListDistances.ServerName = 30;
                    CharacterListDistances.ServerIP = 64;
                    CharacterListDistances.ServerPort = 80;

                    BattleList.Start = 0x0049A07C - 4;
                    BattleList.Step = 156;
                    BattleList.MaxCreatures = 150;
                    BattleList.End = BattleList.Start + BattleList.Step * BattleList.MaxCreatures;
                    BattleListDistances.ID = 0;
                    BattleListDistances.Type = 3;
                    BattleListDistances.Name = 4;
                    BattleListDistances.X = 36;
                    BattleListDistances.Y = 40;
                    BattleListDistances.Z = 44;
                    BattleListDistances.IsWalking = 76;
                    BattleListDistances.Direction = 80;
                    BattleListDistances.OutfitType = 96;
                    BattleListDistances.OutfitHead = 100;
                    BattleListDistances.OutfitTorso = 104;
                    BattleListDistances.OutfitLegs = 108;
                    BattleListDistances.OutfitFeet = 112;
                    BattleListDistances.Light = 116;
                    BattleListDistances.LightColor = 120;
                    BattleListDistances.LightPattern = 124;
                    BattleListDistances.BlackSquare = 128;
                    BattleListDistances.HealthPercent = 132;
                    BattleListDistances.WalkingSpeed = 136;
                    BattleListDistances.IsVisible = 140;
                    BattleListDistances.Skull = 144;
                    BattleListDistances.Party = 148;

                    GameWindowMessages.Start = 0x005EFEC0;
                    GameWindowMessages.LineStep = 40;
                    GameWindowMessages.Step = 448;
                    GameWindowMessageDistances.LineCount = -4;
                    GameWindowMessageDistances.LineText = 0;
                    GameWindowMessageDistances.X = -16;
                    GameWindowMessageDistances.Y = -12;
                    GameWindowMessageDistances.Z = -8;
                    GameWindowMessageDistances.MessageType = -20;
                    GameWindowMessageDistances.Time = -24;

                    Map.LevelSpy1 = 0x00447186;
                    Map.LevelSpy2 = 0x004472AF;
                    Map.LevelSpy3 = 0x00447247;
                    Map.LevelSpyPointer = 0x004A1AE4;
                    // 23142c0, offsets: 1C
                    Map.LevelSpyOriginalBytes1 = new byte[] { 0x89, 0x87, 0xD8, 0x25, 0x00, 0x00 };
                    Map.LevelSpyOriginalBytes2 = new byte[] { 0x89, 0x98, 0xD8, 0x25, 0x00, 0x00 };
                    Map.LevelSpyOriginalBytes3 = new byte[] { 0x89, 0x98, 0xD8, 0x25, 0x00, 0x00 };
                    Map.LevelSpyOffset2 = 0x25D8;
                    Map.LevelSpyOffset1 = 0x1C;

                    Map.Pointer = 0x004A81C0;
                    Map.TileStep = 168;
                    Map.ObjectStep = 12;
                    MapDistances.TileObjectsCount = 0;
                    MapDistances.ObjectID = 0;
                    MapDistances.ObjectData = 4;
                    MapDistances.ObjectDataEx = 8;

                    InternalFunctions.MainFunction = 0x00448C50;
                    InternalFunctions.TurnNorth = 0x00401100;
                    InternalFunctions.TurnEast = 0x00401110;
                    InternalFunctions.TurnSouth = 0x00401120;
                    InternalFunctions.TurnWest = 0x00401130;
                    InternalFunctions.VipAdd = 0x004019D0;
                    InternalFunctions.Say = 0x00401520;
                    InternalFunctions.UseItem = 0x00401290;
                    InternalFunctions.UseItemOn = 0x004012F0;
                    InternalFunctions.Attack = 0x004017D0;
                    InternalFunctions.Follow = 0x00401800;
                    InternalFunctions.Logout = 0x004010F0;
                    InternalFunctions.PrivateMessage = 0x004015A0;
                    InternalFunctions.ChannelMessage = 0x00401630;
                    InternalFunctions.MoveItem = 0x00401140;
                    InternalFunctions.FightModes = 0x00401790;
                    InternalFunctions.CloseContainer = 0x00401430;
                    InternalFunctions.WalkEast = 0x004043F8;
                    InternalFunctions.WalkNorthEast = InternalFunctions.WalkEast + 0x10;//0x00404418;
                    InternalFunctions.WalkNorth = InternalFunctions.WalkEast + 0x20;//0x00404428;
                    InternalFunctions.WalkNorthWest = InternalFunctions.WalkEast + 0x30;
                    InternalFunctions.WalkWest = InternalFunctions.WalkEast + 0x40;
                    InternalFunctions.WalkSouthWest = InternalFunctions.WalkEast + 0x50;
                    InternalFunctions.WalkSouth = InternalFunctions.WalkEast + 0x60;
                    InternalFunctions.WalkSouthEast = InternalFunctions.WalkEast + 0x70;*/
                    #endregion
                    return false;
                case 770:
                    #region 770 addresses
                    /*Player.Flags = 0x0056C7D8;
                    Player.FistFightingPercent = Player.Flags + 4;
                    Player.ClubFightingPercent = Player.Flags + 8;
                    Player.SwordFightingPercent = Player.Flags + 12;
                    Player.AxeFightingPercent = Player.Flags + 16;
                    Player.DistanceFightingPercent = Player.Flags + 20;
                    Player.ShieldingPercent = Player.Flags + 24;
                    Player.FishingPercent = Player.Flags + 28;
                    Player.FistFighting = Player.Flags + 32;
                    Player.ClubFighting = Player.Flags + 36;
                    Player.SwordFighting = Player.Flags + 40;
                    Player.AxeFighting = Player.Flags + 44;
                    Player.DistanceFighting = Player.Flags + 48;
                    Player.Shielding = Player.Flags + 52;
                    Player.Fishing = Player.Flags + 56;
                    Player.Capacity = Player.Flags + 72;
                    //Player.Soul = Player.Flags + 76;
                    Player.ManaMax = Player.Flags + 80;
                    Player.Mana = Player.Flags + 84;
                    Player.MagicLevelPercent = Player.Flags + 88;
                    Player.LevelPercent = Player.Flags + 92;
                    Player.MagicLevel = Player.Flags + 96;
                    Player.Level = Player.Flags + 100;
                    Player.Experience = Player.Flags + 104;
                    Player.HealthMax = Player.Flags + 108;
                    Player.Health = Player.Flags + 112;
                    Player.ID = Player.Flags + 116;
                    Player.Z = 0x005776E8 - 8;
                    Player.Y = Player.Z + 4;
                    Player.X = Player.Z + 8;
                    Player.GoToZ = Player.Flags + 176;
                    Player.GoToY = Player.Flags + 180;
                    Player.GoToX = Player.Flags + 184;
                    Player.LastClickedItem = 0x006C2684;
                    Player.Target = 0x0056C81C;
                    Player.Following = Player.Target - 4;

                    Misc.Connection = 0x006C25E8;
                    Misc.StatusBarText = 0x006C3C40;
                    Misc.StatusBarTime = Misc.StatusBarText - 4;
                    //Client.LastClickedItemID = 0x006C2684;
                    //Client.FPSPointer = 0x006C312C;
                    //Client.FPSCurrentLimitOffset = 0x68;
                    //Client.FPSCurrentFPSOffset = 0x68 - 12;

                    Map.Pointer = 0x0057AC18;
                    Map.TileStep = 172;
                    Map.ObjectStep = 12;
                    MapDistances.TileObjectsCount = 0;
                    MapDistances.ObjectID = 0;
                    MapDistances.ObjectData = 4;
                    MapDistances.ObjectDataEx = 8;

                    BattleList.Start = 0x0056C8B0;
                    BattleList.Step = 156;
                    BattleList.End = BattleList.Start + BattleList.Step * 250;

                    Containers.Start = 0x00574DD8;
                    Containers.Step = 492;
                    Containers.End = Containers.Start + Containers.Step * Containers.MaxContainers;
                    Containers.EqAmmo = Containers.Start - 12;
                    Containers.EqRing = Containers.Start - 24;
                    Containers.EqFeet = Containers.Start - 36;
                    Containers.EqLegs = Containers.Start - 48;
                    Containers.EqLeftHand = Containers.Start - 60;
                    Containers.EqRightHand = Containers.Start - 72;
                    Containers.EqNeck = Containers.Start - 100;
                    Containers.EqHead = Containers.Start - 112;

                    CharacterList.Pointer = 0x0042C25A;
                    CharacterList.SelectedIndex = CharacterList.Pointer - 4;
                    CharacterList.Amount = CharacterList.Pointer + 4;

                    InternalFunctions.MainFunction = 0x0049F4E0;
                    InternalFunctions.Say = 0x004061E0;
                    InternalFunctions.UseItem = 0x004057F0;
                    InternalFunctions.UseItemOn = 0x00405920;
                    InternalFunctions.MoveItem = 0x00405290;
                    InternalFunctions.Attack = 0x00407260;
                    InternalFunctions.Follow = 0x00407350;
                    InternalFunctions.ChannelMessage = 0x00406830;
                    InternalFunctions.PrivateMessage = 0x004064A0;
                    InternalFunctions.VipAdd = 0x004081B0;
                    InternalFunctions.Logout = 0x00404D50;*/
                    #endregion
                    return false;
                case 772:
                    #region 772 addresses
                    this.Player.Flags = 0x005C67D8;
                    this.Player.FistFightingPercent = this.Player.Flags + 4;
                    this.Player.ClubFightingPercent = this.Player.Flags + 8;
                    this.Player.SwordFightingPercent = this.Player.Flags + 12;
                    this.Player.AxeFightingPercent = this.Player.Flags + 16;
                    this.Player.DistanceFightingPercent = this.Player.Flags + 20;
                    this.Player.ShieldingPercent = this.Player.Flags + 24;
                    this.Player.FishingPercent = this.Player.Flags + 28;
                    this.Player.FistFighting = this.Player.Flags + 32;
                    this.Player.ClubFighting = this.Player.Flags + 36;
                    this.Player.SwordFighting = this.Player.Flags + 40;
                    this.Player.AxeFighting = this.Player.Flags + 44;
                    this.Player.DistanceFighting = this.Player.Flags + 48;
                    this.Player.Shielding = this.Player.Flags + 52;
                    this.Player.Fishing = this.Player.Flags + 56;
                    this.Player.Capacity = this.Player.Flags + 72;
                    //this.Player.Soul = this.Player.Flags + 76;
                    this.Player.ManaMax = this.Player.Flags + 80;
                    this.Player.Mana = this.Player.Flags + 84;
                    this.Player.MagicLevelPercent = this.Player.Flags + 88;
                    this.Player.LevelPercent = this.Player.Flags + 92;
                    this.Player.MagicLevel = this.Player.Flags + 96;
                    this.Player.Level = this.Player.Flags + 100;
                    this.Player.Experience = this.Player.Flags + 104;
                    this.Player.HealthMax = this.Player.Flags + 108;
                    this.Player.Health = this.Player.Flags + 112;
                    this.Player.ID = this.Player.Flags + 116;
                    this.Player.Z = 0x005D16E8;
                    this.Player.Y = this.Player.Z + 4;
                    this.Player.X = this.Player.Z + 8;
                    this.Player.GoToZ = this.Player.Flags + 176;
                    this.Player.GoToY = this.Player.GoToZ + 4;
                    this.Player.GoToX = this.Player.GoToZ + 8;
                    this.Player.Target = 0x005c681c;
                    this.Player.LastClickedItem = 0x0071C624;
                    this.Player.SafeMode = 0x719E84;
                    this.Player.FightStance = this.Player.SafeMode + 4;
                    this.Player.FightMode = this.Player.SafeMode + 8;

                    this.BattleList.Start = 0x005C68B0;
                    this.BattleList.Step = 156;
                    this.BattleList.MaxCreatures = 150;
                    this.BattleList.End = this.BattleList.Start + this.BattleList.Step * this.BattleList.MaxCreatures;
                    this.BattleList.Distances.ID = 0;
                    this.BattleList.Distances.Type = 3;
                    this.BattleList.Distances.Name = 4;
                    this.BattleList.Distances.X = 36;
                    this.BattleList.Distances.Y = 40;
                    this.BattleList.Distances.Z = 44;
                    this.BattleList.Distances.TileOffsetHorizontal = 48;
                    this.BattleList.Distances.TileOffsetVertical = 52;
                    this.BattleList.Distances.IsWalking = 76;
                    this.BattleList.Distances.Direction = 80;
                    this.BattleList.Distances.OutfitType = 96;
                    this.BattleList.Distances.OutfitHead = 100;
                    this.BattleList.Distances.OutfitTorso = 104;
                    this.BattleList.Distances.OutfitLegs = 108;
                    this.BattleList.Distances.OutfitFeet = 112;
                    this.BattleList.Distances.Light = 116;
                    this.BattleList.Distances.LightColor = 120;
                    this.BattleList.Distances.LightPattern = 124;
                    this.BattleList.Distances.BlackSquare = 128;
                    this.BattleList.Distances.HealthPercent = 132;
                    this.BattleList.Distances.WalkingSpeed = 136;
                    this.BattleList.Distances.IsVisible = 140;
                    this.BattleList.Distances.Skull = 144;
                    this.BattleList.Distances.Party = 148;

                    this.Containers.Start = 0x005CEDD8;
                    this.Containers.Step = 492;
                    this.Containers.End = this.Containers.Start + this.Containers.Step * Constants.Inventory.MaxContainers;
                    this.Containers.EqAmmo = this.Containers.Start - 12;
                    this.Containers.EqRing = this.Containers.Start - 24;
                    this.Containers.EqFeet = this.Containers.Start - 36;
                    this.Containers.EqLegs = this.Containers.Start - 48;
                    this.Containers.EqLeftHand = this.Containers.Start - 60;
                    this.Containers.EqRightHand = this.Containers.Start - 72;
                    this.Containers.EqContainer = this.Containers.Start - 96;
                    this.Containers.EqNeck = this.Containers.Start - 100;
                    this.Containers.EqHead = this.Containers.Start - 112;
                    this.Containers.ItemStep = 12;
                    this.Containers.Distances.IsOpen = 0;
                    this.Containers.Distances.ID = 4;
                    this.Containers.Distances.Name = 16;
                    this.Containers.Distances.Slots = 48;
                    this.Containers.Distances.HasParent = 52;
                    this.Containers.Distances.Amount = 56;
                    this.Containers.Distances.FirstItem = 60;

                    this.Item.Distances.ID = 0;
                    this.Item.Distances.Count = 4;

                    this.CharacterList.Pointer = 0x0071C54C;
                    this.CharacterList.PremiumDays = this.CharacterList.Pointer - 8;
                    this.CharacterList.SelectedIndex = this.CharacterList.Pointer - 4;
                    this.CharacterList.Amount = this.CharacterList.Pointer + 4;
                    this.CharacterList.Step = 0x54;
                    this.CharacterList.Distances.CharacterName = 0;
                    this.CharacterList.Distances.ServerName = 30;
                    this.CharacterList.Distances.ServerIP = 64;
                    this.CharacterList.Distances.ServerPort = 80;

                    this.Dialog.Pointer = 0x005D16AC;
                    this.Dialog.Distances.Title = 0x50;

                    this.Map.Pointer = 0x005D4C20;
                    this.Map.TileStep = 172;
                    this.Map.ObjectStep = 12;
                    this.Map.Distances.TileObjectsCount = 0;
                    this.Map.Distances.ObjectID = 0;
                    this.Map.Distances.ObjectData = 4;
                    this.Map.Distances.ObjectDataEx = 8;

                    this.Map.TileNumberPointer = 0x005D4C3C;

                    this.Map.NameSpy1 = 0x004C650B;
                    this.Map.NameSpy2 = 0x004C6515;
                    this.Map.NameSpyOriginalBytes1 = BitConverter.GetBytes((ushort)19573);
                    this.Map.NameSpyOriginalBytes2 = BitConverter.GetBytes((ushort)17013);

                    this.Map.LevelSpy1 = 0x004C7740;
                    this.Map.LevelSpy2 = 0x004C7819;
                    this.Map.LevelSpy3 = 0x004C7884;
                    this.Map.LevelSpyPointer = 0x005CE514;
                    this.Map.LevelSpyOriginalBytes1 = new byte[] { 0x89, 0x83, 0xD8, 0x25, 0x00, 0x00 };
                    this.Map.LevelSpyOriginalBytes2 = new byte[] { 0x89, 0x83, 0xD8, 0x25, 0x00, 0x00 };
                    this.Map.LevelSpyOriginalBytes3 = new byte[] { 0x89, 0x83, 0xD8, 0x25, 0x00, 0x00 };
                    this.Map.LevelSpyOffset1 = 0x1C;
                    this.Map.LevelSpyOffset2 = 0x25D8;

                    this.MiniMap.Start = 0x005D4C40;
                    this.MiniMap.End = this.MiniMap.Start + this.MiniMap.MaxEntries * this.MiniMap.Step;

                    this.UI.WindowHeight = 0x0071C5DC;
                    this.UI.WindowWidth = this.UI.WindowHeight + 4;
                    this.UI.ActionState = 0x0071C5E8;
                    this.UI.LastRightClickedItem = 0x0071C630;
                    this.UI.LastRightClickedItemX = 0x0071C598;
                    this.UI.LastRightClickedItemY = this.UI.LastRightClickedItemX - 4;
                    this.UI.LastRightClickedItemZ = this.UI.LastRightClickedItemX - 8;
                    this.UI.StatusBarText = 0x0071DBE0;
                    this.UI.StatusBarTime = this.UI.StatusBarText - 4;

                    this.UI.GameWindow.Pointer = 0x005D16B0;
                    this.UI.GameWindow.Offset1 = 0x2C;
                    this.UI.GameWindow.Offset2 = 0x30;

                    this.UI.GameWindow.Messages.NextIndex = 0x0071DDE0;
                    this.UI.GameWindow.Messages.Start = this.UI.GameWindow.Messages.NextIndex + 8;//0x0071DE08;
                    this.UI.GameWindow.Messages.Distances.LineCount = 28;//-4;
                    this.UI.GameWindow.Messages.Distances.LineText = 32;//0;
                    this.UI.GameWindow.Messages.Distances.X = 16;//-16;
                    this.UI.GameWindow.Messages.Distances.Y = 20;//-12;
                    this.UI.GameWindow.Messages.Distances.Z = 24;//-8;
                    this.UI.GameWindow.Messages.Distances.MessageType = 12;//-20;
                    this.UI.GameWindow.Messages.Distances.Time = 8;//-24;
                    this.UI.GameWindow.Messages.Distances.Index = 4;//-28;
                    this.UI.GameWindow.Messages.Distances.IsVisible = 0;//-32;

                    this.LoginServer.Start = 0x007152F8;
                    this.LoginServer.Step = 112;
                    this.LoginServer.End = this.LoginServer.Start + this.LoginServer.Step * 5;
                    this.LoginServer.Distances.IP = 0;
                    this.LoginServer.Distances.Port = 100;

                    this.Misc.Connection = 0x0071C588;
                    this.Misc.Time = 0x005CE518;
                    this.Misc.TibianicPointer = 0x0044BBF4;
                    this.Misc.TibianicOffsetPlayerID = 0;
                    this.Misc.TibianicOffsetContainerIsOpen = 0x14;
                    this.Misc.TibianicOffsetMana = -8;
                    this.Misc.TibianicOffsetHealth = -4;
                    //Client.XTEAKey = 0x00719D78;

                    this.Fps.Pointer = 0x0071D0CC;
                    this.Fps.LimitOffset = 0x58;
                    this.Fps.CurrentFramerateOffset = 0x60;

                    this.Vip.Start = 0x005C4570;
                    this.Vip.End = this.Vip.Start + this.Vip.Step * this.Vip.MaxEntries;
                    this.Vip.Count = this.Vip.Start + 0x2264;

                    this.InternalFunctions.MainFunction = 0x004CBE00; //404590, ping call: 40c6cb
                    this.InternalFunctions.Say = 0x004067C0;
                    this.InternalFunctions.VipAdd = 0x00409B00;
                    this.InternalFunctions.VipRemove = 0x00409D90;
                    this.InternalFunctions.UseItem = 0x004056F0;
                    this.InternalFunctions.UseItemOn = 0x004058F0;
                    this.InternalFunctions.UseItemOnBattleList = 0x00405B20;
                    this.InternalFunctions.MoveItem = 0x00404DB0;
                    this.InternalFunctions.Attack = 0x00408080;
                    this.InternalFunctions.Follow = 0x00408230;
                    this.InternalFunctions.PrivateMessage = 0x00406B40;
                    this.InternalFunctions.ChannelMessage = 0x00406F90;
                    this.InternalFunctions.Logout = 0x00404350;
                    this.InternalFunctions.CloseContainer = 0x00405EF0;
                    this.InternalFunctions.OpenParentContainer = 0x004060A0;
                    this.InternalFunctions.TurnEast = 0x004048D0;
                    this.InternalFunctions.TurnNorth = 0x00404730;
                    this.InternalFunctions.TurnSouth = 0x00404A70;
                    this.InternalFunctions.TurnWest = 0x00404C10;
                    this.InternalFunctions.FightModes = 0x00407EB0;
                    this.InternalFunctions.GetObjectProperty = 0x004D1C40;
                    this.InternalFunctions.Ping = 0x00404590;
                    this.Misc.PingCall = 0x0040C6CB;
                    #endregion
                    return true;
            }

            return false;
        }

        #region Nested address type classes
        public class LoginServerAddresses
        {
            public LoginServerAddresses()
            {
                this.Distances = new DistanceAddresses();
            }

            public int Start { get; set; }
            public int Step { get; set; }
            public int End { get; set; }
            public DistanceAddresses Distances { get; private set; }

            public class DistanceAddresses
            {
                public int IP { get; set; }
                public int Port { get; set; }
            }
        }
        public class PlayerAddresses
        {
            public int Flags { get; set; }
            public int ID { get; set; }
            public int Experience { get; set; }
            public int Level { get; set; }
            public int LevelPercent { get; set; }
            public int X { get; set; }
            public int Y { get; set; }
            public int Z { get; set; }
            public int Capacity { get; set; }
            public int SoulPoints { get; set; }
            public int Stamina { get; set; }
            public int Health { get; set; }
            public int HealthMax { get; set; }
            public int Mana { get; set; }
            public int ManaMax { get; set; }
            public int MagicLevel { get; set; }
            public int MagicLevelPercent { get; set; }
            public int FistFighting { get; set; }
            public int FistFightingPercent { get; set; }
            public int ClubFighting { get; set; }
            public int ClubFightingPercent { get; set; }
            public int SwordFighting { get; set; }
            public int SwordFightingPercent { get; set; }
            public int AxeFighting { get; set; }
            public int AxeFightingPercent { get; set; }
            public int DistanceFighting { get; set; }
            public int DistanceFightingPercent { get; set; }
            public int Shielding { get; set; }
            public int ShieldingPercent { get; set; }
            public int Fishing { get; set; }
            public int FishingPercent { get; set; }
            public int GoToX { get; set; }
            public int GoToY { get; set; }
            public int GoToZ { get; set; }
            public int Target { get; set; }
            public int Following { get; set; }
            public int SafeMode { get; set; }
            public int FightStance { get; set; }
            public int FightMode { get; set; }
            public int LastClickedItem { get; set; }
            public int LastLookedAtItem { get; set; }
        }

        public class MiscAddresses
        {
            public int Connection { get; set; }
            public int Time { get; set; }
            public int TibianicPointer { get; set; }
            public int TibianicOffsetPlayerID { get; set; }
            public int TibianicOffsetContainerIsOpen { get; set; }
            public int TibianicOffsetMana { get; set; }
            public int TibianicOffsetHealth { get; set; }

            public int PingCall { get; set; }
            public byte[] PingCallOriginal = new byte[] { 0xE8, 0xC0, 0x7E, 0xFF, 0xFF };
            public byte[] PingCallNop = new byte[] { 0x90, 0x90, 0x90, 0x90, 0x90 };
        }

        public class FpsAddresses
        {
            public int Pointer { get; set; }
            public int LimitOffset { get; set; }
            public int CurrentFramerateOffset { get; set; }
        }

        public class DialogAddresses
        {
            public DialogAddresses()
            {
                this.Distances = new DistanceAddresses();
            }

            public int Pointer { get; set; }
            public DistanceAddresses Distances { get; private set; }

            public class DistanceAddresses
            {
                public int Title { get; set; }
            }
        }

        public class ContainersAddresses
        {
            public ContainersAddresses()
            {
                this.Distances = new DistanceAddresses();
            }

            public DistanceAddresses Distances { get; private set; }

            public int EqAmmo { get; set; }
            public int EqHead { get; set; }
            public int EqFeet { get; set; }
            public int EqTorso { get; set; }
            public int EqRightHand { get; set; }
            public int EqLeftHand { get; set; }
            public int EqRing { get; set; }
            public int EqLegs { get; set; }
            public int EqNeck { get; set; }
            public int EqContainer { get; set; }

            public int Start { get; set; }
            public int End { get; set; }
            public int Step { get; set; }
            public int ItemStep { get; set; }

            public class DistanceAddresses
            {
                public int IsOpen { get; set; }
                public int ID { get; set; }
                public int Name { get; set; }
                public int Slots { get; set; }
                public int HasParent { get; set; }
                public int Amount { get; set; }
                public int FirstItem { get; set; }
            }
        }

        public class ItemAddresses
        {
            public ItemAddresses()
            {
                this.Distances = new DistanceAddresses();
            }

            public DistanceAddresses Distances { get; private set; }

            public class DistanceAddresses
            {
                public int ID { get; set; }
                public int Count { get; set; }
            }
        }

        public class CharacterListAddresses
        {
            public CharacterListAddresses()
            {
                this.Distances = new DistanceAddresses();
            }

            public DistanceAddresses Distances { get; private set; }

            public int Pointer { get; set; }
            public int SelectedIndex { get; set; }
            public int Amount { get; set; }
            public int PremiumDays { get; set; }
            public int Step { get; set; }

            public class DistanceAddresses
            {
                public int CharacterName { get; set; }
                public int ServerName { get; set; }
                public int ServerIP { get; set; }
                public int ServerPort { get; set; }
            }
        }

        public class BattleListAddresses
        {
            public BattleListAddresses()
            {
                this.Distances = new DistanceAddresses();
            }

            public DistanceAddresses Distances { get; private set; }

            public int Start { get; set; }
            public int End { get; set; }
            public int Step { get; set; }
            public int MaxCreatures { get; set; }

            public class DistanceAddresses
            {
                public int ID { get; set; }
                public int Type { get; set; }
                public int Name { get; set; }
                public int X { get; set; }
                public int Y { get; set; }
                public int Z { get; set; }
                public int TileOffsetHorizontal { get; set; }
                public int TileOffsetVertical { get; set; }
                public int IsWalking { get; set; }
                public int Direction { get; set; }
                public int OutfitType { get; set; }
                public int OutfitHead { get; set; }
                public int OutfitTorso { get; set; }
                public int OutfitLegs { get; set; }
                public int OutfitFeet { get; set; }
                public int Light { get; set; }
                public int LightColor { get; set; }
                public int LightPattern { get; set; }
                public int BlackSquare { get; set; }
                public int HealthPercent { get; set; }
                public int WalkingSpeed { get; set; }
                public int IsVisible { get; set; }
                public int Skull { get; set; }
                public int Party { get; set; }
            }
        }

        public class UIAddresses
        {
            public UIAddresses()
            {
                this.GameWindow = new GameWindowAddresses();
            }

            public GameWindowAddresses GameWindow { get; private set; }
            public int WindowWidth { get; set; }
            public int WindowHeight { get; set; }
            /// <summary>
            /// Specifies what the current action state is. See Enums.ActionStates for more information.
            /// </summary>
            public int ActionState { get; set; }
            public int StatusBarText { get; set; }
            public int StatusBarTime { get; set; }
            public int LastRightClickedItemX { get; set; }
            public int LastRightClickedItemY { get; set; }
            public int LastRightClickedItemZ { get; set; }
            public int LastRightClickedItem { get; set; }

            public class GameWindowAddresses
            {
                public GameWindowAddresses()
                {
                    this.Messages = new MessagesAddresses();
                }

                public MessagesAddresses Messages { get; private set; }
                public int Pointer { get; set; }
                public int Offset1 { get; set; }
                public int Offset2 { get; set; }

                public class MessagesAddresses
                {
                    public MessagesAddresses()
                    {
                        this.Distances = new DistanceAddresses();
                    }

                    public DistanceAddresses Distances { get; private set; }
                    public int NextIndex { get; set; }
                    public int Start { get; set; }
                    public int Step = 448;
                    public int LineStep = 40;
                    public int MaxMessages = 10;

                    public class DistanceAddresses
                    {
                        public int LineCount { get; set; }
                        public int LineText { get; set; }
                        public int Time { get; set; }
                        public int X { get; set; }
                        public int Y { get; set; }
                        public int Z { get; set; }
                        public int MessageType { get; set; }
                        public int IsVisible { get; set; }
                        public int Index { get; set; }
                    }
                }
            }
        }

        public class MapAddresses
        {
            public MapAddresses()
            {
                this.Distances = new DistanceAddresses();
            }

            public DistanceAddresses Distances { get; private set; }

            public int LevelSpy1 { get; set; }
            public int LevelSpy2 { get; set; }
            public int LevelSpy3 { get; set; }
            public int LevelSpyPointer { get; set; }
            public byte[] LevelSpyOriginalBytes1 { get; set; }
            public byte[] LevelSpyOriginalBytes2 { get; set; }
            public byte[] LevelSpyOriginalBytes3 { get; set; }
            public byte[] LevelSpyPatchedBytes = new byte[] { 0x90, 0x90, 0x90, 0x90, 0x90, 0x90 };
            public int LevelSpyOffset1 { get; set; }
            public int LevelSpyOffset2 { get; set; }

            public int NameSpy1 { get; set; }
            public int NameSpy2 { get; set; }
            public byte[] NameSpyOriginalBytes1 { get; set; }
            public byte[] NameSpyOriginalBytes2 { get; set; }
            public byte[] NameSpyPatchedBytes = new byte[] { 0x90, 0x90 };

            public int Pointer { get; set; }
            public int TileStep { get; set; }
            public int ObjectStep { get; set; }

            public int TileNumberPointer { get; set; }
            public int TileNumberStep = 4;

            public class DistanceAddresses
            {
                public int TileObjectsCount { get; set; }
                public int ObjectID { get; set; }
                public int ObjectData { get; set; }
                public int ObjectDataEx { get; set; }
            }
        }

        public class MiniMapAddresses
        {
            public MiniMapAddresses()
            {
                this.Distances = new DistanceAddresses();
            }

            public DistanceAddresses Distances { get; private set; }

            public int Start { get; set; }
            public int End { get; set; }
            public int Step = 0x200A8;
            /// <summary>
            /// The maximum amount of map files loaded into memory.
            /// </summary>
            public int MaxEntries = 10;
            public int AxisLength = 256;
            public int DataLength = 256 * 256;
            public int WorldLocationMultiplier = 256;
            /// <summary>
            /// Compare this to a chunk's X position to see if data has been loaded.
            /// </summary>
            public int NotYetLoadedValue = 0xFFFF;

            public class DistanceAddresses
            {
                public int X = 0;
                public int Y = 4;
                public int Z = 8;
                public int Colors = 0x14;
                public int TileSpeeds = 0x14 + 256 * 256;
            }
        }

        public class VipAddresses
        {
            public VipAddresses()
            {
                this.Distances = new VipDistances();
            }

            public int Start { get; set; }
            public int End { get; set; }
            public int Step = 44;
            public int MaxEntries = 200;
            public int Count { get; set; }
            public VipDistances Distances { get; private set; }

            public class VipDistances
            {
                public int ID = 0;
                public int Name = 4;
                /// <summary>
                /// A boolean that represents whether this character is online.
                /// </summary>
                public int Online = 34;
                public int Icon = 40;
            }
        }

        public class InternalFunctionsAddresses
        {
            public int MainFunction { get; set; }
            public int Say { get; set; }
            public int UseItem { get; set; }
            public int UseItemOn { get; set; }
            public int UseItemOnBattleList { get; set; }
            public int Attack { get; set; }
            public int Follow { get; set; }
            public int Logout { get; set; }
            public int PrivateMessage { get; set; }
            public int ChannelMessage { get; set; }
            public int MoveItem { get; set; }
            public int TextMessage { get; set; }
            public int CloseContainer { get; set; }
            public int OpenParentContainer { get; set; }
            public int FightModes { get; set; }
            public int HotkeyUseItem { get; set; }
            public int HotkeyUseItemEx { get; set; }
            public int HotkeyUseItemOnCreature { get; set; }
            public int TurnEast { get; set; }
            public int TurnWest { get; set; }
            public int TurnSouth { get; set; }
            public int TurnNorth { get; set; }
            public int WalkNorth { get; set; }
            public int WalkEast { get; set; }
            public int WalkSouth { get; set; }
            public int WalkWest { get; set; }
            public int WalkNorthEast { get; set; }
            public int WalkSouthEast { get; set; }
            public int WalkSouthWest { get; set; }
            public int WalkNorthWest { get; set; }
            /// <summary>
            /// Args: uint character id
            /// </summary>
            public int VipRemove { get; set; }
            /// <summary>
            /// Args: string character name
            /// </summary>
            public int VipAdd { get; set; }
            /// <summary>
            /// Can be found by looking through the MoveItem function.
            /// </summary>
            public int GetObjectProperty { get; set; }
            /// <summary>
            /// Only found so far in 7.72.
            /// </summary>
            public int SendPacket { get; set; }
            public int Ping { get; set; }
        }
        #endregion
    }
}
