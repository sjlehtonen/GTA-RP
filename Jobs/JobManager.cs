using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GrandTheftMultiplayer.Server.API;
using GrandTheftMultiplayer.Server.Elements;
using GrandTheftMultiplayer.Shared;
using GrandTheftMultiplayer.Shared.Math;
using GTA_RP.Misc;
using GTA_RP.Map;

namespace GTA_RP.Jobs
{
    /// <summary>
    /// Job information structure
    /// </summary>
    struct JobInfo
    {
        public string name;
        public int colorR;
        public int colorG;
        public int colorB;
        public Type job;
        public int salary;
        public string description;
    }

    /// <summary>
    /// Class that handles everything related to jobs
    /// </summary>
    class JobManager : Singleton<JobManager>
    {
        private static JobManager _instance = null;
        private List<JobPickUpCheckpoint> jobPickupPoints = new List<JobPickUpCheckpoint>();
        private Dictionary<int, Job> jobsForCharacterId = new Dictionary<int, Job>();
        private Dictionary<int, JobInfo> jobInfo = new Dictionary<int, JobInfo>();

        /// <summary>
        /// Creates and returns a JobInfo object
        /// </summary>
        /// <param name="jobType">Type of Job (class)</param>
        /// <param name="id">Id of job</param>
        /// <param name="name">Name of job</param>
        /// <param name="r">Red color value</param>
        /// <param name="g">Green color value</param>
        /// <param name="b">Blue color value</param>
        /// <returns>JobInfo object</returns>
        private JobInfo GetJobInfo(Type jobType, int id, string name, int r, int g, int b, int salary, string description)
        {
            JobInfo job;
            job.name = name;
            job.colorR = r;
            job.colorG = g;
            job.colorB = b;
            job.job = jobType;
            job.salary = salary;
            job.description = description;
            return job;
        }

        /// <summary>
        /// Adds a new job to the server
        /// </summary>
        /// <param name="jobType">Type of job</param>
        /// <param name="id">If of job</param>
        /// <param name="name">Name of job</param>
        /// <param name="r">Red color value</param>
        /// <param name="g">Green color value</param>
        /// <param name="b">Blue color value</param>
        private void AddJob(Type jobType, int id, string name, int r, int g, int b, int salary, string description)
        {
            jobInfo.Add(id, GetJobInfo(jobType, id, name, r, g, b, salary, description));
        }

        /// <summary>
        /// Initializes all jobs
        /// Add new jobs here
        /// </summary>
        private void InitJobs()
        {
            AddJob(typeof(System.Object), 0, "Unemployed", 255, 255, 255, 0, "Unemployed");
            AddJob(typeof(TrashJob), 1, "Trash Collector", 255, 255, 255, 2500, "A job where you drive a garbage truck around and pick up trash");
        }


        /// <summary>
        /// Creates a job for character based on the id
        /// </summary>
        /// <param name="c">Character for which to initialize job</param>
        /// <returns>Created Job or null if there is no such job</returns>
        private Job CreateJobForId(Character c)
        {
            if (jobInfo.ContainsKey(c.job) && c.job != 0)
                return Activator.CreateInstance(jobInfo.Get(c.job).job, c) as Job;

            return null;
        }

        /// <summary>
        /// Checks if a job with id is valid
        /// </summary>
        /// <param name="jobId">Job id</param>
        /// <returns>Whether job is valid or not</returns>
        private bool IsJobValid(int jobId)
        {
            return jobInfo.ContainsKey(jobId);
        }

        /// <summary>
        /// Returns job info for job with certain id
        /// </summary>
        /// <param name="id"></param>
        /// <returns>JobInfo for id</returns>
        public JobInfo GetInfoForJobWithId(int id)
        {
            return jobInfo.Get(id);
        }

        /// <summary>
        /// Checks if character has a job
        /// </summary>
        /// <param name="c">Character to check</param>
        /// <returns>True if character has a job, otherwise false</returns>
        public Boolean DoesCharacterHaveJob(Character c)
        {
            if (c.job != 0)
                return true;
            return false;
        }

        /// <summary>
        /// Checks if job is set for character
        /// </summary>
        /// <param name="c">Character to check</param>
        /// <returns>True if job is set, false otherwise</returns>
        private Boolean IsJobSetForCharacter(Character c)
        {
            return jobsForCharacterId.ContainsKey(c.ID);
        }

        /// <summary>
        /// Creates a job object for character if it doesn't exist yet
        /// </summary>
        /// <param name="c">Character to create job for</param>
        private void CreateJobForCharacter(Character c)
        {
            if (!IsJobSetForCharacter(c))
                jobsForCharacterId.Add(c.ID, this.CreateJobForId(c));
        }

