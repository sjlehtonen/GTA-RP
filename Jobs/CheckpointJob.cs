using System.Collections.Generic;
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
        private int checkpointId = 0;

        public CheckpointJob(Character character) : base(character)
        {

        }

        /// <summary>
        /// Adds a new checkpoints
        /// </summary>
        /// <param name="coords">Coordinates for the checkpoint</param>
        /// <param name="type">Type of the checkpoint to add</param>
        protected void AddCheckpoint(Vector3 coords, int type)
        {
            ClientCheckpoint checkpoint = new ClientCheckpoint(checkpointId, character.owner.client, coords, type, this.OnEnterCheckpoint, this.OnExitCheckpoint);
            checkpoints.Add(checkpoint);
            checkpointId++;
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
        /// <param name="checkpoint">Checkpoint to remove</param>
        protected void RemoveCheckpoint(ClientCheckpoint checkpoint)
        {
            checkpoints.Remove(checkpoint);
            checkpoint.Destroy();
        }

        /// <summary>
        /// Removes all checkpoints
        /// </summary>
        protected void RemoveAllCheckpoints()
        {
            checkpoints.ForEach(c => c.Destroy());
            checkpoints.Clear();
            checkpointId = 0;
        }

        /// <summary>
        /// Is triggered when player enters a checkpoint
        /// </summary>
        /// <param name="checkpoint">Checkpoint where player(or vehicle) entered</param>
        /// <param name="handle">Handle of entered object</param>
        virtual public void OnEnterCheckpoint(ClientCheckpoint checkpoint, NetHandle handle) { }

        /// <summary>
        /// Is triggered when player exits a checkpoint
        /// </summary>
        /// <param name="checkpoint">Checkpoint that player(or vehicle) exited</param>
        /// <param name="handle">Handle of object that exited</param>
        virtual public void OnExitCheckpoint(ClientCheckpoint checkpoint, NetHandle handle) { }
    }
}
