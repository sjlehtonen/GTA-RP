using System.Collections.Generic;
using GrandTheftMultiplayer.Server.API;
using GrandTheftMultiplayer.Shared;
using GTA_RP.Misc;

namespace GTA_RP.Factions
{
    /// <summary>
    /// A class that is responsible for registering new factions and handling all matters associated with them
    /// </summary>
    class FactionManager : Singleton<FactionManager>
    {
        private Dictionary<FactionI, Faction> factions = new Dictionary<FactionI, Faction>();

        public FactionManager()
        {
             RegisterFaction(new Civilian(FactionI.CIVILIAN, "Civilian", 50, 205, 50));
             RegisterFaction(new LawEnforcement(FactionI.LAW_ENFORCEMENT, "Law enforcement", 0, 102, 204));
        }

        /// <summary>
        /// Initializes all factions
        /// </summary>
        public void InitializeFactions()
        {
            foreach (Faction faction in factions.Values)
                faction.Initialize();
        }

        /// <summary>
        /// Registers a new faction on the server
        /// This method has to be called for all factions that are added to the server
        /// </summary>
        /// <param name="f">The faction to register</param>
        private void RegisterFaction(Faction f)
        {
            factions.Add(f.id, f);
        }

        /// <summary>
        /// Return a faction with certain id
        /// </summary>
        /// <param name="id"></param>
        /// <returns>A faction with id</returns>
        public Faction GetFactionWithId(FactionI id)
        {
            return factions.Get(id);
        }

        public LawEnforcement LawEnforcement() { return factions.Get(FactionI.LAW_ENFORCEMENT) as LawEnforcement; }

        /// <summary>
        /// Handles the on duty command for character's faction
        /// </summary>
        /// <param name="c">Character object</param>
        public void HandleDutyCommand(Character c)
        {
            factions.Get(c.factionID).HandleOnDutyCommand(c);
        }

    }
}
