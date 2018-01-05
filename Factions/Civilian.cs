using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GTA_RP.Factions
{
    /// <summary>
    /// Class representing the civilian faction
    /// </summary>
    class Civilian : Faction
    {
        public Civilian(FactionI id, string name, int colorR, int colorG, int colorB) : base(id, name, colorR, colorG, colorB) { }

        public override void Initialize()
        {

        }

        public override string GetChatColor()
        {
            return "~g~";
        }

        public override void HandleOnDutyCommand(Character c)
        {

        }
    }
}
