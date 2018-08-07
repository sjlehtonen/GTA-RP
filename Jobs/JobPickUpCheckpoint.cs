using System;
using GrandTheftMultiplayer.Shared.Math;
using GTA_RP.Misc;

namespace GTA_RP.Jobs
{
    /// <summary>
    /// Class that represents a job pickup point (red circle on ground, like checkpoint)
    /// </summary>
    class JobPickUpCheckpoint
    {
        public int jobId { get; private set; }
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
            character.TriggerEvent("EVENT_OPEN_TAKE_JOB_MENU", this.jobId, info.name, info.salary.ToString(), info.description);
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
            character.TriggerEvent("EVENT_CLOSE_TAKE_JOB_MENU");
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
