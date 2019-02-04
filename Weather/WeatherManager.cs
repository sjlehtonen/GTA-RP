using System;
using System.Linq;
using GrandTheftMultiplayer.Server.API;
using System.Timers;

namespace GTA_RP.Weather
{
    /// <summary>
    /// Class for managing weather
    /// </summary>
    class WeatherManager : Script
    {
        private Random rdn = new Random();
        private Timer weatherTimer = new Timer();
        private int minTime = 3600000 / 2;
        private int maxTime = 3600000 * 3;

        /// <summary>
        /// Generates a random weather id
        /// </summary>
        /// <param name="exclude">Id that is excluded</param>
        /// <returns>Random weather id that isn't excluded id</returns>
        private int GenerateRandomWeatherId(int exclude)
        {
            int a = rdn.Next(0, 9);
            while(a==exclude)
            {
                a = rdn.Next(0, 9);
            }
            return a;
        }

        /// <summary>
        /// Gets random integer from selection
        /// </summary>
        /// <param name="ints">List of integers</param>
        /// <returns>Random integer from list</returns>
        private int GetRandomIntFromSelectedInts(params int[] ints)
        {
            return ints.ElementAt(rdn.Next(0, ints.Count()));
        }

        /// <summary>
        /// Sets random even after random time
        /// </summary>
        /// <param name="source">Timer</param>
        /// <param name="args">Timer arguments</param>
        private void OnTimedEvent(System.Object source, ElapsedEventArgs args)
        {
            SetRandomWeather();
        }

        /// <summary>
        /// Sets random weather
        /// Only allows certain transitions from one weather to another
        /// For example it's not possible to change suddenly from thunder storm into sunny
        /// because it would be unrealistic
        /// </summary>
        private void SetRandomWeather()
        {
            int currentWeather = API.getWeather();
            switch (currentWeather)
            {
                case 8:
                    API.setWeather(GetRandomIntFromSelectedInts(7, 6, 0, 1, 2));
                    break;
                case 7:
                    API.setWeather(GetRandomIntFromSelectedInts(6, 8, 0, 1, 2));
                    break;
                case 6:
                    API.setWeather(GetRandomIntFromSelectedInts(7, 8, 0, 1, 2));
                    break;
                default:
                    API.setWeather(GenerateRandomWeatherId(currentWeather));
                    break;
            }
        }

        /// <summary>
        /// Returns random time between two times
        /// </summary>
        /// <param name="time1">First time</param>
        /// <param name="time2">Second time</param>
        /// <returns>Time between first and second time</returns>
        private int GetRandomTimeBetweenSelectedTimes(int time1, int time2)
        {
            return rdn.Next(time1, time2 + 1);
        }

        /// <summary>
        /// Initializes weather timer
        /// </summary>
        private void InitWeatherTimer()
        {
            weatherTimer.Interval = GetRandomIntFromSelectedInts(this.minTime, this.maxTime);
            weatherTimer.Elapsed += OnTimedEvent;
            weatherTimer.AutoReset = true;
            weatherTimer.Enabled = true;
        }

        public WeatherManager()
        {
            SetRandomWeather();
            InitWeatherTimer();
        }
    }
}
