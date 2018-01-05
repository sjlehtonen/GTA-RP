using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GrandTheftMultiplayer.Server.API;
using GrandTheftMultiplayer.Server.Elements;
using GrandTheftMultiplayer.Server.Managers;
using GrandTheftMultiplayer.Shared;
using GrandTheftMultiplayer.Shared.Math;

namespace GTA_RP.Misc
{
    public delegate void OnEnterCheckpointDelegate(Checkpoint checkpoint, Character c);
    public delegate void OnExitCheckpointDelegate(Checkpoint checkpoint, Character c);

    public class Checkpoint
    {
        public Marker entrance { get; private set; }
        public SphereColShape shape { get; private set; }
        private float range = 1.2f;
        private event OnEnterCheckpointDelegate OnEnterCheckPointEvent;
        private event OnExitCheckpointDelegate OnExitCheckPointEvent;

        private Checkpoint(Vector3 coords, OnEnterCheckpointDelegate enterDelegate, OnExitCheckpointDelegate exitDelegate, int markerType, int dimension, float scaleX, float scaleY, float scaleZ, int alpha, int colorR, int colorG, int colorB, bool popUpAndDown)
        {
            entrance = API.shared.createMarker(markerType, coords, new Vector3(), new Vector3(), new Vector3(scaleX, scaleY, scaleZ), alpha, colorR, colorG, colorB, dimension, popUpAndDown);
            shape = API.shared.createSphereColShape(coords, range*Math.Max(scaleX, scaleY));
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

        public Checkpoint(Vector3 coordinates, OnEnterCheckpointDelegate enterDelegate, OnExitCheckpointDelegate exitDelegate, int type, int dimension) : this(coordinates, enterDelegate, exitDelegate, type, dimension, 1, 1, 1, 255, 255, 0, 0, false)
        {
        }

        public Checkpoint(Vector3 coordinates, OnEnterCheckpointDelegate enterDelegate, OnExitCheckpointDelegate exitDelegate, int type, float scale, int dimension) : this(coordinates, enterDelegate, exitDelegate, type, dimension, scale, scale, 1, 255, 255, 0, 0, false)
        {
        }

        public Checkpoint(Vector3 coordinates, OnEnterCheckpointDelegate enterDelegate, OnExitCheckpointDelegate exitDelegate, int dimension = 0) : this(coordinates, enterDelegate, exitDelegate, 0, dimension, 1, 1, 1, 255, 255, 0, 0, false)
        {
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
                if (p != null) OnEnterCheckPointEvent.Invoke(this, p.activeCharacter);
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
                if (p != null) OnExitCheckPointEvent.Invoke(this, p.activeCharacter);
            }
        }
    }
}
