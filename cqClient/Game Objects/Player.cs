using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace cqClient.Game_Objects
{
    class Player
    {
        public string kingdomName
        {
            get; set;
        }
        public string race { get; set; }
        public string level { get; set; }
        public string name { get; set; }
        public string city { get; set; }
        public string continent { get; set; }
        public int movement { get; set; }
        public int structures { get; set; }
        public int gold { get; set; }
        public int land { get; set; }
        public int peasants { get; set; }
        public int food { get; set; }
        public bool protection { get; set; }
        public int experience { get; set; }
        public string taxes { get; set; }
    }
}
