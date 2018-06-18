using System.Collections.Generic;
using GrandTheftMultiplayer.Server.API;
using GrandTheftMultiplayer.Shared;
using GTA_RP.Misc;
using GTA_RP.Map;
using System;

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
            RegisterFaction(new LawEnforcement(FactionI.LAW_ENFORCEMENT, "Law Enforcement", 0, 102, 204));
            RegisterFaction(new FireDepartment(FactionI.FIREMAN, "Fire Department", 209, 33, 56));
            RegisterFaction(new FirstResponder(FactionI.FIRST_RESPONDER, "First Responder", 219, 107, 141));
            RegisterFaction(new TaxiDriver(FactionI.TAXI_DRIVER, "Los Santos Taxi", 244, 191, 66));
        }

        /// <summary>
        /// Initializes all factions
        /// </summary>
        public void InitializeFactions()
        {
            foreach (Faction faction in factions.Values)
            {
                API.shared.consoleOutput("[Factions] Initialized faction \"" + faction.name + "\"");
                faction.Initialize();
            }

            MapManager.Instance().SubscribeToOnMinuteChange(this.PaySalaries);
        }

        private void PaySalaries(TimeSpan t)
        {
            if (t.Minutes == 0)
                foreach (Faction faction in factions.Values) faction.PaySalary();
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

        public string GetRankTextForCharacter(Character c)
        {
            return factions[c.factionID].GetRankText(c);
        }

        public LawEnforcement LawEnforcement() { return factions.Get(FactionI.LAW_ENFORCEMENT) as LawEnforcement; }
        public FireDepartment FireDepartment() { return factions.Get(FactionI.FIREMAN) as FireDepartment;  }
        public FirstResponder FirstResponder() { return factions.Get(FactionI.FIRST_RESPONDER) as FirstResponder;  }
        public TaxiDriver TaxiDriver() { return factions.Get(FactionI.TAXI_DRIVER) as TaxiDriver; }

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
