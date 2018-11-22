using System;
using System.Collections.Generic;
using GTA_RP.Misc;
using GTA_RP.Items;
using GrandTheftMultiplayer.Shared.Math;
using System.Timers;

namespace GTA_RP.Map
{
    public enum FishingZoneType
    {
        NORMAL = 0,
        WRECKAGE = 1
    }

    public struct FishingSpot
    {
        public float x, y, z;
        public float rotX, rotY, rotZ;
        public float scale;
        private FishingZoneType type;
        public Loot[] loots;

        public FishingSpot(float x, float y, float z, float rotX, float rotY, float rotZ, float scale, FishingZoneType type, Loot[] loots)
        {
            this.type = type;
            this.loots = loots;
            this.x = x;
            this.y = y;
            this.z = z;
            this.rotX = rotX;
            this.rotY = rotY;
            this.rotZ = rotZ;
            this.scale = scale;
        }
    }

    public delegate void FishingZoneDepletedDelegate(FishingZone zone);

    /// <summary>
    /// Class that represents a fishing zone with certain location and certain loot table.
    /// </summary>
    public class FishingZone
    {
        private int id;
        private Random random = new Random();
        private Checkpoint checkpoint;
        private float rotationX, rotationY;
        private int amountOfFish;

        private FishingSpot fishingSpot;
        private LootTable lootTable;

        private bool active = false;

        private Dictionary<Character, Timer> fishingTimers = new Dictionary<Character, Timer>();
        private FishingZoneDepletedDelegate fishingZoneDepletedMethod;

        public FishingZone(FishingZoneDepletedDelegate callback, int id, float x, float y, float z, float rotX, float rotY, float rotZ, float scale, FishingZoneType type, Loot[] loot)
        {
            this.fishingZoneDepletedMethod = callback;
            this.id = id;
            FishingSpot fishingSpot = new FishingSpot(x, y, z, rotX, rotY, rotZ, scale, type, loot);
            this.fishingSpot = fishingSpot;
            lootTable = new LootTable(fishingSpot.loots);
        }

        public override bool Equals(object obj)
        {
            var second = obj as FishingZone;

            if (second == null)
            {
                return false;
            }

            return this.id.Equals(second.id);
        }

        public override int GetHashCode()
        {
            return this.id.GetHashCode();
        }

        private void ReduceFish()
        {
            amountOfFish--;
            if (amountOfFish == 0)
            {
                Despawn();
                this.fishingZoneDepletedMethod.Invoke(this);
            }
        }

        /// <summary>
        /// Spawns the fishing zone if not already spawned
        /// </summary>
        public void Spawn()
        {
            if (!this.active)
            {
                this.active = true;
                this.checkpoint = new Checkpoint(new Vector3(fishingSpot.x, fishingSpot.y, fishingSpot.z), this.CharacterEnteredSpot, this.CharacterExitedSpot, 1, 2.0f, 66, 179, 244);
                this.amountOfFish = this.random.Next(5, 27);
            }
        }

        /// <summary>
        /// Despawns the fishing zone if not already spawned
        /// </summary>
        public void Despawn()
        {
            if (this.active)
            {
                List<Character> toRemove = new List<Character>();

                // Remove all fishers
                foreach (Character c in fishingTimers.Keys)
                {
                    c.SendNotification("Fishing pool depleted!");
                    fishingTimers[c].Enabled = false;
                    FishingRodItem item = c.equippedItem as FishingRodItem;
                    item.StopFishing(c);
                }

                fishingTimers.Clear();

                this.checkpoint.DestroyCheckpoint();
                this.checkpoint = null;
                this.active = false;
            }
        }

        /// <summary>
        /// Checks if the fishing zone is active
        /// </summary>
        /// <returns>True if active, otherwise false</returns>
        public bool IsActive()
        {
            return active;
        }

        /// <summary>
        /// Checks if character is in zone and has correct rotation
        /// </summary>
        /// <param name="character">Character</param>
        /// <returns>True if character in zone with correct rotation, otherwise false</returns>
        public bool IsCharacterInZoneWithCorrectRotation(Character character)
        {
            if (checkpoint == null)
            {
                return false;
            }
            return checkpoint.IsCharacterInsideCheckpoint(character);
        }

        /// <summary>
        /// Ran when character enters the fishing spot
        /// </summary>
        /// <param name="spot">Fishing spot</param>
        /// <param name="character">Character</param>
        public void CharacterEnteredSpot(Checkpoint spot, Character character)
        {
            character.SendChatMessage("[NOTE]: Use your fishing rod to start fishing in this spot");
            character.PlayFrontendSound("SELECT", "HUD_FRONTEND_DEFAULT_SOUNDSET");
        }

        public void CharacterExitedSpot(Checkpoint spot, Character character)
        {
            // Stop fishing if not already stopped
            this.StopFishing(character);
        }

        private void FishingTimerTick(System.Object source, ElapsedEventArgs args, Character character)
        {
            this.Fish(character);
        }

        /// <summary>
        /// Starts the fishing on the spot
        /// </summary>
        /// <param name="character">Fisher</param>
        public void StartFishing(Character character)
        {
            // Check everything
            Timer t = new Timer();
            fishingTimers[character] = t;
            t.Interval = 5000;
            t.Elapsed += (sender, e) => FishingTimerTick(sender, e, character);
            t.Enabled = true;

            // Set character rotation
            character.client.rotation = new Vector3(this.fishingSpot.rotX, this.fishingSpot.rotY, this.fishingSpot.rotZ);

        }

        /// <summary>
        /// Stops the fishing on the spot
        /// </summary>
        /// <param name="character">Fisher</param>
        public void StopFishing(Character character)
        {
            if (fishingTimers.ContainsKey(character))
            {
                fishingTimers[character].Enabled = false;
                fishingTimers.Remove(character);
            }
        }

        /// <summary>
        /// Attemps to catch a fish from the spot
        /// </summary>
        /// <param name="character">Character</param>
        public void Fish(Character character)
        {
            Item item = lootTable.GetLoot();
            if (item == null) character.SendNotification("You failed to catch fish");
            else
            {
                bool ret = character.AddItemToInventory(item, true);
                if (ret)
                {
                    character.SendNotification("You caught a " + item.name);
                    this.ReduceFish();
                }
                else
                {
                    character.SendNotification("Your inventory is full!");
                    this.StopFishing(character);
                }
            }
        }
    }

}
