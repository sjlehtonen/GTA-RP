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

        public Checkpoint(Vector3 coordinates, OnEnterCheckpointDelegate enterDelegate, int dimension = 0)
        {
            entrance = API.shared.createMarker(0, coordinates, new Vector3(), new Vector3(), new Vector3(1, 1, 1), 255, 255, 0, 0, dimension);
            shape = API.shared.createSphereColShape(coordinates, range);
            shape.onEntityEnterColShape += EntityEnteredCheckpoint;
            OnEnterCheckPointEvent += enterDelegate;
        }

        public Checkpoint(Vector3 coordinates, OnEnterCheckpointDelegate enterDelegate, OnExitCheckpointDelegate exitDelegate, int dimension = 0)
        {
            entrance = API.shared.createMarker(0, coordinates, new Vector3(), new Vector3(), new Vector3(1, 1, 1), 255, 255, 0, 0, dimension);
            shape = API.shared.createSphereColShape(coordinates, range);
            shape.onEntityEnterColShape += EntityEnteredCheckpoint;
            OnEnterCheckPointEvent += enterDelegate;
            OnExitCheckPointEvent += exitDelegate;
        }

        /// <summary>
        /// Checks if character is inside the checkpoint
        /// </summary>
        /// <param name="character"></param>
        /// <returns></returns>
        public Boolean IsCharacterInsideCheckpoint(Character character)
        {
            if (character.position.DistanceTo(shape.Center) <= range)
                return true;

            return false;
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
