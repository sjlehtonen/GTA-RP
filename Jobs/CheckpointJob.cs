using System;
using System.Collections.Generic;
using GrandTheftMultiplayer.Server.Elements;
using GrandTheftMultiplayer.Server.Constant;
using GrandTheftMultiplayer.Server.Managers;
using GrandTheftMultiplayer.Shared;
using GrandTheftMultiplayer.Shared.Math;

namespace GTA_RP.Jobs
{
    /// <summary>
    /// Job that utilizes checkpoints where players has to go
    /// </summary>
    abstract class CheckpointJob : Job
    {
        private List<ClientCheckpoint> checkpoints = new List<ClientCheckpoint>();
        private int id2 = 0;

        public CheckpointJob(Character c) : base(c)
        {

        }

        /// <summary>
        /// Adds a new checkpoints
        /// </summary>
        /// <param name="coords">Coordinates for the checkpoint</param>
        /// <param name="type">Type of the checkpoint to add</param>
        protected void AddCheckpoint(Vector3 coords, int type)
        {
            ClientCheckpoint c = new ClientCheckpoint(id2, character.owner.client, coords, type, this.OnEnterCheckpoint, this.OnExitCheckpoint);
            checkpoints.Add(c);
            id2++;
        }

        /// <summary>
        /// Returns amount of checkpoints
        /// </summary>
        /// <returns>Amount of checkpoints</returns>
        protected int GetCheckpointCount()
        {
            return checkpoints.Count;
        }

        /// <summary>
        /// Removes a checkpoint
        /// </summary>
        /// <param name="cp">Checkpoint to remove</param>
        protected void RemoveCheckpoint(ClientCheckpoint cp)
        {
            checkpoints.Remove(cp);
            cp.Destroy();
        }

        /// <summary>
        /// Removes all checkpoints
        /// </summary>
        protected void RemoveAllCheckpoints()
        {
            checkpoints.ForEach(c => c.Destroy());
            checkpoints.Clear();
            id2 = 0;
        }

        /// <summary>
        /// Is triggered when player enters a checkpoint
        /// </summary>
        /// <param name="cp">Checkpoint where player(or vehicle) entered</param>
        /// <param name="e">Handle of entered object</param>
        virtual public void OnEnterCheckpoint(ClientCheckpoint cp, NetHandle e) { }

        /// <summary>
        /// Is triggered when player exits a checkpoint
        /// </summary>
        /// <param name="cp">Checkpoint that player(or vehicle) exited</param>
        /// <param name="e">Handle of object that exited</param>
        virtual public void OnExitCheckpoint(ClientCheckpoint cp, NetHandle e) { }
    }
}
