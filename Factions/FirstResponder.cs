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
    class FirstResponder : RankedFaction
    {
        public FirstResponder(FactionI id, string name, int colorR, int colorG, int colorB) : base(id, name, colorR, colorG, colorB) { }

        public override void HandleOnDutyCommand(Character c)
        {

        }

        public override string GetChatColor()
        {
            return "~r~";
        }

        private void InitializeRanks()
        {
            AddRank(0, "Medical Officer", 100);
        }

        public override void Initialize()
        {
            base.Initialize();
            InitializeRanks();
        }
    }
}
