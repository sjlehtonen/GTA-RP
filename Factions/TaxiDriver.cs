﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GTA_RP.Vehicles;
using GrandTheftMultiplayer.Server.Constant;
using GrandTheftMultiplayer.Server.API;
using GrandTheftMultiplayer.Server.Elements;
using GrandTheftMultiplayer.Shared;
using GrandTheftMultiplayer.Shared.Math;
using GTA_RP.Map;
using GTA_RP.Misc;

namespace GTA_RP.Factions
{
    class TaxiMeter
    {
        public int setterId;
        public GTRPTimer taxiTimer;
        public int money;

        public TaxiMeter(int setterId, int money, GTRPTimer timer)
        {
            this.setterId = setterId;
            this.money = money;
            this.taxiTimer = timer;
        }

        public Character setter { get { return PlayerManager.Instance().GetCharacterWithId(this.setterId); } }
    }

    class TaxiDriver : RankedFaction
    {
        private Dictionary<int, Dictionary<int, TaxiMeter>> taxiMeters = new Dictionary<int, Dictionary<int, TaxiMeter>>(); // Car ID maps to character ids
        public TaxiDriver(FactionI id, string name, int colorR, int colorG, int colorB) : base(id, name, colorR, colorG, colorB) { }

        // TODO: Switch so that each taxi driver can adjust there individually
        private int taxiFeeInterval = (int)TimeSpan.FromSeconds(60).TotalMilliseconds;
        private const int taxiFee = 100;
        private const int taxiStartingFee = 200;

        private void StopTaxiMeterForCharacterInTaxi(Character character, int taxiId)
        {
            GetTaxiMeterForCharacterInTaxi(character, taxiId).taxiTimer.Stop();
        }

        private void RestartTaxiMeterForCharacterInTaxi(Character character, int taxiId)
        {
            GetTaxiMeterForCharacterInTaxi(character, taxiId).taxiTimer.Start();
        }

        private void RemoveTaxiMeterFromCharacterInTaxi(Character character, int taxiId)
        {
            StopTaxiMeterForCharacterInTaxi(character, taxiId);
            taxiMeters[taxiId].Remove(character.ID);
            DisableTaxiMeterHUDForCharacter(character);
        }

        /// <summary>
        /// Checks if taxi meter is set for a character
        /// </summary>
        /// <param name="character">Character</param>
        /// <param name="carId">ID of the taxi</param>
        /// <returns>If taxi meter is set, yes, otherwise no</returns>
        private bool IsTaxiMeterSetForCharacterOnCar(Character character, int carId)
        {
            if (taxiMeters.ContainsKey(carId)) return taxiMeters[carId].ContainsKey(character.ID);
            return false;
        }

        private void CharacterEnteredTaxi(Character c, NetHandle vehicleHandle, int seat)
        {
            if (VehicleManager.Instance().DoesVehicleHandleHaveRPVehicle(vehicleHandle))
            {
                RPVehicle vehicle = VehicleManager.Instance().GetVehicleForHandle(vehicleHandle);
                if (IsTaxiMeterSetForCharacterOnCar(c, vehicle.id))
                {
                    SetTaxiMeterHUDForCharacter(c, vehicle.id);
                }
            }
        }

        private void CharacterExitedTaxi(Character c, NetHandle vehicleHandle, int seat)
        {
            if (VehicleManager.Instance().DoesVehicleHandleHaveRPVehicle(vehicleHandle))
            {
                RPVehicle vehicle = VehicleManager.Instance().GetVehicleForHandle(vehicleHandle);
                if (IsTaxiMeterSetForCharacterOnCar(c, vehicle.id))
                {
                    DisableTaxiMeterHUDForCharacter(c);
                }
            }
        }

        private TaxiMeter GetTaxiMeterForCharacterInTaxi(Character c, int taxiId)
        {
            if (!taxiMeters.ContainsKey(taxiId)) return null;
            if (!taxiMeters[taxiId].ContainsKey(c.ID)) return null;
            return taxiMeters[taxiId][c.ID];
        }

        private int GetTaxiFeeForCharacterInTaxi(Character c, int taxiId)
        {
            return taxiMeters[taxiId][c.ID].money;
        }

        public override string GetChatColor()
        {
            return "~y~";
        }

