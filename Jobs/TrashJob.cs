using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GrandTheftMultiplayer.Server.API;
using GrandTheftMultiplayer.Server.Elements;
using GrandTheftMultiplayer.Server.Constant;
using GrandTheftMultiplayer.Server.Managers;
using GrandTheftMultiplayer.Shared;
using GrandTheftMultiplayer.Shared.Math;
using System.Timers;
using GTA_RP.Vehicles;

namespace GTA_RP.Jobs
{
    class TrashJob : CheckpointJob
    {
        public int jobStage { get; private set; }
        private const int jobPointCount = 4;
        private Random rdm = new Random();
        private Boolean cooldown = false;
        private Vehicle workVehicle = null;
        private Timer trashLoadTimer = new Timer();
        private Timer finishJobTimer = new Timer();
        private Timer exitVehicleTimer = new Timer();
        private Timer exitJobCooldownTimer = new Timer();
        private ClientCheckpoint currentCheckPoint = null;

        // Fix Z
        /// <summary>
        /// List of trash pickup points
        /// 4 are picked randomly
        /// </summary>
        private Vector3[] positions =
        {
            new Vector3(-162.9298, -1668.545, 32.64808),
            new Vector3(-189.5532, -1375.702, 30.77444),
            new Vector3(-560.0544, -707.5613, 32.50684),
            new Vector3(-1159.908, -1457.231, 3.861255),
            new Vector3(599.4404, 147.7233, 97.55682),
            new Vector3(-1185.603, -1088.889, 1.813894),
            new Vector3(-1236.442, -1404.753, 3.790302),
            new Vector3(-708.5078, -725.743, 28.21856),
            new Vector3(-1500.967, -887.9347, 9.626642),
            new Vector3(1079.971, -1968.367, 30.556),
            new Vector3(-468.9455, -1729.26, 18.16786),
            new Vector3(-705.4124, -2537.478, 13.51044),
            new Vector3(-1232.402, -692.3838, 23.14487),
            new Vector3(-1296.669, -619.4992, 26.61686),
            new Vector3(-1520.694, -411.7015, 35.027),
            new Vector3(-553.3751, 307.9679, 82.742),
            new Vector3(-277.0289, 204.4182, 85.25507)
        };

        /// <summary>
        /// List of job completion points
        /// One is picked at random
        /// </summary>
        private Vector3[] endPoints =
        {
            new Vector3(453.981, -1965.712, 22.97292),
            new Vector3(-53.35933, -1317.924, 28.98088),
            new Vector3(-451.8873, -1696.857, 22.967725)
        };

        public TrashJob(Character c) : base(c)
        {
            trashLoadTimer.Elapsed += FinishedLoadingTrash;
            finishJobTimer.Elapsed += FinishUnloadingTrash;
            exitVehicleTimer.Elapsed += ExitVehicleTimerExpire;
            exitJobCooldownTimer.Elapsed += EnableJob;

            exitVehicleTimer.Interval = 15000;
            exitVehicleTimer.AutoReset = false;

            exitJobCooldownTimer.Interval = 600000;
            exitVehicleTimer.AutoReset = false;
        }


