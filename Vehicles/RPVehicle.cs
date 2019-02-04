using System;
using GrandTheftMultiplayer.Server.API;
using GrandTheftMultiplayer.Server.Elements;
using GrandTheftMultiplayer.Shared;
using GrandTheftMultiplayer.Shared.Math;
using GTA_RP.Factions;

namespace GTA_RP.Vehicles
{
    /// <summary>
    /// Class for vehicles
    /// All vehicles are supposed to have this class as wrapper
    /// </summary>
    class RPVehicle
    {
        public int id { get; private set; }
        public int ownerId { get; private set; }
        public Vehicle handle { get; private set; }
        public FactionEnums factionID { get; private set; }
        public VehicleHash model { get; private set; }
        public Vector3 parkPosition { get; private set; }
        public Vector3 parkRotation { get; private set; }
        public String licensePlateText { get; private set; }
        public int licensePlateStyle { get; private set; }
        public int color1 { get; private set; }
        public int color2 { get; private set; }

        public Vector3 position
        {
            get { return handle.position; }
            set { handle.position = value; }
        }

        public Boolean locked
        {
            get { return handle.locked; }
            set { handle.locked = value; }
        }

        public Boolean spawned
        {
            get
            {
                if (this.handle != null)
                {
                    return true;
                }

                return false;
            }

            private set { }
        }

        public RPVehicle(int id, int ownerId, FactionEnums factionID, VehicleHash model, float parkx, float parky, float parkz, float parkrotationx, float parkrotationy, float parkrotationz, int color1, int color2, String licensePlateText = "", Boolean spawn = true)
        {
            this.id = id;
            this.ownerId = ownerId;
            this.factionID = factionID;
            this.model = model;
            this.parkPosition = new Vector3(parkx, parky, parkz);
            this.parkRotation = new Vector3(parkrotationx, parkrotationy, parkrotationz);
            this.licensePlateText = licensePlateText;
            this.licensePlateStyle = 0;
            this.color1 = color1;
            this.color2 = color2;
            this.handle = null;

            if (spawn && ownerId < 0)
            {
                this.Spawn();
                this.ToggleDoorLock();
            }

            if (!licensePlateText.Equals("") && this.spawned)
            {
                this.handle.numberPlate = licensePlateText;
            }

            VehicleManager.Instance().SubscribeToVehicleDestroyedEvent(this.VehicleDestroyedEvent);
        }

        /// <summary>
        /// Called when vehicle is destroyed
        /// </summary>
        /// <param name="vehicle">Vehicle handle</param>
        private void VehicleDestroyedEvent(NetHandle vehicle)
        {
            if (spawned)
            {
                if (vehicle == handle)
                {
                    handle = null;
                }
            }
        }

        /// <summary>
        /// Updates the vehicle's parking position
        /// </summary>
        /// <param name="newPos">New park position</param>
        /// <param name="newRot">New park position rotation</param>
        public void UpdateParkPosition(Vector3 newPos, Vector3 newRot)
        {
            this.parkPosition = newPos;
            this.parkRotation = newRot;
        }

        /// <summary>
        /// Gets hashcode for vehicle
        /// </summary>
        /// <returns>Vehicle id</returns>
        public override int GetHashCode()
        {
            return this.id;
        }

        /// <summary>
        /// Checks if two vehicles are the same
        /// </summary>
        /// <param name="obj">Other vehicle</param>
        /// <returns>True if vehicles are the same, otherwise false</returns>
        public override bool Equals(object obj)
        {
            var second = obj as RPVehicle;

            if (second == null)
            {
                return false;
            }

            return this.id.Equals(second.id);
        }

        /// <summary>
        /// Toggles the lock of the vehicle
        /// </summary>
        public void ToggleDoorLock()
        {
            if (spawned)
            {
                handle.locked = !handle.locked;
            }
        }

        /// <summary>
        /// Spawns the vehicle
        /// </summary>
        public void Spawn()
        {
            handle = API.shared.createVehicle(model, parkPosition, parkRotation, color1, color2);
            API.shared.setVehicleNumberPlate(handle, this.licensePlateText);
            API.shared.setVehicleNumberPlateStyle(handle, this.licensePlateStyle);
        }

        /// <summary>
        /// Parks the vehicle and despawns it
        /// </summary>
        public void Park()
        {
            if (spawned)
            {
                API.shared.deleteEntity(handle);
                handle = null;
            }
        }
    }
}
