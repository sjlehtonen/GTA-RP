using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GTA_RP.Factions
{
    class TaxiDriver : RankedFaction
    {
        public TaxiDriver(FactionI id, string name, int colorR, int colorG, int colorB) : base(id, name, colorR, colorG, colorB) { }

        public override string GetChatColor()
        {
            return "~y~";
        }

        public override void HandleOnDutyCommand(Character c)
        {

        }

        private void InitializeRanks()
        {
            this.AddRank(0, "Taxi Driver", 360);
        }

        public override void Initialize()
        {
            base.Initialize();
            InitializeRanks();
        }
    }
}
