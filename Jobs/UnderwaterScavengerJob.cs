using System;
using System.Collections.Generic;
using System.Linq;
using GrandTheftMultiplayer.Shared;
using GrandTheftMultiplayer.Shared.Math;
using GTA_RP.Misc;

namespace GTA_RP.Jobs
{
    /// <summary>
    /// Underwater scavenger job where player drives a small submarine
    /// and tries to find trash underwater, the deeper the player
    /// goes, the easier it is to find trash.
    /// Some things TODO
    /// </summary>
    class UnderwaterScavengerJob : VehicleJob
    {
        private int dangerousMaterialFound = 0;
        private int dangerousMaterialRequired = 1;
        private List<Vector3> checkedSpots = new List<Vector3>();
        private GTRPTimer searchTimer;
        private bool finished = false;

        private const float maxZPos = -15f;
        private const float minZPos = -150f;
        private const float minDist = 10f;
        private Random rnd = new Random();
        private int jobStage = 0;

        private const int salary = 12500;

        /// <summary>
        /// Add possible ending locations here, currently only 1
        /// </summary>
        private Vector3[] endPositions =
        {
            new Vector3(529.3144, -3145.132, -1.2811469)
        };

        public UnderwaterScavengerJob(Character c) : base(c) { this.searchTimer = new GTRPTimer(this.TryToFindMaterial, (int)TimeSpan.FromSeconds(10).TotalMilliseconds, true); }

        /// <summary>
        /// Tries to find hazardous material.
        /// If player is deeper the chance is higher.
        /// Player also has to move from already checked spots.
        /// </summary>
        /// <param name="timer">Not used now</param>
        private void TryToFindMaterial(GTRPTimer timer)
        {
            if (this.IsPlayerInWorkVehicle())
            {
                float zPos = this.workVehicle.position.Z;
                if (zPos <= maxZPos && zPos >= minZPos)
                {
                    if (!checkedSpots.Any(x => x.DistanceTo(this.workVehicle.position) <= minDist))
                    {
                        int chanceToFind = (int)Math.Abs(zPos / 3);
                        int roll = rnd.Next(0, 101);
                        if (roll <= chanceToFind)
                        {
                            character.SendNotification("You discovered some dangerous material!");
                            dangerousMaterialFound++;
                            CheckFinish();
                        }
                        else
                        {
                            character.SendNotification("No waste found, try moving around. Deeper levels have more waste.");
                        }
                        checkedSpots.Add(this.workVehicle.position);
                    }
                }
                else if (zPos > maxZPos)
                {
                    character.SendNotification("There is no waste this close to the surface. Go deeper.");
                }
                else
                {
                    character.SendNotification("You are too deep to find waste. Go towards the surface.");
                }
            }
        }

        /// <summary>
        /// Updates the player HUD
        /// </summary>
        private void UpdateUI()
        {
            this.character.TriggerEvent("EVENT_SET_ASSIST_TEXT", 135, 232, 71, "Waste found: " + this.dangerousMaterialFound + "/" + this.dangerousMaterialRequired, 0);
        }

        /// <summary>
        /// Checks if player finished the job
        /// </summary>
        private void CheckFinish()
        {
            if (dangerousMaterialFound == dangerousMaterialRequired)
            {
                character.SendNotification("Deliver the waste cargo to the marked delivery point");
                SetDeliverPoint();
                this.searchTimer.Stop();
            }
            UpdateUI();
        }


        public override void OnEnterCheckpoint(ClientCheckpoint cp, NetHandle e)
        {
            // Job complete, unload cargo
            if (!finished)
            {
                this.finished = true;
                character.PlayFrontendSound("FLIGHT_SCHOOL_LESSON_PASSED", "HUD_AWARDS");
                this.character.SetMoney(this.character.money + salary);
                this.FinishJob();
            }
        }

        /// <summary>
        /// Sets the delivery point
        /// TODO: make endpoint random
        /// </summary>
        private void SetDeliverPoint()
        {
            this.jobStage = 1;
            this.AddCheckpoint(this.endPositions[0], 1);
        }

        private void CleanUp()
        {
            this.dangerousMaterialFound = 0;
            this.jobStage = 0;
            this.finished = false;
        }

        /// <summary>
        /// Toggles UI on and off
        /// </summary>
        /// <param name="enabled">Set UI on or off</param>
        private void ToggleUI(bool enabled)
        {
            if (enabled) { UpdateUI(); }
            else { this.character.TriggerEvent("EVENT_REMOVE_ASSIST_TEXT", 0); }

        }

        /// <summary>
        /// Starts job temporarily (kind of resumes)
        /// </summary>
        private void StartJobTemp()
        {
            if (this.jobStage == 0)
            {
                this.searchTimer.Start();
            }
            this.ToggleUI(true);
        }

        /// <summary>
        /// Stops job temporarily (kind of pause)
        /// </summary>
        private void StopJobTemp()
        {
            this.searchTimer.Stop();
            this.ToggleUI(false);
        }

        protected override void CharacterExitedWorkVehicle()
        {
            base.CharacterExitedWorkVehicle();
            this.StartJobTemp();
        }

        protected override void CharacterEnteredWorkVehicle()
        {
            base.CharacterEnteredWorkVehicle();
            this.StopJobTemp();
        }


        public override void StartJob()
        {
            if (this.IsPlayerInCorrectVehicle("submercible vehicle", "Submersible", "Submersible2") && !this.IsOnCooldown())
            {
                base.StartJob();
                this.StartJobTemp();
            }
        }

        public override void EndJob()
        {
            base.EndJob();
            this.ToggleUI(false);
            this.CleanUp();
        }

        public override void FinishJob()
        {
            base.FinishJob();
            this.ToggleUI(false);
            this.CleanUp();
        }
    }
}
