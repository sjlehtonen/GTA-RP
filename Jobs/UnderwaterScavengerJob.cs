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
using GTA_RP.Misc;

namespace GTA_RP.Jobs
{
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

        private Vector3[] endPositions =
        {
            new Vector3(529.3144, -3145.132, -1.2811469)
        };

        public UnderwaterScavengerJob(Character c) : base(c) { this.searchTimer = new GTRPTimer(this.TryToFindMaterial, (int)TimeSpan.FromSeconds(10).TotalMilliseconds, true); }

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
                else if (zPos > maxZPos) character.SendNotification("There is no waste this close to the surface. Go deeper.");
                else character.SendNotification("You are too deep to find waste. Go towards the surface.");
            }
        }

        private void UpdateUI()
        {
            this.character.TriggerEvent("EVENT_SET_ASSIST_TEXT", 135, 232, 71, "Waste found: " + this.dangerousMaterialFound + "/" + this.dangerousMaterialRequired, 0);
        }

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
                this.character.SetMoney(this.character.money + 12500);
                this.FinishJob();
            }
        }

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

        private void ToggleUI(bool enabled)
        {
            if (enabled) UpdateUI();
            else this.character.TriggerEvent("EVENT_REMOVE_ASSIST_TEXT", 0);

        }

        private void StartJobTemp()
        {
            if (this.jobStage == 0) this.searchTimer.Start();
            this.ToggleUI(true);
        }

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
