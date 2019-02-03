using System;
using System.Collections.Generic;
using System.Linq;
using GrandTheftMultiplayer.Shared;
using GrandTheftMultiplayer.Shared.Math;
using System.Timers;

namespace GTA_RP.Jobs
{
    class TrashJob : VehicleJob
    {
        public int jobStage { get; private set; }
        private const int jobPointCount = 1;
        private Random rdm = new Random();

        private Timer trashLoadTimer = new Timer();
        private Timer finishJobTimer = new Timer();

        private ClientCheckpoint currentCheckPoint = null;
        private const int salary = 5500;
        private const int loadTrashTime = 5000;

        /// <summary>
        /// List of trash pickup points
        /// 4 are picked randomly
        /// </summary>
        private Vector3[] positions =
        {
            new Vector3(-162.9298, -1668.545, 31.64808),
            new Vector3(-189.5532, -1375.702, 29.77444),
            new Vector3(-560.0544, -707.5613, 31.50684),
            new Vector3(-1159.908, -1457.231, 2.861255),
            new Vector3(599.4404, 147.7233, 96.55682),
            new Vector3(-1185.603, -1088.889, 0.813894),
            new Vector3(-1236.442, -1404.753, 2.790302),
            new Vector3(-708.5078, -725.743, 27.21856),
            new Vector3(-1500.967, -887.9347, 8.626642),
            new Vector3(1079.971, -1968.367, 29.556),
            new Vector3(-468.9455, -1729.26, 17.16786),
            new Vector3(-705.4124, -2537.478, 12.51044),
            new Vector3(-1232.402, -692.3838, 22.14487),
            new Vector3(-1296.669, -619.4992, 25.61686),
            new Vector3(-1520.694, -411.7015, 34.027),
            new Vector3(-553.3751, 307.9679, 81.742),
            new Vector3(-277.0289, 204.4182, 84.25507)
        };

        /// <summary>
        /// List of job completion points
        /// One is picked at random
        /// </summary>
        private Vector3[] endPoints =
        {
            new Vector3(453.981, -1965.712, 21.97292),
            new Vector3(-53.35933, -1317.924, 27.98088),
            new Vector3(-451.8873, -1696.857, 21.967725)
        };

        public TrashJob(Character c) : base(c)
        {
            trashLoadTimer.Elapsed += FinishedLoadingTrash;
            finishJobTimer.Elapsed += FinishUnloadingTrash;
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
        /// Cleans up the job when it ends
        /// </summary>
        private void CleanUp()
        {
            currentCheckPoint = null;
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
                character.SendNotification("Vehicle destroyed! Task failed!");
                EndJob();
            }
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
                {
                    number = rdm.Next(0, positions.Count());
                }

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
                character.SendNotification("Task complete! Return to the marked delivery point");
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
            trashLoadTimer.Interval = loadTrashTime;
            trashLoadTimer.Enabled = true;
            currentCheckPoint = cp;
            character.SendNotification("Loading trash...");
        }

        /// <summary>
        /// Unloads trash at selected point
        /// </summary>
        private void UnloadTrash()
        {
            workVehicle.freezePosition = true;
            finishJobTimer.Interval = loadTrashTime;
            finishJobTimer.Enabled = true;
            character.SendNotification("Unloading trash...");
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
            character.PlayFrontendSound("FLIGHT_SCHOOL_LESSON_PASSED", "HUD_AWARDS");
            FinishJob();
        }

        // Public methods

        /// <summary>
        /// Begins the job
        /// </summary>
        public override void StartJob()
        {
            if (IsPlayerInCorrectVehicle("garbage truck", "Trash", "Trash2") && !this.IsOnCooldown())
            {
                base.StartJob();
                this.jobStage = 0;
                RandomizeTrashPoints();
            }
        }

        /// <summary>
        /// Ends the job (fail)
        /// </summary>
        public override void EndJob()
        {
            base.EndJob();
            CleanUp();
        }

        /// <summary>
        /// Finishes the job (success)
        /// </summary>
        public override void FinishJob()
        {
            base.FinishJob();
            character.SendNotification("Job complete! You earned $5500!");
            character.SetMoney(character.money + salary);
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
            character.SendNotification("Trash loaded");
            this.RemoveCheckpoint(currentCheckPoint);
            currentCheckPoint = null;
            workVehicle.freezePosition = false;
            character.PlayFrontendSound("SELECT", "HUD_LIQUOR_STORE_SOUNDSET");
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
                if (jobStage == 0) { LoadTrash(cp); }
                else if (jobStage == 1) { UnloadTrash(); }
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