        private void EnableJob(System.Object source, ElapsedEventArgs e)
        {
            cooldown = true;
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

        /// <summary>
        /// Ran when player exits vehicle
        /// Starts timer to enter vehicle again
        /// </summary>
        /// <param name="c">Client who exited vehicle</param>
        /// <param name="vHandle">Vehicle handle</param>
        private void PlayerExitedVehicle(Client c, NetHandle vHandle)
        {
            if (c.handle == character.owner.client.handle && vHandle == workVehicle.handle)
            {
                API.shared.sendNotificationToPlayer(character.owner.client, "You have 15 seconds to get back into the work vehicle!");
                exitVehicleTimer.Start();
            }
        }

        /// <summary>
        /// Ran when player enters vehicle
        /// Stops the timer to enter vehicle
        /// </summary>
        /// <param name="c">Client who entered vehicle</param>
        /// <param name="vHandle">Vehicle handle</param>
        private void PlayerEnteredVehicle(Client c, NetHandle vHandle)
        {
            if (c.handle == character.owner.client.handle && vHandle == workVehicle.handle)
                exitVehicleTimer.Stop();
        }

        /// <summary>
        /// Cleans up the job when it ends
        /// </summary>
        private void CleanUp()
        {
            VehicleManager.Instance().UnsubscribeFromVehicleEnterEvent(this.PlayerEnteredVehicle);
            VehicleManager.Instance().UnsubscribeFromVehicleDestroyedEvent(this.JobVehicleDestroyed);
            VehicleManager.Instance().UnsubscribeFromVehicleExitEvent(this.PlayerExitedVehicle);
            this.RemoveAllCheckpoints();
            this.isActive = false;
            currentCheckPoint = null;
            workVehicle = null;
            jobStage = 0;
        }

        /// <summary>
        /// Ran when the work vehicle is destroyed
        /// </summary>
        /// <param name="vHandle">Vehicle handle</param>
        private void JobVehicleDestroyed(NetHandle vHandle)
        {
            if (workVehicle.handle == vHandle)
            {
                API.shared.sendNotificationToPlayer(character.owner.client, "Vehicle destroyed! Task failed!");
                EndJob();
            }
        }

        /// <summary>
        /// Checks if player is in work vehicle
        /// </summary>
        /// <returns>True if player is in work vehicle, false otherwise</returns>
        private Boolean IsPlayerInWorkVehicle()
        {
            return this.workVehicle.occupants.Contains(character.owner.client);
        }

        /// <summary>
        /// Checks if player is in trash master vehicle
        /// </summary>
        /// <returns>True if player is in trash master, false otherwise</returns>
        private Boolean IsPlayerInTrashMaster()
        {
            if (this.IsPlayerInVehicle())
            {
                if (this.GetPlayerVehicleHash().Equals(API.shared.vehicleNameToModel("Trash")) || this.GetPlayerVehicleHash().Equals(API.shared.vehicleNameToModel("Trash2")))
                    return true;
            }

            return false;
        }

        /// <summary>
        /// Spawns the trash collection points randomly
        /// </summary>
        private void RandomizeTrashPoints()
        {
            List<int> usedNumbers = new List<int>();

            for (int i = 0; i < jobPointCount; i++)
            {
                int number = rdm.Next(0, positions.Count());

                while (usedNumbers.Contains(number))
                    number = rdm.Next(0, positions.Count());

                this.AddCheckpoint(positions.ElementAt(number), 1);
                usedNumbers.Add(number);
            }
        }

        /// <summary>
        /// Checks if missions is complete and moves to next stage
        /// </summary>
        private void CheckMissionComplete()
        {
            if (GetCheckpointCount() == 0)
            {
                this.jobStage = 1;
                API.shared.sendNotificationToPlayer(character.owner.client, "Task complete! Return to the marked delivery point");
                this.AddCheckpoint(endPoints.ElementAt(rdm.Next(0, endPoints.Count() - 1)), 1);
            }
        }

        /// <summary>
        /// Loads trash from a trash collection point
        /// </summary>
        /// <param name="cp">Trash collection point</param>
        private void LoadTrash(ClientCheckpoint cp)
        {
            workVehicle.freezePosition = true;
            trashLoadTimer.Interval = 5000;
            trashLoadTimer.Enabled = true;
            currentCheckPoint = cp;
            API.shared.sendNotificationToPlayer(character.owner.client, "Loading trash...", true);
        }

        /// <summary>
        /// Unloads trash at selected point
        /// </summary>
        private void UnloadTrash()
        {
            workVehicle.freezePosition = true;
            finishJobTimer.Interval = 5000;
            finishJobTimer.Enabled = true;
            API.shared.sendNotificationToPlayer(character.owner.client, "Unloading trash...", true);
        }

        /// <summary>
        /// Is ran when trash unloading is finished
        /// </summary>
        /// <param name="source">Timer</param>
        /// <param name="args">Timer arguments</param>
        private void FinishUnloadingTrash(System.Object source, ElapsedEventArgs args)
        {
            Timer t = (Timer)source;
            t.Enabled = false;
            workVehicle.freezePosition = false;
            FinishJob();
        }

        // Public methods

        /// <summary>
        /// Begins the job
        /// </summary>
        public override void StartJob()
        {
            if (IsPlayerInTrashMaster())
            {
                if (!cooldown)
                {
                    this.jobStage = 0;
                    RandomizeTrashPoints();
                    workVehicle = character.owner.client.vehicle;
                    this.isActive = true;
                    VehicleManager.Instance().SubscribeToVehicleExitEvent(this.PlayerExitedVehicle);
                    VehicleManager.Instance().SubscribeToVehicleDestroyedEvent(this.JobVehicleDestroyed);
                    VehicleManager.Instance().SubscribeToVehicleEnterEvent(this.PlayerEnteredVehicle);
                }
                else
                {
                    API.shared.sendNotificationToPlayer(character.owner.client, "You recently stopped your job and have to wait before starting again");
                }
            }
            else
            {
                API.shared.sendNotificationToPlayer(character.owner.client, "You have to be in a garbage truck to start the job!");
            }
        }

        /// <summary>
        /// Ends the job (fail)
        /// </summary>
        public override void EndJob()
        {
            cooldown = true;
            exitJobCooldownTimer.Start();
            CleanUp();
        }

        /// <summary>
        /// Finishes the job (success)
        /// </summary>
        public override void FinishJob()
        {
            API.shared.sendNotificationToPlayer(character.owner.client, "Job complete! You earned 5500!");
            CleanUp();
        }

        /// <summary>
        /// Is ran when trash loading is finished
        /// </summary>
        /// <param name="source">Timer</param>
        /// <param name="args">Timer arguments</param>
        public void FinishedLoadingTrash(System.Object source, ElapsedEventArgs args)
        {
            Timer t = (Timer)source;
            t.Enabled = false;
            API.shared.sendNotificationToPlayer(character.owner.client, "Trash loaded", true);
            this.RemoveCheckpoint(currentCheckPoint);
            currentCheckPoint = null;

            workVehicle.freezePosition = false;

            CheckMissionComplete();
        }

        /// <summary>
        /// Is ran when player enters checkpoint
        /// </summary>
        /// <param name="cp">Checkpoint entered</param>
        /// <param name="e">Handle of entered thing, for example vehicle or player</param>
        public override void OnEnterCheckpoint(ClientCheckpoint cp, NetHandle e)
        {
            if (IsPlayerInWorkVehicle())
            {
                Vehicle v = API.shared.getEntityFromHandle<Vehicle>(e);

                if (jobStage == 0)
                    LoadTrash(cp);
                else if (jobStage == 1)
                    UnloadTrash();
            }

        }

        /// <summary>
        /// Is ran when player exits checkpoint
        /// </summary>
        /// <param name="cp">Checkpoint exited</param>
        /// <param name="e">Handle of exited thing, for example vehicle or player</param>
        public override void OnExitCheckpoint(ClientCheckpoint cp, NetHandle e) { }
    }
}
