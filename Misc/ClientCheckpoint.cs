using GrandTheftMultiplayer.Server.API;
using GrandTheftMultiplayer.Server.Elements;
using GrandTheftMultiplayer.Server.Managers;
using GrandTheftMultiplayer.Shared;
using GrandTheftMultiplayer.Shared.Math;

namespace GTA_RP
{
    public delegate void OnEnterCheckpointDelegate(ClientCheckpoint cp, NetHandle entity);
    public delegate void OnExitCheckpointDelegate(ClientCheckpoint cp, NetHandle entity);

    /// <summary>
    /// Clientsided checkpoint
    /// Only the player that the checkpoint is assigned to can see it
    /// </summary>
    public class ClientCheckpoint
    {
        private float range = 2.0f;
        public int id { get; private set; }
        public Client client { get; private set; }
        public SphereColShape shape { get; private set; }

        private event OnEnterCheckpointDelegate onEnterCheckpointEvent;
        private event OnExitCheckpointDelegate onExitCheckpointEvent;

        public ClientCheckpoint(int id, Client client, Vector3 coords, int type, OnEnterCheckpointDelegate delegate1, OnExitCheckpointDelegate delegate2)
        {
            this.id = id;
            this.client = client;
            shape = API.shared.createSphereColShape(coords, range);
            shape.onEntityEnterColShape += EntityEnteredCheckpoint;
            shape.onEntityExitColShape += EntityExitedCheckpoint;

            this.onEnterCheckpointEvent += delegate1;
            this.onExitCheckpointEvent += delegate2;

            // Send info to client with triggerClientCommand, need to send id too
            API.shared.triggerClientEvent(client, "EVENT_CREATE_CHECKPOINT", id, coords, type, 255, 0, 0);
        }


        /// <summary>
        /// Ran when entity enters the client checkpoint
        /// </summary>
        /// <param name="shape">Shape</param>
        /// <param name="entity">Entity, usually player</param>
        private void EntityEnteredCheckpoint(ColShape shape, NetHandle entity)
        {
            if (this.onEnterCheckpointEvent != null)
            {
                if ((API.shared.getEntityType(entity) == EntityType.Player && entity == this.client.handle) || (API.shared.getEntityType(entity) == EntityType.Vehicle && API.shared.getPlayerVehicle(client) == entity))
                {
                    this.onEnterCheckpointEvent.Invoke(this, entity);
                }
            }
        }

        /// <summary>
        /// Ran when entity exits the client checkpoint
        /// </summary>
        /// <param name="shape">Shape</param>
        /// <param name="entity">Entity, usually player</param>
        private void EntityExitedCheckpoint(ColShape shape, NetHandle entity)
        {
            if (this.onExitCheckpointEvent != null)
            {
                if ((API.shared.getEntityType(entity) == EntityType.Player && entity == this.client.handle) || (API.shared.getEntityType(entity) == EntityType.Vehicle && API.shared.getPlayerVehicle(client) == entity))
                {
                    this.onExitCheckpointEvent.Invoke(this, entity);
                }
            }
        }

        // Public methods

        /// <summary>
        /// Destroys the checkpoint
        /// </summary>
        public void Destroy()
        {
            API.shared.deleteColShape(shape);
            API.shared.triggerClientEvent(client, "EVENT_DELETE_CHECKPOINT", id);
        }

        /// <summary>
        /// Check if checkpoint equals other checkpoint
        /// </summary>
        /// <param name="obj">Checkpoint</param>
        /// <returns>True if equals, false otherwise</returns>
        public override bool Equals(object obj)
        {
            var second = obj as ClientCheckpoint;

            if (second == null)
            {
                return false;
            }

            return this.id.Equals(second.id);
        }

        /// <summary>
        /// Gets hashcode of id
        /// </summary>
        /// <returns>Hashcode of id</returns>
        public override int GetHashCode()
        {
            return this.id.GetHashCode();
        }

    }
}
