using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GTA_RP.Misc;
using GrandTheftMultiplayer.Server.API;
using GrandTheftMultiplayer.Server.Elements;
using GrandTheftMultiplayer.Server.Managers;
using GrandTheftMultiplayer.Shared;
using GrandTheftMultiplayer.Shared.Math;

namespace GTA_RP.Vehicles
{
    public struct VehicleInfo
    {
        public string model;
        public int price;

        public VehicleInfo(string model, int price)
        {
            this.model = model;
            this.price = price;
        }
    }

    /// <summary>
    /// Class for buying vehicles
    /// </summary>
    class VehicleShop
    {
        private List<Character> characters = new List<Character>();
        private List<VehicleInfo> availableVehicles = new List<VehicleInfo>();
        private Dictionary<string, int> vehiclePrice = new Dictionary<string, int>();
        private Checkpoint enterStoreCheckpoint;
        private Vector3 exitPos;
        private Vector3 exitRot;

        private Vector3 characterShopPosition = new Vector3(207.1324, -1007.67, -98.99998);

        private Vector3 cameraPosition = new Vector3(205.331, -1004.533, -98.0);
        private Vector3 cameraRotation = new Vector3(-18, 0, 50.22344);

        // FIX
        private Vector3 vehiclePosition = new Vector3(201.9024, -1001.854, -99.00001);
        private Vector3 vehicleRotation = new Vector3(0, 0, 176.9532);

        public VehicleShop(Vector3 entrance, Vector3 exitPos, Vector3 exitRot)
        {
            enterStoreCheckpoint = new Checkpoint(entrance, this.CharacterWalkedToEntrance);
            this.exitPos = exitPos;
            this.exitRot = exitRot;

            PlayerManager.Instance().SubscribeToPlayerDisconnectEvent(this.CharacterDisconnected);
        }

        /// <summary>
        /// Is ran when player disconnects
        /// </summary>
        /// <param name="c"></param>
        private void CharacterDisconnected(Client c)
        {
            if (PlayerManager.Instance().IsClientUsingCharacter(c))
                ExitShop(PlayerManager.Instance().GetActiveCharacterForClient(c));
        }

       

        /// <summary>
        /// Teleports player inside the vehicle shop
        /// </summary>
        /// <param name="c">Character</param>
        private void TeleportPlayerInsideShop(Character c)
        {
            c.position = characterShopPosition;
            c.owner.client.freezePosition = true;
            c.owner.client.dimension = 1;
        }

        /// <summary>
        /// Checks if shop has certain model of vehicle for sale
        /// </summary>
        /// <param name="model">Vehicle model</param>
        /// <returns>True if model exists in job, false otherwise</returns>
        private Boolean DoesShopHaveModel(string model)
        {
            return vehiclePrice.ContainsKey(model);
        }

        /// <summary>
        /// Gets price for selected vehicle model
        /// </summary>
        /// <param name="model"></param>
        /// <returns>Vehicle price</returns>
        private int GetPriceForModel(string model)
        {
            return vehiclePrice[model];
        }

        /// <summary>
        /// Teleports player out of the vehicle shop
        /// </summary>
        /// <param name="c"></param>
        private void TeleportPlayerOutOfShop(Character c)
        {
            c.position = exitPos;
            c.owner.client.rotation = exitRot;
            c.owner.client.freezePosition = false;
            c.owner.client.dimension = 0;
        }

        /// <summary>
        /// Loads vehicle prices and models from the database
        /// </summary>
        public void LoadVehiclePricesFromDatabase()
        {
            var cmd = DBManager.SimpleQuery("SELECT * FROM vehicle_prices");
            var reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                vehiclePrice.Add(reader.GetString(0), reader.GetInt32(1));
                availableVehicles.Add(new VehicleInfo(reader.GetString(0), reader.GetInt32(1)));
            }

            reader.Close();
        }

        /// <summary>
        /// Checks whether character is inside the shop
        /// </summary>
        /// <param name="c">Character</param>
        /// <returns>True if character is in the shop, otherwise false</returns>
        public Boolean IsCharacterInShop(Character c)
        {
            return characters.Contains(c);
        }

        /// <summary>
        /// Ran when character enters the entrance checkpoint
        /// </summary>
        /// <param name="checkpoint">Checkpoint</param>
        /// <param name="character">Character</param>
        public void CharacterWalkedToEntrance(Checkpoint checkpoint, Character character)
        {
            EnterShop(character);
        }

        /// <summary>
        /// Enters the character into the shop
        /// </summary>
        /// <param name="character"></param>
        public void EnterShop(Character character)
        {
            if (!IsCharacterInShop(character))
            {
                characters.Add(character);
                TeleportPlayerInsideShop(character);
                API.shared.triggerClientEvent(character.owner.client, "EVENT_CHARACTER_ENTER_VEHICLE_SHOP", availableVehicles.Select(x => x.model).ToList(), availableVehicles.Select(x => x.price).ToList(), vehiclePosition, vehicleRotation);
                API.shared.triggerClientEvent(character.owner.client, "EVENT_SET_LOGIN_SCREEN_CAMERA", cameraPosition, cameraRotation);
            }
        }

        /// <summary>
        /// Exists the character from the shop
        /// </summary>
        /// <param name="character"></param>
        public void ExitShop(Character character)
        {
            if (IsCharacterInShop(character))
            {
                characters.Remove(character);
                TeleportPlayerOutOfShop(character);
                API.shared.triggerClientEvent(character.owner.client, "EVENT_CHARACTER_EXIT_VEHICLE_SHOP");
                API.shared.triggerClientEvent(character.owner.client, "EVENT_REMOVE_CAMERA");
            }
        }

        /// <summary>
        /// Buys a vehicle with selected attributes
        /// </summary>
        /// <param name="character">Character who is buying</param>
        /// <param name="model">Vehicle model</param>
        /// <param name="color1">Color 1</param>
        /// <param name="color2">Color 2</param>
        public void PurchaseVehicle(Character character, string model, int color1, int color2)
        {
            if (IsCharacterInShop(character))
            {
                if (DoesShopHaveModel(model))
                {
                    int price = GetPriceForModel(model);
                    if (character.money >= price)
                    {
                        // Deduct the money
                        // Create vehicle and add to databse
                        // Add vehicle to vehicle manager
                        // Inform bueyer that he got new vehicle

                        character.money -= price;
                        string licensePlate = VehicleManager.Instance().GenerateUnusedCivilianLicensePlate();
                        int id = VehicleManager.Instance().AddVehicleToDatabase(character.ID, Factions.FactionI.CIVILIAN, model, 0.0f, 0.0f, 0.0f, 0.0f, 0.0f, 0.0f, licensePlate, 0, color1, color2);
                        VehicleManager.Instance().SendUpdatedVehicleToClient(character, id, licensePlate, false);
                        API.shared.triggerClientEvent(character.owner.client, "EVENT_CHARACTER_EXIT_VEHICLE_SHOP");
                        API.shared.sendNotificationToPlayer(character.owner.client, "Vehicle purchased!\nExit the shop and access the vehicle from your F1 menu");
                    }
                    else
                    {
                        API.shared.sendNotificationToPlayer(character.owner.client, "You don't have enough money to buy this vehicle!");
                    }
                }
            }
        }
    }
}
