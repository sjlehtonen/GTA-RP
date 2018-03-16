using System;
using GrandTheftMultiplayer.Server.Constant;
using GrandTheftMultiplayer.Server.API;
using GrandTheftMultiplayer.Server.Elements;
using GrandTheftMultiplayer.Shared;
using GrandTheftMultiplayer.Shared.Math;
using System.Collections.Generic;
using GTA_RP.Map;
using GTA_RP.Misc;
using System.Timers;

namespace GTA_RP.Factions
{
    class FireDepartment : RankedFaction
    {
        public FireDepartment(FactionI id, string name, int colorR, int colorG, int colorB) : base(id, name, colorR, colorG, colorB) { }

        public override void HandleOnDutyCommand(Character c)
        {

        }

        public override string GetChatColor()
        {
            return "~r~";
        }

        private void InitializeRanks()
        {
            this.AddRank(0, "Fire Officer", 600);
        }

        public override void Initialize()
        {
            base.Initialize();
            InitializeRanks();
        }
    }
}
