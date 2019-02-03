using System;
using System.Linq;
using GrandTheftMultiplayer.Server.API;
using GrandTheftMultiplayer.Server.Elements;
using GrandTheftMultiplayer.Shared;
using System.Timers;
using GTA_RP.Vehicles;

namespace GTA_RP.Jobs
{
    /// <summary>
    /// Class that represents a job that is done with a vehicle primarily.
    /// The job is associated to vehicle.
    /// </summary>
    abstract class VehicleJob : CheckpointJob
    {
        private Timer exitVehicleTimer = new Timer();
        private Timer exitJobCooldownTimer = new Timer();
        protected Vehicle workVehicle = null;
        protected Boolean cooldown = false;

        private const int exitVehicleTimeInterval = 15000;
        private const int exitJobTimeInterval = 60000;

        public VehicleJob(Character c) : base(c)
        {

            exitJobCooldownTimer.Elapsed += CoolDownElapsed;
            exitVehicleTimer.Elapsed += ExitVehicleTimerExpire;
            exitVehicleTimer.Interval = exitVehicleTimeInterval;
            exitVehicleTimer.AutoReset = false;
            exitJobCooldownTimer.Interval = exitJobTimeInterval;
        }

        /// <summary>
        /// Ends job if player is too long out of the work vehicle
        /// </summary>
        /// <param name="source">Timer</param>
        /// <param name="e">Timer arguments</param>
        private void ExitVehicleTimerExpire(System.Object source, ElapsedEventArgs e)
        {
            EndJob();
        }

        private void CoolDownElapsed(System.Object source, ElapsedEventArgs e)
        {
            this.cooldown = false;
            this.exitJobCooldownTimer.Stop();
        }

        /// <summary>
        /// Ran when player exits vehicle
        /// Starts timer to enter vehicle again
        /// </summary>
        /// <param name="c">Client who exited vehicle</param>
        /// <param name="vHandle">Vehicle handle</param>
        private void PlayerExitedVehicle(Client c, NetHandle vHandle, int seat)
        {
            if (c.handle == character.owner.client.handle && vHandle == workVehicle.handle)
            {
                character.SendNotification("You have 15 seconds to get back into the work vehicle!");
                exitVehicleTimer.Start();
                this.CharacterExitedWorkVehicle();
            }
        }

        /// <summary>
        /// Ran when player enters vehicle
        /// Stops the timer to enter vehicle
        /// </summary>
        /// <param name="c">Client who entered vehicle</param>
        /// <param name="vHandle">Vehicle handle</param>
        private void PlayerEnteredVehicle(Client c, NetHandle vHandle, int seat)
        {
            if (c.handle == character.owner.client.handle && vHandle == workVehicle.handle)
            {
                exitVehicleTimer.Stop();
                this.CharacterEnteredWorkVehicle();
            }
        }

        /// <summary>
        /// Ran when the work vehicle is destroyed
        /// </summary>
        /// <param name="vHandle">Vehicle handle</param>
        private void JobVehicleDestroyed(NetHandle vHandle)
        {
            if (workVehicle.handle == vHandle)
            {
                character.SendNotification("Vehicle destroyed! Task failed!");
                EndJob();
            }
        }

        /// <summary>
        /// Checks if player is in work vehicle
        /// </summary>
        /// <returns>True if player is in work vehicle, false otherwise</returns>
        protected Boolean IsPlayerInWorkVehicle()
        {
            return this.workVehicle.occupants.Contains(character.owner.client);
        }

        protected Boolean IsPlayerInWorkVehicleAsDriver()
        {
            if (this.workVehicle.occupants.Contains(character.owner.client) && API.shared.getPlayerVehicleSeat(character.client) == -1) { return true; }
            return false;
        }

        protected virtual void CharacterExitedWorkVehicle() { }
        protected virtual void CharacterEnteredWorkVehicle() { }

        private void Clean()
        {
            VehicleManager.Instance().UnsubscribeFromVehicleEnterEvent(this.PlayerEnteredVehicle);
            VehicleManager.Instance().UnsubscribeFromVehicleDestroyedEvent(this.JobVehicleDestroyed);
            VehicleManager.Instance().UnsubscribeFromVehicleExitEvent(this.PlayerExitedVehicle);
            this.workVehicle = null;
            this.RemoveAllCheckpoints();
            this.isActive = false;

        }

        public override void StartJob()
        {
            VehicleManager.Instance().SubscribeToVehicleExitEvent(this.PlayerExitedVehicle);
            VehicleManager.Instance().SubscribeToVehicleDestroyedEvent(this.JobVehicleDestroyed);
            VehicleManager.Instance().SubscribeToVehicleEnterEvent(this.PlayerEnteredVehicle);
            this.workVehicle = character.owner.client.vehicle;
            this.isActive = true;
        }

        public override void EndJob()
        {
            this.Clean();
            this.cooldown = true;
            exitJobCooldownTimer.Start();
        }

        public override void FinishJob()
        {
            this.Clean();
        }

        protected bool IsPlayerInCorrectVehicle(string vehicleNameErrorMessage, params string[] names)
        {
            foreach(string name in names)
            {
                if (this.GetPlayerVehicleHash().Equals(API.shared.vehicleNameToModel(name)))
                {
                    return true;
                }
            }

            character.SendNotification(String.Format("You have to be in a {0} to start the job!", vehicleNameErrorMessage));
            return false;
        }

        protected bool IsOnCooldown()
        {
            if (this.cooldown)
            {
                character.SendNotification("You recently stopped your job and have to wait before starting again");
                return true;
            }
            return false;
        }

    }
}
