using System.Collections.Generic;
using System.Linq;
using GrandTheftMultiplayer.Server.API;
using GrandTheftMultiplayer.Server.Elements;
using GrandTheftMultiplayer.Server.Managers;
using GrandTheftMultiplayer.Shared;
using GrandTheftMultiplayer.Shared.Math;
using GTA_RP.Factions;
using GTA_RP.Misc;
using System;

namespace GTA_RP.Vehicles
{
    public delegate void OnVehicleDestroyedDelegate(NetHandle vehicleHandle);
    public delegate void OnVehicleExitedDelegate(Client c, NetHandle vehicleHandle);
    public delegate void OnVehicleEnteredDelegate(Client c, NetHandle vehicleHandle);

    /// <summary>
    /// Class responsible for handling vehicles
    /// </summary>
    class VehicleManager : Singleton<VehicleManager>
    {
        List<RPVehicle> vehicles = new List<RPVehicle>();
        VehicleShop vehicleShop = new VehicleShop(new Vector3(-177.2077, -1158.487, 23.8137), new Vector3(-177.0255, -1153.632, 23.11556), new Vector3(0, 0, -2.621614));

        private event OnVehicleDestroyedDelegate OnVehicleDestroyedEvent;
        private event OnVehicleExitedDelegate OnVehicleExitedEvent;
        private event OnVehicleEnteredDelegate OnVehicleEnterEvent;

        private static VehicleManager _instance = null;
        private const float doorLockDistance = 1.5f;
        private const float parkDistance = 3.0f;
        private int vehicleInsertId = 0;

        public VehicleManager()
        {
            this.InitializeVehicleInsertId();
        }

        /// <summary>
        /// Announce destruction of vehicle to subscribers
        /// </summary>
        /// <param name="vehicle">Vehicle handle</param>
        public void VehicleDestroyedEvent(NetHandle vehicle)
        {
            RunVehicleDestroyedEvents(vehicle);
        }

        /// <summary>
        /// Announce player exiting vehicle to subscribers
        /// </summary>
        /// <param name="c">Client who exited vehicle</param>
        /// <param name="vehicle">Vehicle from which client exited</param>
        public void VehicleExitEvent(Client c, NetHandle vehicle)
        {
            RunVehicleExitEvents(c, vehicle);
        }

        /// <summary>
        /// Announce player entering vehicle to subscribers
        /// </summary>
        /// <param name="c">Client who entered vehicle</param>
        /// <param name="vehicle">Vehicle that client entered into</param>
        public void VehicleEnterEvent(Client c, NetHandle vehicle)
        {
            RunVehicleEnterEvents(c, vehicle);
        }


        /// <summary>
        /// Subscribes a delegate to vehicle destroyed event
        /// </summary>
        /// <param name="d">Delegate</param>
        public void SubscribeToVehicleDestroyedEvent(OnVehicleDestroyedDelegate d)
        {
            this.OnVehicleDestroyedEvent += d;
        }

        /// <summary>
        /// Subscribes a delegate to enter vehicle event
        /// </summary>
        /// <param name="d">Delegate</param>
        public void SubscribeToVehicleEnterEvent(OnVehicleEnteredDelegate d)
        {
            this.OnVehicleEnterEvent += d;
        }

        /// <summary>
        /// Subscribes a delegate to exit vehicle event
        /// </summary>
        /// <param name="d">Delegate</param>
        public void SubscribeToVehicleExitEvent(OnVehicleExitedDelegate d)
        {
            this.OnVehicleExitedEvent += d;
        }

        /// <summary>
        /// Unsubscribes from vehicle destroyed event
        /// </summary>
        /// <param name="d">Delegate</param>
        public void UnsubscribeFromVehicleDestroyedEvent(OnVehicleDestroyedDelegate d)
        {
            this.OnVehicleDestroyedEvent -= d;
        }

        /// <summary>
        /// Unsubscribes from enter vehicle event
        /// </summary>
        /// <param name="d">Delegate</param>
        public void UnsubscribeFromVehicleEnterEvent(OnVehicleEnteredDelegate d)
        {
            this.OnVehicleEnterEvent -= d;
        }

