using System;
using GrandTheftMultiplayer.Server.API;
using GrandTheftMultiplayer.Server.Elements;
using GrandTheftMultiplayer.Server.Managers;
using GrandTheftMultiplayer.Shared;
using GrandTheftMultiplayer.Shared.Math;

namespace GTA_RP.Misc
{
    public delegate void OnEnterCheckpointDelegate(Checkpoint checkpoint, Character c);
    public delegate void OnExitCheckpointDelegate(Checkpoint checkpoint, Character c);

    /// <summary>
    /// Class that represents a checkpoint (usually those red things on the ground)
    /// </summary>
    public class Checkpoint
    {
        public Marker entrance { get; private set; }
        public SphereColShape shape { get; private set; }
        private const float range = 1.05f;
        private event OnEnterCheckpointDelegate OnEnterCheckPointEvent;
        private event OnExitCheckpointDelegate OnExitCheckPointEvent;

        private Checkpoint(Vector3 coords, OnEnterCheckpointDelegate enterDelegate, OnExitCheckpointDelegate exitDelegate, int markerType, int dimension, float scaleX, float scaleY, float scaleZ, int alpha, int colorR, int colorG, int colorB, bool popUpAndDown)
        {
            entrance = API.shared.createMarker(markerType, coords, new Vector3(), new Vector3(), new Vector3(scaleX, scaleY, scaleZ), alpha, colorR, colorG, colorB, dimension, popUpAndDown);
            shape = API.shared.createSphereColShape(coords, range * Math.Max(scaleX, scaleY));
            shape.onEntityEnterColShape += EntityEnteredCheckpoint;
            shape.onEntityExitColShape += EntityExitedCheckpoint;
            OnEnterCheckPointEvent += enterDelegate;
            OnExitCheckPointEvent += exitDelegate;
        }

        public Checkpoint(Vector3 coordinates, OnEnterCheckpointDelegate enterDelegate, int dimension = 0)
        {

            entrance = API.shared.createMarker(0, coordinates, new Vector3(), new Vector3(), new Vector3(1, 1, 1), 255, 255, 0, 0, dimension);
            shape = API.shared.createSphereColShape(coordinates, range);
            shape.onEntityEnterColShape += EntityEnteredCheckpoint;
            OnEnterCheckPointEvent += enterDelegate;
        }

        public Checkpoint(Vector3 coordinates)
        {
            entrance = API.shared.createMarker(0, coordinates, new Vector3(), new Vector3(), new Vector3(1, 1, 1), 255, 255, 0, 0, 0);
            shape = API.shared.createSphereColShape(coordinates, range);
        }

        public Checkpoint(Vector3 coordinates, OnEnterCheckpointDelegate enterDelegate, OnExitCheckpointDelegate exitDelegate, int type, int dimension) : this(coordinates, enterDelegate, exitDelegate, type, dimension, 1, 1, 1, 255, 255, 0, 0, false)
        {
        }

        public Checkpoint(Vector3 coordinates, OnEnterCheckpointDelegate enterDelegate, OnExitCheckpointDelegate exitDelegate, int type, float scale, int dimension) : this(coordinates, enterDelegate, exitDelegate, type, dimension, scale, scale, 1, 255, 255, 0, 0, false)
        {
        }

        public Checkpoint(Vector3 coordinates, OnEnterCheckpointDelegate enterDelegate, OnExitCheckpointDelegate exitDelegate, int dimension = 0) : this(coordinates, enterDelegate, exitDelegate, 0, dimension, 1, 1, 1, 255, 255, 0, 0, false)
        {
        }

        public Checkpoint(Vector3 coordinates, OnEnterCheckpointDelegate enterDelegate, OnExitCheckpointDelegate exitDelegate, int type, float scale, int colorR, int colorG, int colorB, int dimension = 0) : this(coordinates, enterDelegate, exitDelegate, type, dimension, scale, scale, 1, 255, colorR, colorG, colorB, false)
        {
        }

        /// <summary>
        /// Removes the checkpoint
        /// </summary>
        public void DestroyCheckpoint()
        {
            entrance.delete();
            API.shared.deleteColShape(shape);
            shape = null;
            entrance = null;
        }

        /// <summary>
        /// Checks if character is inside the checkpoint
        /// </summary>
        /// <param name="character"></param>
        /// <returns></returns>
        public Boolean IsCharacterInsideCheckpoint(Character character)
        {
            return shape.containsEntity(character.client);
        }

        /// <summary>
        /// Ran when player enters the checkpoint
        /// </summary>
        /// <param name="shape">Shape</param>
        /// <param name="entity">Entity, usually player</param>
        protected void EntityEnteredCheckpoint(ColShape shape, NetHandle entity)
        {
            if (OnEnterCheckPointEvent != null)
            {
                Player p = PlayerManager.Instance().PlayerForHandle(entity);
                if (p != null)
                {
                    OnEnterCheckPointEvent.Invoke(this, p.activeCharacter);
                }
            }
        }

        /// <summary>
        /// Ran when player exists the checkpoint
        /// </summary>
        /// <param name="shape">Shape</param>
        /// <param name="entity">Entity, usually player</param>
        protected void EntityExitedCheckpoint(ColShape shape, NetHandle entity)
        {
            if (OnExitCheckPointEvent != null)
            {
                Player p = PlayerManager.Instance().PlayerForHandle(entity);
                if (p != null)
                {
                    OnExitCheckPointEvent.Invoke(this, p.activeCharacter);
                }
            }
        }
    }
}
