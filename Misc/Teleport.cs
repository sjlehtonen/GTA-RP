using System;
using System.Collections.Generic;
using System.Linq;
using GrandTheftMultiplayer.Server.API;
using GrandTheftMultiplayer.Server.Elements;
using GrandTheftMultiplayer.Server.Managers;
using GrandTheftMultiplayer.Shared;
using GrandTheftMultiplayer.Shared.Math;
using GTA_RP.Misc;

namespace GTA_RP
{
    public struct TeleportDestination
    {
        public int id;
        public Vector3 location;
        public String name;
    }

    public class Teleport : Checkpoint
    {
        private float range = 2.0f;

        public Teleport(int id, Vector3 coordinates, int dimension, Misc.OnEnterCheckpointDelegate enterDelegate, List<TeleportDestination> exits) : base(coordinates, enterDelegate, dimension)
        {
            this.id = id;
            this.exits = new List<TeleportDestination>();
            exits.ForEach(x => this.exits.Add(x));
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
            foreach(TeleportDestination t in exits)
            {
                if(t.id == id)
                    return t;
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
            if (c.position.DistanceTo(shape.Center) <= range)
                return true;

            return false;
        }
    }

}