        /// <summary>
        /// Unsubscribes from exit vehicle event
        /// </summary>
        /// <param name="d">Delegate</param>
        public void UnsubscribeFromVehicleExitEvent(OnVehicleExitedDelegate d)
        {
            this.OnVehicleExitedEvent -= d;
        }

        /// <summary>
        /// Runs all subscriber methods for vehicle destroyed event
        /// </summary>
        /// <param name="vehicle">Vehicle handle</param>
        private void RunVehicleDestroyedEvents(NetHandle vehicle)
        {
            if (this.OnVehicleDestroyedEvent != null)
                this.OnVehicleDestroyedEvent.Invoke(vehicle);
        }

        /// <summary>
        /// Runs all subscriber methods for enter vehicle event
        /// </summary>
        /// <param name="vehicle">Vehicle handle</param>
        private void RunVehicleEnterEvents(Client c, NetHandle vehicle)
        {
            if (this.OnVehicleEnterEvent != null)
                this.OnVehicleEnterEvent.Invoke(c, vehicle);
        }

        
        /// <summary>
        /// Checks whether any vehicles exist
        /// </summary>
        /// <returns>True if vehicles exist, otherwise false</returns>
        private Boolean HasVehicles()
        {
            var cmd = DBManager.SimpleQuery("SELECT COUNT(*) FROM vehicles");
            var reader = cmd.ExecuteReader();
            reader.Read();
            int rows = reader.GetInt32(0);
            reader.Close();
            if (rows < 1)
                return false;
            return true;
        }

        /// <summary>
        /// Initializes the vehicle insert ID
        /// </summary>
        private void InitializeVehicleInsertId()
        {
            if (HasVehicles())
            {
                var cmd = DBManager.SimpleQuery("SELECT MAX(id) FROM vehicles");
                var reader = cmd.ExecuteReader();
                reader.Read();
                this.vehicleInsertId = reader.GetInt32(0) + 1;
                reader.Close();
            }
            else
                this.vehicleInsertId = 0;
        }

        /// <summary>
        /// Runs all subscriber methods for exit vehicle event
        /// </summary>
        /// <param name="vehicle">Vehicle handle</param>
        private void RunVehicleExitEvents(Client c, NetHandle vehicle)
        {
            if (this.OnVehicleExitedEvent != null)
                this.OnVehicleExitedEvent.Invoke(c, vehicle);
        }

        /// <summary>
        ///  Adds a vehicle into the vehicle manager
        /// </summary>
        /// <param name="id">Vehicle id</param>
        /// <param name="ownerId">Vehicle owner id</param>
        /// <param name="faction">Faction id</param>
        /// <param name="model">Vehicle model</param>
        /// <param name="parkX">Vehicle parking X position</param>
        /// <param name="parkY">Vehicle parking Y position</param>
        /// <param name="parkZ">Vehicle parking Z position</param>
        /// <param name="parkRotX">Vehicle parking X rotation</param>
        /// <param name="parkRotY">Vehicle parking Y rotation</param>
        /// <param name="parkRotZ">Vehicle parking Z rotation</param>
        /// <param name="plateText">Vehicle license plate text</param>
        /// <param name="color1">Vehicle primary color</param>
        /// <param name="color2">Vehicle secondary color</param>
        private void AddVehicleToManager(int id, int ownerId, FactionI faction, string model, float parkX, float parkY, float parkZ, float parkRotX, float parkRotY, float parkRotZ, string plateText, int color1, int color2)
        {
            RPVehicle v = new RPVehicle(id, ownerId, faction, API.shared.vehicleNameToModel(model), parkX, parkY, parkZ, parkRotX, parkRotY, parkRotZ, color1, color2, plateText, false);
            vehicles.Add(v);
        }

        /// <summary>
        /// Gets nearest vehicle
        /// </summary>
        /// <param name="position">Position for which to get the nearest vehicle</param>
        /// <returns>The vehicle</returns>
        public RPVehicle GetNearestVehicle(Vector3 position)
        {
            RPVehicle nearest = vehicles.First(); // fix
            float nearestDist = position.DistanceTo(nearest.position);

            foreach(RPVehicle v in vehicles)
            {
                if(position.DistanceTo(v.position) < nearestDist)
                    nearest = v;
            }

            return nearest;
        }

