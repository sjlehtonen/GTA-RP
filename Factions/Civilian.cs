using GTA_RP.Jobs;

namespace GTA_RP.Factions
{
    /// <summary>
    /// Class representing the civilian faction
    /// </summary>
    class Civilian : Faction
    {
        public Civilian(FactionEnums id, string name, int colorR, int colorG, int colorB) : base(id, name, colorR, colorG, colorB) { }

        public override void Initialize() { }

        public override string GetRankText(Character character)
        {
            JobInfo info = JobManager.Instance().GetInfoForJobWithId(character.job);
            return info.name;
        }

        public override string GetChatColor()
        {
            return "~g~";
        }

        // Citizen has no duty, no need to implement
        public override void HandleOnDutyCommand(Character c) { }
    }
}
