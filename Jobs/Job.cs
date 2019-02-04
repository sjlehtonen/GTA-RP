using System;
using GrandTheftMultiplayer.Server.API;
using GrandTheftMultiplayer.Server.Elements;
using GrandTheftMultiplayer.Shared;

namespace GTA_RP.Jobs
{
    abstract class Job
    {
        public int id { get; private set; }
        protected Character character;
        public Boolean isActive { get; protected set; }

        /// <summary>
        /// Starts the job
        /// Always ran at job startup
        /// </summary>
        abstract public void StartJob();

        /// <summary>
        /// Ends the job
        /// Always ran when job is stopped before finishing it
        /// </summary>
        abstract public void EndJob();

        /// <summary>
        /// Finishes the job
        /// Ran when job finishes succesfully
        /// </summary>
        abstract public void FinishJob();


        public Job(Character character)
        {
            this.character = character;
            PlayerManager.Instance().SubscribeToPlayerDisconnectEvent(this.PlayerDisconnected);
        }

        private void PlayerDisconnected(Character character)
        {
            if (character == this.character)
            {
                EndJob();
                PlayerManager.Instance().UnsubscribeFromPlayerDisconnectEvent(this.PlayerDisconnected);
            }
        }

        /// <summary>
        /// Checks if player is in vehicle
        /// </summary>
        /// <param name="c">Client to check</param>
        /// <returns>True if player is in vehicle, otherwise false</returns>
        protected Boolean IsPlayerInVehicle()
        {
            return character.owner.client.isInVehicle;
        }

        /// <summary>
        /// Rewards money to the player who is on the job
        /// </summary>
        /// <param name="money"></param>
        protected void RewardMoney(int money)
        {

        }

        /// <summary>
        /// Gets VehicleHash for vehicle the player is in
        /// </summary>
        /// <param name="c">Client to check</param>
        /// <returns>VehicleHash for vehicle player is in</returns>
        protected VehicleHash GetPlayerVehicleHash()
        {
            NetHandle handle = API.shared.getPlayerVehicle(character.owner.client);
            Vehicle vehicle = API.shared.getEntityFromHandle<Vehicle>(handle);
            return (VehicleHash)vehicle.model;
        }

    }
}