        /// <summary>
        /// Creates a new vehicle into the database
        /// </summary>
        /// <param name="ownerId">Id of the owning character</param>
        /// <param name="faction">Id of the faction if exists, 0 if civilian</param>
        /// <param name="model">Vehicle model</param>
        /// <param name="parkX">Vehicle park X position</param>
        /// <param name="parkY">Vehicle park Y position</param>
        /// <param name="parkZ">Vehicle park Z position</param>
        /// <param name="parkRotX">Vehicle park X rotation</param>
        /// <param name="parkRotY">Vehicle park Y rotation</param>
        /// <param name="parkRotZ">Vehicle park Z rotation</param>
        /// <param name="plateText">Text of the license plate</param>
        /// <param name="plateStyle">Style of the license plate</param>
        /// <param name="color1">Primary color of the vehicle</param>
        /// <param name="color2">Secondary color of the vehicle</param>
        /// <returns></returns>
        public int AddVehicleToDatabase(int ownerId, FactionI faction, string model, float parkX, float parkY, float parkZ, float parkRotX, float parkRotY, float parkRotZ, string plateText, int plateStyle, int color1, int color2)
        {
            var cmd = DBManager.SimpleQuery("INSERT INTO vehicles VALUES (@id, @ownerId, @faction, @model, @park_x, @park_y, @park_z, @park_rot_x, @park_rot_y, @park_rot_z, @platetxt, @style, @color1, @color2)");
            cmd.Parameters.AddWithValue("@id", this.vehicleInsertId);
            cmd.Parameters.AddWithValue("@ownerId", ownerId);
            cmd.Parameters.AddWithValue("@faction", (int)faction);
            cmd.Parameters.AddWithValue("@model", API.shared.vehicleNameToModel(model));
            cmd.Parameters.AddWithValue("@park_x", parkX);
            cmd.Parameters.AddWithValue("@park_y", parkY);
            cmd.Parameters.AddWithValue("@park_z", parkZ);
            cmd.Parameters.AddWithValue("@park_rot_x", parkRotX);
            cmd.Parameters.AddWithValue("@park_rot_y", parkRotY);
            cmd.Parameters.AddWithValue("@park_rot_z", parkRotZ);
            cmd.Parameters.AddWithValue("@platetxt", plateText);
            cmd.Parameters.AddWithValue("@style", 0);
            cmd.Parameters.AddWithValue("@color1", color1);
            cmd.Parameters.AddWithValue("@color2", color2);
            cmd.ExecuteNonQuery();
            this.AddVehicleToManager(this.vehicleInsertId, ownerId, faction, model, parkX, parkY, parkZ, parkRotX, parkRotY, parkRotZ, plateText, color1, color2);
            this.vehicleInsertId++;
            return this.vehicleInsertId - 1;
        }

        /// <summary>
        /// Generates an unused license plate for civilian vehicle
        /// </summary>
        /// <returns>License plate text</returns>
        public string GenerateUnusedCivilianLicensePlate()
        {
            return "a";
        }

        /// <summary>
        /// Sends a message to client that updates his HUD to include a new vehicle
        /// </summary>
        /// <param name="character">Character to whose client to send</param>
        /// <param name="vehicleId">Id of the new vehicle</param>
        /// <param name="licensePlateText">Text of the vehicle's license plate</param>
        /// <param name="spawned">Is vehicle spawned</param>
        public void SendUpdatedVehicleToClient(Character character, int vehicleId, string licensePlateText, bool spawned)
        {
            API.shared.triggerClientEvent(character.owner.client, "EVENT_NEW_VEHICLE_ADDED", vehicleId, licensePlateText, spawned);
        }

        /// <summary>
        /// Tries to purchase a vehicle for client
        /// Is called from event handler
        /// </summary>
        /// <param name="c">Client</param>
        /// <param name="model">Vehicle model</param>
        /// <param name="color1">Vehicle color 1</param>
        /// <param name="color2">Vehicle color 2</param>
        public void TryPurchaseVehicle(Client c, string model, int color1, int color2)
        {
            Character character = PlayerManager.Instance().GetActiveCharacterForClient(c);
            vehicleShop.PurchaseVehicle(character, model, color1, color2);
        }

