using GrandTheftMultiplayer.Server.API;
using GrandTheftMultiplayer.Server.Elements;
using GrandTheftMultiplayer.Server.Constant;
using System.Collections.Generic;
using GTA_RP.Jobs;
using GTA_RP.Factions;
using GTA_RP.Vehicles;
using System.Linq;
using GTA_RP.Items;

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
        /// Is ran when the player is deleted
        /// Removes attached objects from active character
        /// </summary>
        public void CleanUp()
        {
            if (this.activeCharacter != null) { this.activeCharacter.CleanUp(); }
        }

        /// <summary>
        /// Triggers a client event
        /// </summary>
        /// <param name="name">Event name</param>
        /// <param name="args">Event args</param>
        public void TriggerEvent(string name, params object[] args)
        {
            this.client.triggerEvent(name, args);
        }

        /// <summary>
        /// Sets active character for player
        /// </summary>
        /// <param name="character">Character to set as active character</param>
        public void SetActiveCharacter(Character character)
        {
            activeCharacter = character;
            client.nametag = activeCharacter.ID.ToString() + " " + activeCharacter.fullName;
            client.nametagColor = new Color(character.faction.colorR, character.faction.colorG, character.faction.colorB);
            client.setSkin(API.shared.pedNameToModel(character.model));

            JobInfo i = JobManager.Instance().GetInfoForJobWithId(character.job);
            Faction f = FactionManager.Instance().GetFactionWithId(character.factionID);

            List<RPVehicle> vehicles = VehicleManager.Instance().GetVehiclesForCharacter(character);
            List<Item> items = character.GetAllItemsFromInventory();

            API.shared.triggerClientEvent(character.owner.client, "EVENT_INIT_HUD", i.name, i.colorR, i.colorG, i.colorB, f.name, f.colorR, f.colorG, f.colorB, character.money.ToString(), character.fullName, character.phone.phoneNumber, character.phone.GetTextMessageIds(), character.phone.GetTextMessageSenders(), character.phone.GetTextMessageTimes(), character.phone.GetTextMessageTexts(), character.phone.GetContactNames(), character.phone.GetContactNumbers(), vehicles.Select(x => x.id).ToList(), vehicles.Select(x => x.licensePlateText).ToList(), vehicles.Select(x => x.spawned).ToList(), items.Select(x => x.id).ToList(), items.Select(x => x.name).ToList(), items.Select(x => x.count).ToList(), items.Select(x => x.description).ToList());
            API.shared.triggerClientEvent(character.owner.client, "EVENT_CLOSE_CHARACTER_SELECT_MENU");

            HouseManager.Instance().SendListOfOwnedHousesToClient(character.owner.client);

            // Check faction, if is part of faction, then get text
            if (character.factionID != FactionEnums.CIVILIAN) character.UpdateFactionRankText(FactionManager.Instance().GetRankTextForCharacter(character), 255, 255, 255);

            API.shared.triggerClientEvent(character.owner.client, "EVENT_TOGGLE_HUD_ON");
            PlayerManager.Instance().SetMinimapForPlayer(character);
        }
    }
}
