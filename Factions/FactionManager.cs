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
        private Dictionary<FactionEnums, Faction> factions = new Dictionary<FactionEnums, Faction>();

        /// <summary>
        /// Register all new factions here, otherwise they won't be added to the game.
        /// </summary>
        public FactionManager()
        {
            RegisterFaction(new Civilian(FactionEnums.CIVILIAN, "Civilian", 50, 205, 50));
            RegisterFaction(new LawEnforcement(FactionEnums.LAW_ENFORCEMENT, "Law Enforcement", 0, 102, 204));
            RegisterFaction(new FireDepartment(FactionEnums.FIREMAN, "Fire Department", 209, 33, 56));
            RegisterFaction(new FirstResponder(FactionEnums.FIRST_RESPONDER, "First Responder", 219, 107, 141));
            RegisterFaction(new TaxiDriver(FactionEnums.TAXI_DRIVER, "Los Santos Taxi", 244, 191, 66));
        }

        /// <summary>
        /// Initializes all factions
        /// </summary>
        public void InitializeFactions()
        {
            foreach (Faction faction in factions.Values)
            {
                API.shared.consoleOutput(String.Format("[Factions] Initialized faction \"{0}\"", faction.name));
                faction.Initialize();
            }

            MapManager.Instance().SubscribeToOnMinuteChange(this.PaySalaries);
        }

        /// <summary>
        /// Pays the salaries for members in all factions.
        /// </summary>
        /// <param name="timeSpan">Timespan</param>
        private void PaySalaries(TimeSpan timeSpan)
        {
            if (timeSpan.Minutes == 0)
            {
                foreach (Faction faction in factions.Values)
                {
                    faction.PaySalary();
                }
            }
        }

        /// <summary>
        /// Registers a new faction on the server
        /// This method has to be called for all factions that are added to the server
        /// </summary>
        /// <param name="faction">The faction to register</param>
        private void RegisterFaction(Faction faction)
        {
            factions.Add(faction.id, faction);
        }

        /// <summary>
        /// Return a faction with certain id
        /// </summary>
        /// <param name="id"></param>
        /// <returns>A faction with id</returns>
        public Faction GetFactionWithId(FactionEnums id)
        {
            return factions.Get(id);
        }

        /// <summary>
        /// Gets the name of the faction rank of the character.
        /// </summary>
        /// <param name="character"></param>
        /// <returns>Rank name</returns>
        public string GetRankTextForCharacter(Character character)
        {
            return factions[character.factionID].GetRankText(character);
        }

        /// Accessor methods for different factions
        /// If you create a new faction, you can add an accessor method here so it's easier
        /// to access the faction from the faction manager.
        public LawEnforcement LawEnforcement() { return factions.Get(FactionEnums.LAW_ENFORCEMENT) as LawEnforcement; }
        public FireDepartment FireDepartment() { return factions.Get(FactionEnums.FIREMAN) as FireDepartment;  }
        public FirstResponder FirstResponder() { return factions.Get(FactionEnums.FIRST_RESPONDER) as FirstResponder;  }
        public TaxiDriver TaxiDriver() { return factions.Get(FactionEnums.TAXI_DRIVER) as TaxiDriver; }

        /// <summary>
        /// Handles the on duty command for character's faction
        /// </summary>
        /// <param name="character">Character object</param>
        public void HandleDutyCommand(Character character)
        {
            factions.Get(character.factionID).HandleOnDutyCommand(character);
        }

    }
}