        /// <summary>
        /// Gets all vehicles that the player owns
        /// </summary>
        /// <param name="character">Character</param>
        /// <returns>List of vehicles</returns>
        public List<RPVehicle> GetVehiclesForCharacter(Character character)
        {
            return vehicles.Where(x => x.ownerId == character.ID).ToList();
        }

        /// <summary>
        /// Returns a vehicle with id
        /// </summary>
        /// <param name="id">Vehicle id</param>
        /// <returns>Vehicle if exists with the id, otherwise null</returns>
        public RPVehicle GetVehicleWithId(int id)
        {
            foreach (RPVehicle v in vehicles)
            {
                if (v.id == id)
                    return v;
            }

            return null;
        }
        

        /// <summary>
        /// Spawns a vehicle for player with certain id
        /// </summary>
        /// <param name="c">Character</param>
        /// <param name="vehicleId">Vehicle id</param>
        public void SpawnVehicleForCharacter(Client c, int vehicleId)
        {
            Character character = PlayerManager.Instance().GetActiveCharacterForClient(c);
            RPVehicle vehicle = VehicleManager.Instance().GetVehicleWithId(vehicleId);
            if (vehicle != null && character.ID == vehicle.ownerId)
            {
                if (!vehicle.spawned)
                    vehicle.Spawn();
                else
                    API.shared.sendNotificationToPlayer(c, "This vehicle is already active!");
            }

        }

        /// <summary>
        /// Parks a vehicle with selected id
        /// </summary>
        /// <param name="c">Client</param>
        /// <param name="vehicleId">Vehicle id</param>
        public void ParkVehicle(Client c, int vehicleId)
        {
            Character character = PlayerManager.Instance().GetActiveCharacterForClient(c);
            RPVehicle vehicle = VehicleManager.Instance().GetVehicleWithId(vehicleId);
            if (vehicle != null && character.ID == vehicle.ownerId && vehicle.spawned)
            {
                if (vehicle.handle == API.shared.getPlayerVehicle(c))
                {
                    if (vehicle.position.DistanceTo(vehicle.parkPosition) <= parkDistance)
                        vehicle.Park();
                    else
                        API.shared.sendNotificationToPlayer(c, "You have to close to the parking spot in order to park the vehicle!");
                }
                else
                    API.shared.sendNotificationToPlayer(c, "You need to be in the vehicle you want to park!");
            }
        }

        /// <summary>
        /// Locks/Unlocks the most nearby vehicle if possible
        /// </summary>
        /// <param name="client"></param>
        public void LockVehicle(Client client)
        {
            RPVehicle nearest = VehicleManager.Instance().GetNearestVehicle(client.position);
            if (nearest != null && client.position.DistanceTo(nearest.position) < doorLockDistance)
            {
                Character c = PlayerManager.Instance().GetPlayerByClient(client).activeCharacter;
                Boolean locked = false;

                if (c.factionID == nearest.factionID && c.onDuty)
                    locked = true;
                else if (c.owner.id == nearest.ownerId)
                    locked = true;

                // check if player has keys to the car 

                if (locked)
                {
                    nearest.ToggleDoorLock();
                    if (nearest.locked)
                        API.shared.sendNotificationToPlayer(client, "Vehicle locked");
                    else
                        API.shared.sendNotificationToPlayer(client, "Vehicle unlocked");
                }
            }

        }

        public void TryExitVehicleShop(Client c)
        {
            Character character = PlayerManager.Instance().GetActiveCharacterForClient(c);
            vehicleShop.ExitShop(character);
        }

        /// <summary>
        /// Loads all vehicles from the database and adds them to the array
        /// </summary>
        public void LoadVehiclesFromDB()
        {
            var cmd = DBManager.SimpleQuery("SELECT * FROM vehicles");
            var reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                RPVehicle v = new RPVehicle(reader.GetInt32(0), reader.GetInt32(1), (FactionI)reader.GetInt32(2), (VehicleHash)reader.GetInt32(3), reader.GetFloat(4), reader.GetFloat(5), reader.GetFloat(6), reader.GetFloat(7), reader.GetFloat(8), reader.GetFloat(9), reader.GetInt32(12), reader.GetInt32(13), reader.GetString(10), true);
                vehicles.Add(v);
            }

            reader.Close();

            // other inits
            vehicleShop.LoadVehiclePricesFromDatabase();
        }
    }
}
