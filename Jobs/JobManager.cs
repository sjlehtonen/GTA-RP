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
    }

    /// <summary>
    /// Class that handles everything related to jobs
    /// </summary>
    class JobManager : Singleton<JobManager>
    {
        private static JobManager _instance = null;
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
        private JobInfo GetJobInfo(Type jobType, int id, string name, int r, int g, int b)
        {
            JobInfo job;
            job.name = name;
            job.colorR = r;
            job.colorG = g;
            job.colorB = b;
            job.job = jobType;
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
        private void AddJob(Type jobType, int id, string name, int r, int g, int b)
        {
            jobInfo.Add(id, GetJobInfo(jobType, id, name, r, g, b));
        }

        /// <summary>
        /// Initializes all jobs
        /// Add new jobs here
        /// </summary>
        private void InitJobs()
        {
            API.shared.consoleOutput("Initialising jobs...");
            AddJob(typeof(System.Object), 0, "Unemployed", 255, 255, 255);
            AddJob(typeof(TrashJob), 1, "Trash collector", 255, 255, 255);
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
        /// Returns job info for job with certain id
        /// </summary>
        /// <param name="id"></param>
        /// <returns>JobInfo for id</returns>
        public JobInfo GetInfoForJobWithId(int id)
        {
            return jobInfo.Get(id);
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
        /// Checks if character has a job
        /// </summary>
        /// <param name="c">Character to check</param>
        /// <returns>True if character has a job, otherwise false</returns>
        private Boolean DoesCharacterHaveJob(Character c)
        {
            if (c.job != 0)
            {
                return true;
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
        /// Gets instance of the JobManager
        /// </summary>
        /// <returns>Current instance of JobManager</returns>
        /*public static JobManager Instance()
        {
            return _instance;
        }*/

        /// <summary>
        /// Loads jobs for characters
        /// </summary>
        public void LoadJobsForCharacters()
        {

        }
    }
}
