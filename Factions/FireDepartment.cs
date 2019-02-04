
namespace GTA_RP.Factions
{
    /// <summary>
    /// Class for the fire department faction.
    /// TODO
    /// </summary>
    class FireDepartment : RankedFaction
    {
        public FireDepartment(FactionEnums id, string name, int colorR, int colorG, int colorB) : base(id, name, colorR, colorG, colorB) { }

        public override void HandleOnDutyCommand(Character c)
        {
            // TODO
        }

        public override string GetChatColor()
        {
            return "~r~";
        }

        private void InitializeRanks()
        {
            // Add new ranks here
            this.AddRank(0, "Fire Officer", 600);
        }

        public override void Initialize()
        {
            base.Initialize();
            InitializeRanks();
        }
    }
}