        /// <summary>
        /// Removes a job from character
        /// </summary>
        /// <param name="c">Character</param>
        private void RemoveJobFromCharacter(Character c)
        {
            if (IsJobSetForCharacter(c))
                jobsForCharacterId.Remove(c.ID);
        }

        /// <summary>
        /// Checks if character is in the job pickup point for some job
        /// </summary>
        /// <param name="character">Character</param>
        /// <param name="jobId">Job id</param>
        /// <returns></returns>
        private bool IsCharacterInJobPickUpPointForJob(Character character, int jobId)
        {
            foreach(JobPickUpCheckpoint point in jobPickupPoints)
            {
                if (point.jobId == jobId && point.IsCharacterInCheckpoint(character))
                    return true;
            }
            return false;
        }

        /// <summary>
        /// Creates a new job pickup point
        /// </summary>
        /// <param name="jobId">Id of job</param>
        /// <param name="x">X</param>
        /// <param name="y">Y</param>
        /// <param name="z">Z</param>
        private void CreateJobPickupPointForJob(int jobId, float x, float y, float z, int blipId = 408)
        {
            this.jobPickupPoints.Add(new JobPickUpCheckpoint(jobId, x, y, z));
            JobInfo info = this.GetInfoForJobWithId(jobId);
            MapManager.Instance().AddBlipToMap(blipId, info.name, x, y, z);
        }

        /// <summary>
        /// Updates HUD for character's job
        /// </summary>
        /// <param name="character">Character</param>
        /// <param name="jobId">Job id</param>
        private void UpdateClientJobHUD(Character character)
        {
            JobInfo job = GetInfoForJobWithId(character.job);
            API.shared.triggerClientEvent(character.owner.client, "EVENT_UPDATE_JOB", job.name, job.colorR, job.colorG, job.colorB);
        }

        /// <summary>
        /// Sets a job for character and updates the database
        /// </summary>
        /// <param name="character">Character</param>
        /// <param name="jobId">Job id</param>
        private void SetJobForCharacter(Character character, int jobId)
        {
            if (IsJobActiveForCharacter(character))
            {
                API.shared.sendNotificationToPlayer(character.owner.client, "You have to stop your currrent job first!");
                return;
            }

            if (!IsJobValid(jobId))
            {
                API.shared.sendNotificationToPlayer(character.owner.client, "Invalid job id!");
                return;
            }

            if (!IsCharacterInJobPickUpPointForJob(character, jobId))
            {
                API.shared.sendNotificationToPlayer(character.owner.client, "You have to be at the job pickup point!");
                return;
            }

            // Add check for job timer

            character.SetJob(jobId);
            DBManager.UpdateQuery("UPDATE characters set job=@job_id WHERE id=@character_id")
                .AddValue("@job_id", jobId)
                .AddValue("@character_id", character.ID)
                .Execute();
            RemoveJobFromCharacter(character);
            CreateJobForCharacter(character);
            UpdateClientJobHUD(character);
            API.shared.sendChatMessageToPlayer(character.owner.client, "Your new job is: " + GetInfoForJobWithId(jobId).name);
        }

        /// <summary>
        /// Checks if job is active for some character
        /// </summary>
        /// <param name="c">Character</param>
        /// <returns>True if job is active, otherwise false</returns>
        private bool IsJobActiveForCharacter(Character c)
        {
            if (DoesCharacterHaveJob(c))
            {
                Job j = jobsForCharacterId.Get(c.ID);
                if (j.isActive) return true;
            }

            return false;
        }
        

        /// <summary>
        /// Starts a job for character
        /// </summary>
        /// <param name="c">Character to start job for</param>
        public void StartJobForCharacter(Character c)
        {
            if (DoesCharacterHaveJob(c))
            {
                CreateJobForCharacter(c);
                jobsForCharacterId.Get(c.ID).StartJob();
            }
        }

        /// <summary>
        /// Stops job for character
        /// </summary>
        /// <param name="c">Character to stop job for</param>
        public void StopJobForCharacter(Character c)
        {
            if (DoesCharacterHaveJob(c))
            {
                Job j = jobsForCharacterId.Get(c.ID);
                if (j.isActive)
                    j.EndJob();
            }
        }

        public JobManager()
        {
            if (_instance == null)
            {
                _instance = this;
                 InitJobs();
            }
        }

        /// <summary>
        /// Takes a job for client
        /// </summary>
        /// <param name="client">Client</param>
        /// <param name="jobId">Job id</param>
        public void TakeJobForClient(Character character, int jobId)
        {
            // 1. Remove old job
            // 2. Set new job
            // 3. Save new job to DB
            SetJobForCharacter(character, jobId);

        }

        /// <summary>
        /// Initialize all job pickup points
        /// Places markers on map where players can take jobs
        /// </summary>
        public void InitJobPickupPoints()
        {
            this.CreateJobPickupPointForJob(1, -574.9212f, -1779.259f, 22.47824f);
        }
    }
}
