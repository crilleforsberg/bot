using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace KarelazisBot.Modules
{
    public partial class Website
    {
        public class Character
        {
            public Character() { }

            public string Name { get; set; }
            public string Vocation { get; set; }
            public Guild Guild { get; set; }
            public string GuildNick { get; set; }
            public ushort Level { get; set; }
            public uint Experience { get; set; }
            public ushort MagicLevel { get; set; }
            public ushort AxeSkill { get; set; }
            public ushort ClubSkill { get; set; }
            public ushort SwordSkill { get; set; }
            public ushort DistanceSkill { get; set; }
            public ushort FistSkill { get; set; }
            public ushort FishingSkill { get; set; }

            public override string ToString()
            {
                return !string.IsNullOrEmpty(this.Name) ? this.Name : base.ToString();
            }
        }
    }
}
