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
using GTA_RP.Misc;

namespace GTA_RP.Jobs
{
    class JobPickUpCheckpoint
    {
        private int jobId;
        private Checkpoint checkpoint;

        /// <summary>
        /// Constructor for JobPickUpCheckpoint
        /// </summary>
        /// <param name="jobId">Job id</param>
        /// <param name="x">X</param>
        /// <param name="y">Y</param>
        /// <param name="z">Z</param>
        public JobPickUpCheckpoint(int jobId, float x, float y, float z)
        {
            this.jobId = jobId;
            this.checkpoint = new Checkpoint(new Vector3(x, y, z), this.CharacterEnteredCheckpoint, this.CharacterExitedCheckpoint);
        }

        /// <summary>
        /// Opens a client side menu for a character
        /// </summary>
        /// <param name="character">Character for who to open the menu</param>
        /// <param name="info">Jobinfo</param>
        private void OpenJobSelectionMenuForCharacter(Character character, JobInfo info)
        {
            API.shared.triggerClientEvent(character.owner.client, "EVENT_OPEN_TAKE_JOB_MENU", this.jobId, info.name, "5000", "1000");
        }

        /// <summary>
        /// Is ran when character walks into a job checkpoint
        /// </summary>
        /// <param name="checkpoint">Checkpoint that character walked into</param>
        /// <param name="character">Character who walked in the checkpoint</param>
        public void CharacterEnteredCheckpoint(Checkpoint checkpoint, Character character)
        {
            JobInfo jobInfo = JobManager.Instance().GetInfoForJobWithId(jobId);
            this.OpenJobSelectionMenuForCharacter(character, jobInfo);
        }

        /// <summary>
        /// Is ran when character walks out of a job checkpoint
        /// </summary>
        /// <param name="checkpoint">Checkpoint that character walked out of</param>
        /// <param name="character">Character who walked out of the checkpoint</param>
        public void CharacterExitedCheckpoint(Checkpoint checkpoint, Character character)
        {
            // Close job pickup menu if open
        }

        /// <summary>
        /// Checks whether character is within the checkpoint
        /// </summary>
        /// <param name="character">Character</param>
        /// <returns>True if character is inside the checkpoint, otherwise false</returns>
        public Boolean IsCharacterInCheckpoint(Character character)
        {
            return checkpoint.IsCharacterInsideCheckpoint(character);
        }
    }
}
