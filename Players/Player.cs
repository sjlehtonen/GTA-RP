using GrandTheftMultiplayer.Server.API;
using GrandTheftMultiplayer.Server.Elements;
using GrandTheftMultiplayer.Server.Managers;
using GrandTheftMultiplayer.Shared;
using GrandTheftMultiplayer.Shared.Math;
using GrandTheftMultiplayer.Server.Constant;
using System.Collections.Generic;
using GTA_RP.Jobs;
using GTA_RP.Factions;
using GTA_RP.Vehicles;
using System.Linq;

namespace GTA_RP
{
    /// <summary>
    /// Player class
    /// Every player who is logged in has this class
    /// </summary>
    public class Player
    {
        public int id { get; private set; }
        public Client client { get; private set; }
        public Character activeCharacter { get; private set; }
        public int adminLevel { get; private set; }

        /// <summary>
        /// Gets hash code for player id
        /// </summary>
        /// <returns>AHashcode of player id</returns>
        public override int GetHashCode()
        {
            return id;
        }

        /// <summary>
        /// Checks if two players equal
        /// </summary>
        /// <param name="obj"></param>
        /// <returns>True if players are equal, otherwise false</returns>
        public override bool Equals(object obj)
        {
            if(obj.GetType() != typeof(Player))
            {
                return false;
            }

            Player second = obj as Player;
            if(second.id != this.id)
            {
                return false;
            }

            return true;
        }

        public Player(Client client, int id, int adminLevel)
        {
            this.client = client;
            this.id = id;
            this.adminLevel = adminLevel;
        }

        /// <summary>
        /// Sets active character for player
        /// </summary>
        /// <param name="c">Character to set as active character</param>
        public void SetActiveCharacter(Character c)
        {
            activeCharacter = c;
            client.nametag = activeCharacter.fullName;
            client.nametagColor = new Color(c.faction.colorR, c.faction.colorG, c.faction.colorB);
            client.setSkin(API.shared.pedNameToModel(c.model));

            JobInfo i = JobManager.Instance().GetInfoForJobWithId(c.job);
            Faction f = FactionManager.Instance().GetFactionWithId(c.factionID);

            // Add vehicles
            List<RPVehicle> vehicles = VehicleManager.Instance().GetVehiclesForCharacter(c);

            API.shared.triggerClientEvent(c.owner.client, "EVENT_INIT_HUD", i.name, i.colorR, i.colorG, i.colorB, f.name, f.colorR, f.colorG, f.colorB, c.money.ToString(), c.fullName, c.phone.phoneNumber, c.phone.GetTextMessageIds(), c.phone.GetTextMessageSenders(), c.phone.GetTextMessageTimes(), c.phone.GetTextMessageTexts(), c.phone.GetContactNames(), c.phone.GetContactNumbers(), vehicles.Select(x => x.id).ToList(), vehicles.Select(x => x.licensePlateText), vehicles.Select(x => x.spawned));
            API.shared.triggerClientEvent(c.owner.client, "EVENT_CLOSE_CHARACTER_SELECT_MENU");

            HouseManager.Instance().SendListOfOwnedHousesToClient(c.owner.client);


            API.shared.triggerClientEvent(c.owner.client, "EVENT_TOGGLE_HUD_ON");
        }
    }
}