        public void PayTaxiMeterFee(Character payer)
        {
            // Give money to driver
            // Remove taxi meter
            RPVehicle v = VehicleManager.Instance().GetVehicleForCharacter(payer);
            TaxiMeter meter = GetTaxiMeterForCharacterInTaxi(payer, v.id);
            if (meter == null)
            {
                payer.TriggerEvent("You don't have an active taxi fee in this car");
                return;
            }

            if (v == VehicleManager.Instance().GetVehicleForCharacter(meter.setter))
            {
                if (payer.money < meter.money)
                {
                    payer.SendErrorNotification("You don't have enough money to pay the taxi fee.");
                    return;
                }

                payer.SetMoney(payer.money - meter.money);
                meter.setter.SetMoney(meter.setter.money + meter.money);
                payer.SendSuccessNotification("You payed a taxi fee of $" + meter.money);
                meter.setter.SendNotification("You received taxi fee payment of $" + meter.money + " from " + payer.fullName);
                // Destroy the taxi meter totally
                RemoveTaxiMeterFromCharacterInTaxi(payer, v.id);

            }
        }

        public void SetTaxiMeterHUDForCharacter(Character character, int carId)
        {
            character.TriggerEvent("EVENT_SET_ASSIST_TEXT", 237, 195, 68, "Taxi Fee: $"+GetTaxiFeeForCharacterInTaxi(character, carId), 1); // replace 1000 with taxi fee
        }

        public void DisableTaxiMeterHUDForCharacter(Character character)
        {
            character.TriggerEvent("EVENT_REMOVE_ASSIST_TEXT", 1);
        }

        public void SetTaxiMeterForCharacter(Character setter, Character character)
        {
            // Has to be in same car
            // Can't have timer going
            RPVehicle taxiVehicle = VehicleManager.Instance().GetVehicleForCharacter(setter);
            RPVehicle v2 = VehicleManager.Instance().GetVehicleForCharacter(character);
            if (taxiVehicle == null || taxiVehicle.handle.model != (int)API.shared.vehicleNameToModel("Taxi"))
            {
                setter.SendNotification("You have to be in a taxi to set the meter");
                return;
            }

            if (v2 == null || v2.id != taxiVehicle.id)
            {
                setter.SendNotification("Your customer has to be in your the taxi");
                return;
            }

            if (IsTaxiMeterSetForCharacterOnCar(character, taxiVehicle.id))
            {
                setter.SendNotification("This customer already has an active taxi meter");
                return;
            }

            if (!taxiMeters.ContainsKey(taxiVehicle.id)) taxiMeters[taxiVehicle.id] = new Dictionary<int, TaxiMeter>();
            taxiMeters[taxiVehicle.id][character.ID] = new TaxiMeter(setter.ID, 200, new GTRPTimer(RPTimerEvent, taxiFeeInterval, new KeyValuePair<int,int>(taxiVehicle.id,character.ID), true)); // Starting money
            SetTaxiMeterHUDForCharacter(character, taxiVehicle.id);
            setter.SendNotification("Taxi meter set for customer " + character.fullName);
            taxiMeters[taxiVehicle.id][character.ID].taxiTimer.Start();
        }

        public void StopTaxiMeterForCharacter(Character setter, Character character)
        {
            RPVehicle v = VehicleManager.Instance().GetVehicleForCharacter(setter);
            TaxiMeter meter = GetTaxiMeterForCharacterInTaxi(character, v.id);
            if (meter == null) return;

            GetTaxiMeterForCharacterInTaxi(character,v.id).taxiTimer.Stop();
            setter.SendNotification("Taxi meter stopped for " + character.fullName);
        }

        /// <summary>
        /// Function that is run on taxi meter tick
        /// </summary>
        /// <param name="taxiId">ID of the taxi</param>
        /// <param name="characterId">ID of the character</param>
        public void UpdateTaxiFeeForCharacterInTaxi(int taxiId, int characterId)
        {
            // Check if character is in taxi just in case etc
            taxiMeters[taxiId][characterId].money += taxiFee;
            Character character = PlayerManager.Instance().GetCharacterWithId(characterId);
            SetTaxiMeterHUDForCharacter(character, taxiId);
        }

        public void RPTimerEvent(GTRPTimer timer)
        {
            KeyValuePair<int,int> pAndCar = (KeyValuePair<int,int>)timer.data;
            UpdateTaxiFeeForCharacterInTaxi(pAndCar.Key, pAndCar.Value);
        }

        public override void HandleOnDutyCommand(Character c)
        {
            c.onDuty = !c.onDuty;
            if (c.onDuty)
            {

            }
            else
            {

            }
        }

        private void InitializeRanks()
        {
            this.AddRank(0, "Taxi Driver", 360);
        }

        public override void Initialize()
        {
            base.Initialize();
            InitializeRanks();
            VehicleManager.Instance().SubscribeToVehicleEnterEvent(this.CharacterEnteredTaxi);
            VehicleManager.Instance().SubscribeToVehicleExitEvent(this.CharacterExitedTaxi);
        }
    }
}