using System;
using System.Collections.Generic;
using GTA_RP.Misc;
using GrandTheftMultiplayer.Server.API;
using GrandTheftMultiplayer.Server.Elements;
using GrandTheftMultiplayer.Shared.Math;
using System.Timers;

namespace GTA_RP.Map
{
    public delegate void OnTimeMinuteChangedEvent(TimeSpan time);

    /// <summary>
    /// Class reponsible for managing things on the map such as fishing zones, blips on minimap etc.
    /// </summary>
    class MapManager : Singleton<MapManager>
    {
        public MapManager() { }
        private List<Blip> staticBlips = new List<Blip>();
        private TimeSpan lastTime;
        private bool hasLastTime = false;
        private Timer minuteTimer = new Timer();
        private FishingHandler fishingHandler = new FishingHandler();
        
        private event OnTimeMinuteChangedEvent OnMinuteChangeEvent;


        /// <summary>
        /// Adds a new static blip on the map
        /// For example vehicle shop and apartment locations should be added with this method
        /// </summary>
        /// <param name="id">Sprite id for blip</param>
        /// <param name="x">X position</param>
        /// <param name="y">Y position</param>
        /// <param name="z">Z position</param>
        public void AddBlipToMap(int id, string name, float x, float y, float z)
        {
            Blip blip = API.shared.createBlip(new Vector3(x, y, z));
            blip.sprite = id;
            blip.name = name;
            staticBlips.Add(blip);
        }

        public void Initialize()
        {
            this.InitializeClock();
            this.InitializeMinuteTimer();
            this.fishingHandler.InitializeFishingSpots();
        }

        private void InitializeClock()
        {
            TimeSpan time = DateTime.Now.TimeOfDay;
            API.shared.setTime(time.Hours, time.Minutes);
        }

        /// <summary>
        /// Initializes the minute timer
        /// </summary>
        public void InitializeMinuteTimer()
        {
            minuteTimer.Interval = (int)TimeSpan.FromMinutes(1).TotalMilliseconds;
            minuteTimer.Elapsed += MinuteChanged;
            minuteTimer.Enabled = true;
        }

        /// <summary>
        /// Starts fishing for character
        /// </summary>
        /// <param name="fisher">Character who is fishing</param>
        public bool StartFishing(Character fisher)
        {
            return this.fishingHandler.StartFishing(fisher);
        }

        /// <summary>
        /// Stops fishing for character
        /// </summary>
        /// <param name="fisher">Character</param>
        public void StopFishing(Character fisher)
        {
            this.fishingHandler.StopFishing(fisher);
        }

        /// <summary>
        /// Onupdate handler for time events
        /// </summary>
        public void MinuteChanged(System.Object source, ElapsedEventArgs args)
        {
            TimeSpan time = DateTime.Now.TimeOfDay;
            API.shared.setTime(time.Hours, time.Minutes);
            OnMinuteChangeEvent.Invoke(time);
            minuteTimer.Enabled = true;
        }

        public void SubscribeToOnMinuteChange(OnTimeMinuteChangedEvent minuteChangedEvent)
        {
            OnMinuteChangeEvent += minuteChangedEvent;
        }
    }
}
