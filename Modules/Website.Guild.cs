using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KarelazisBot.Modules
{
    public partial class Website
    {
        public class Guild
        {
            public Guild(string name, int id)
            {
                this.Name = name;
                this.ID = id;
                this.Members = new List<Character>();
            }

            public string Name { get; set; }
            public int ID { get; set; }
            public List<Character> Members { get; private set; }
        }
    }
}
