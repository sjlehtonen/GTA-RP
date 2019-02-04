using System;
using GrandTheftMultiplayer.Server.Constant;
using GrandTheftMultiplayer.Server.API;
using GrandTheftMultiplayer.Server.Elements;
using GrandTheftMultiplayer.Shared;
using GrandTheftMultiplayer.Shared.Math;

namespace GTA_RP.Factions
{
    /// <summary>
    /// Class for the first responder faction.
    /// TODO
    /// </summary>
    class FirstResponder : RankedFaction
    {
        public FirstResponder(FactionEnums id, string name, int colorR, int colorG, int colorB) : base(id, name, colorR, colorG, colorB) { }

        public override void HandleOnDutyCommand(Character character)
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
