using System;
using System.Collections.Generic;
using GTA_RP.Misc;
using GrandTheftMultiplayer.Server.API;
using GrandTheftMultiplayer.Shared.Math;
using GTA_RP.Map;

namespace GTA_RP.Vehicles
{
    /// <summary>
    /// Class for buying vehicles
    /// </summary>
    class VehicleShop
    {
        private List<Character> characters = new List<Character>();
        private List<string> availableVehicles = new List<string>();
        private Dictionary<string, int> vehiclePrice = new Dictionary<string, int>();

        private int id;
        private Checkpoint enterStoreCheckpoint;
        private Vector3 exitPos;
        private Vector3 exitRot;

        private Vector3 characterShopPosition = new Vector3(207.1324, -1007.67, -98.99998);

        private Vector3 cameraPosition = new Vector3(205.331, -1004.533, -98.0);
        private Vector3 cameraRotation = new Vector3(-18, 0, 50.22344);

        private Vector3 vehiclePosition = new Vector3(201.9024, -1001.854, -99.00001);
        private Vector3 vehicleRotation = new Vector3(0, 0, 176.9532);

        public VehicleShop(int id, Vector3 entrance, Vector3 exitPos, Vector3 exitRot, Vector3 cameraPos, Vector3 cameraRot, Vector3 charPos, Vector3 vehiclePos, Vector3 vehicleRot)
        {
            this.LoadVehiclePricesFromDatabase();
            this.id = id;
            this.enterStoreCheckpoint = new Checkpoint(entrance, this.CharacterWalkedToEntrance);
            this.exitPos = exitPos;
            this.exitRot = exitRot;
            this.cameraPosition = cameraPos;
            this.cameraRotation = cameraRot;
            this.characterShopPosition = charPos;
            this.vehiclePosition = vehiclePos;
            this.vehicleRotation = vehicleRot;

            MapManager.Instance().AddBlipToMap(523, "Vehicle dealership", entrance.X, entrance.Y, entrance.Z);
            PlayerManager.Instance().SubscribeToPlayerDisconnectEvent(this.CharacterDisconnected);
        }

        /// <summary>
        /// Is ran when player disconnects
        /// </summary>
        /// <param name="character"></param>
        private void CharacterDisconnected(Character character)
        {
             ExitShop(character);
        }

       

        /// <summary>
        /// Teleports player inside the vehicle shop
        /// </summary>
        /// <param name="character">Character</param>
        private void TeleportPlayerInsideShop(Character character)
        {
            character.position = characterShopPosition;
            character.owner.client.freezePosition = true;
            character.owner.client.dimension = 1;
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
        /// <param name="character"></param>
        private void TeleportPlayerOutOfShop(Character character)
        {
            character.position = exitPos;
            character.owner.client.rotation = exitRot;
            character.owner.client.freezePosition = false;
            character.owner.client.dimension = 0;
        }

        /// <summary>
        /// Loads vehicle prices and models from the database
        /// </summary>
        private void LoadVehiclePricesFromDatabase()
        {
            var cmd = DBManager.SimpleQuery("SELECT * FROM vehicle_prices");
            var reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                vehiclePrice.Add(reader.GetString(0), reader.GetInt32(1));
                availableVehicles.Add(reader.GetString(0));
            }

            reader.Close();
        }

        /// <summary>
        /// Checks whether character is inside the shop
        /// </summary>
        /// <param name="character">Character</param>
        /// <returns>True if character is in the shop, otherwise false</returns>
        public Boolean IsCharacterInShop(Character character)
        {
            return characters.Contains(character);
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
                List<int> prices = new List<int>();
                availableVehicles.ForEach(x => prices.Add(vehiclePrice[x]));
                character.TriggerEvent("EVENT_CHARACTER_ENTER_VEHICLE_SHOP", id, availableVehicles, prices, vehiclePosition, vehicleRotation);
                character.TriggerEvent("EVENT_SET_LOGIN_SCREEN_CAMERA", cameraPosition, cameraRotation);
                PlayerManager.Instance().RemoveMinimapFromPlayer(character);
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
                character.TriggerEvent("EVENT_CHARACTER_EXIT_VEHICLE_SHOP");
                character.TriggerEvent("EVENT_REMOVE_CAMERA");
                PlayerManager.Instance().SetMinimapForPlayer(character);
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

                        character.SetMoney(character.money - price);
                        string licensePlate = VehicleManager.Instance().GenerateUnusedLicensePlate();
                        int id = VehicleManager.Instance().AddVehicleToDatabase(character.ID, Factions.FactionEnums.CIVILIAN, model, -223.2121f, -1168.157f, 22.5882f, 0.5045538f, 0.1192265f, -91.68389f, licensePlate, 0, color1, color2);
                        VehicleManager.Instance().SendUpdatedVehicleToClient(character, id, licensePlate, false);
                        character.TriggerEvent("EVENT_CHARACTER_EXIT_VEHICLE_SHOP");
                        character.SendNotification("Vehicle purchased!\nExit the shop and access the vehicle from your F1 menu");
                    }
                    else
                    {
                        character.SendNotification("You don't have enough money to buy this vehicle!");
                    }
                }
            }
        }
    }
}
