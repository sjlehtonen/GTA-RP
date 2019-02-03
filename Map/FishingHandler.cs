using System;
using System.Collections.Generic;
using System.Linq;
using GrandTheftMultiplayer.Shared;
using GTA_RP.Misc;

namespace GTA_RP.Map
{

    /// <summary>
    /// Class responsible for handling everything related to fishing.
    /// </summary>
    class FishingHandler
    {
        private List<FishingZone> fishingZones = new List<FishingZone>();
        private const int fishingZoneCount = 5;
        public FishingHandler()
        {
            PlayerManager.Instance().SubscribeToPlayerDisconnectEvent(this.PlayerDisconnected);
            PlayerManager.Instance().SubscribeToCharacterDeathEvent(this.PlayerDied);
        }

        /// <summary>
        /// TODO
        /// </summary>
        private void GenerateFishingSpots()
        {

        }

        private void PlayerDied(Character character, NetHandle killer, int weapon)
        {
            this.StopFishing(character);
        }

        private void PlayerDisconnected(Character character)
        {
            this.StopFishing(character);
        }

        /// <summary>
        /// Adds a new fishing zone
        /// </summary>
        /// <param name="id">id</param>
        /// <param name="x">X</param>
        /// <param name="y">Y</param>
        /// <param name="z">Z</param>
        /// <param name="rotX">X rotation</param>
        /// <param name="rotY">Y rotation</param>
        /// <param name="scale">Scale</param>
        /// <param name="type">Type</param>
        /// <param name="loot">Loot</param>
        private void AddNewFishingZone(int id, float x, float y, float z, float rotX, float rotY, float rotZ, float scale, FishingZoneType type, Loot[] loot)
        {
            fishingZones.Add(new FishingZone(this.SpawnRandomFishingZone, id, x, y, z, rotX, rotY, rotZ, scale, type, loot));
        }

        /// <summary>
        /// Checks if character is in any fishing zone
        /// </summary>
        /// <param name="character">Character</param>
        /// <returns>True if character is in some fishing zone, otherwise false</returns>
        private bool IsCharacterInAnyFishingZone(Character character)
        {
            FishingZone zone = fishingZones.SingleOrDefault(x => x.IsCharacterInZoneWithCorrectRotation(character));
            if (zone == null)
            {
                return false;
            }
            return true;
        }

        /// <summary>
        /// Gets the fishing spot where character is in
        /// </summary>
        /// <param name="character">Character</param>
        /// <returns>Fishing spot or null if character not in any fishing spot</returns>
        private FishingZone GetFishingZoneWhereCharacterIsIn(Character character)
        {
            return fishingZones.Where(x => x.IsCharacterInZoneWithCorrectRotation(character)).DefaultIfEmpty(null).Single();
        }

        /// <summary>
        /// Attemps to catch a fish from some fishing spot
        /// </summary>
        /// <param name="character">Character</param>
        public bool StartFishing(Character character)
        {
            FishingZone zone = GetFishingZoneWhereCharacterIsIn(character);
            if (zone == null)
            {
                return false;
            }
            zone.StartFishing(character);
            return true;
        }

        /// <summary>
        /// Stops fishing for character
        /// </summary>
        /// <param name="character">Character</param>
        public void StopFishing(Character character)
        {
            FishingZone zone = GetFishingZoneWhereCharacterIsIn(character);
            if (zone == null)
            {
                return;
            }
            zone.StopFishing(character);
        }

        public void SpawnRandomFishingZone(FishingZone despawnedZone)
        {
            foreach (FishingZone zone in this.fishingZones)
            {
                if (!despawnedZone.Equals(zone) && !zone.IsActive())
                {
                    zone.Spawn();
                    break;
                }
            }
        }

        /// <summary>
        /// Initializes all fishing spots
        /// </summary>
        public void InitializeFishingSpots()
        {
            // Load loot tables from DB
            Dictionary<FishingZoneType, List<Loot>> loots = new Dictionary<FishingZoneType, List<Loot>>();
            foreach (FishingZoneType type in Enum.GetValues(typeof(FishingZoneType)))
            {
                loots[type] = new List<Loot>();
            }

            DBManager.SelectQuery("SELECT * FROM fishing_loot", (MySql.Data.MySqlClient.MySqlDataReader reader) =>
            {
                int itemId = reader.GetInt32(0);
                int chance = reader.GetInt32(1);
                FishingZoneType type = (FishingZoneType)reader.GetInt32(2);
                loots[type].Add(new Loot(itemId, chance));
            }).Execute();

            DBManager.SelectQuery("SELECT * FROM fishing_spots", (MySql.Data.MySqlClient.MySqlDataReader reader) =>
            {
                FishingZoneType type = (FishingZoneType)reader.GetInt32(1);
                List<Loot> loot = loots[type];
                if (type != FishingZoneType.NORMAL)
                {
                    loot.AddRange(loots[FishingZoneType.NORMAL]);
                }
                this.AddNewFishingZone(reader.GetInt32(0), reader.GetFloat(2), reader.GetFloat(3), reader.GetFloat(4), reader.GetFloat(5), reader.GetFloat(6), reader.GetFloat(7), reader.GetFloat(8), type, loot.ToArray());
            }).Execute();

            // After initialization spawn certain number of spots
            for (int i = 0; i < fishingZones.Count; i++)
            {
                if (i >= fishingZoneCount - 1)
                {
                    break;
                }
                fishingZones[i].Spawn();
            }
        }
    }
}
