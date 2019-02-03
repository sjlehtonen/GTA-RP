using System;
using System.Collections.Generic;
using System.Linq;
using GrandTheftMultiplayer.Shared.Math;
using GTA_RP.Misc;

namespace GTA_RP
{
    /// <summary>
    /// Struct to represent teleport destination with id,
    /// location and name.
    /// </summary>
    public struct TeleportDestination
    {
        public int id;
        public Vector3 location;
        public String name;
    }

    /// <summary>
    /// Class for teleport
    /// </summary>
    public class Teleport : Checkpoint
    {
        private float range = 2.0f;

        public Teleport(int id, Vector3 coordinates, int dimension, Misc.OnEnterCheckpointDelegate enterDelegate, List<TeleportDestination> exits) : base(coordinates, enterDelegate, dimension)
        {
            this.id = id;
            this.exits = new List<TeleportDestination>();
            exits.ForEach(x => this.exits.Add(x));
            // Throw custom exception if there is no exits for teleport
            // Because every teleport has to have at least 1 exit
        }

        public int id { get; private set; }
        public List<TeleportDestination> exits { get; private set; }

        /// <summary>
        /// Gets teleport destination with id
        /// </summary>
        /// <param name="id">Id of teleport destination to get</param>
        /// <returns>Teleport destination</returns>
        private TeleportDestination DestinationWithId(int id)
        {
            foreach (TeleportDestination t in exits)
            {
                if (t.id == id)
                {
                    return t;
                }
            }

            return exits.First();
        }

        // Public methods

        /// <summary>
        /// Teleports character
        /// </summary>
        /// <param name="c">Character to teleport</param>
        public void UseTeleport(Character c)
        {
            c.position = exits.First().location;
        }

        /// <summary>
        /// Teleports character to selected destination
        /// </summary>
        /// <param name="c">Character to teleport</param>
        /// <param name="id">Destination to teleport to</param>
        public void UseTeleport(Character c, int id)
        {
            c.position = DestinationWithId(id).location;
        }

        /// <summary>
        /// Gets ids for all destinations
        /// </summary>
        /// <returns>All destination ids</returns>
        public List<int> GetDestinationIds()
        {
            return exits.Select(h => h.id).ToList();
        }

        /// <summary>
        /// Gets all destination names
        /// </summary>
        /// <returns>All destination names</returns>
        public List<string> GetDestinationNames()
        {
            return exits.Select(h => h.name).ToList();
        }

        /// <summary>
        /// Checks if character is inside teleport
        /// </summary>
        /// <param name="c">Character to check</param>
        /// <returns>True if character is inside teleport, otherwise false</returns>
        public Boolean IsCharacterInsideTeleport(Character c)
        {
            return IsCharacterInsideCheckpoint(c);
        }
    }

}
