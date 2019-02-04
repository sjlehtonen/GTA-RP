using System;
using System.Collections.Generic;
using GTA_RP.Vehicles;
using GrandTheftMultiplayer.Server.API;
using GrandTheftMultiplayer.Shared;
using GTA_RP.Misc;

namespace GTA_RP.Factions
{
    /// <summary>
    /// Class that represents a taxi meter.
    /// </summary>
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

    /// <summary>
    /// Faction for taxi drivers.
    /// TODO
    /// </summary>
    class TaxiDriver : RankedFaction
    {
        private Dictionary<int, Dictionary<int, TaxiMeter>> taxiMeters = new Dictionary<int, Dictionary<int, TaxiMeter>>(); // Car ID maps to character ids
        public TaxiDriver(FactionEnums id, string name, int colorR, int colorG, int colorB) : base(id, name, colorR, colorG, colorB) { }

        // TODO: Switch so that each taxi driver can adjust there individually
        private int taxiFeeInterval = (int)TimeSpan.FromSeconds(60).TotalMilliseconds;
        private const int taxiFee = 100;
        private const int taxiStartingFee = 200;

        /// <summary>
        /// Stops taxi meter for character in taxi
        /// </summary>
        /// <param name="character">Character</param>
        /// <param name="taxiId">Taxi id</param>
        private void StopTaxiMeterForCharacterInTaxi(Character character, int taxiId)
        {
            GetTaxiMeterForCharacterInTaxi(character, taxiId).taxiTimer.Stop();
        }

        /// <summary>
        /// Restarts a taxi meter for character in car
        /// </summary>
        /// <param name="character">Character</param>
        /// <param name="taxiId">Taxi id</param>
        private void RestartTaxiMeterForCharacterInTaxi(Character character, int taxiId)
        {
            GetTaxiMeterForCharacterInTaxi(character, taxiId).taxiTimer.Start();
        }

        /// <summary>
        /// Removes taxi meter from character in taxi
        /// </summary>
        /// <param name="character">Character</param>
        /// <param name="taxiId">Taxi id</param>
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
            if (taxiMeters.ContainsKey(carId))
            {
                return taxiMeters[carId].ContainsKey(character.ID);
            }
            return false;
        }

        /// <summary>
        /// When character enters taxi, this is ran
        /// </summary>
        /// <param name="character">Character</param>
        /// <param name="vehicleHandle">Vehicle handle</param>
        /// <param name="seat">Seat</param>
        private void CharacterEnteredTaxi(Character character, NetHandle vehicleHandle, int seat)
        {
            if (VehicleManager.Instance().DoesVehicleHandleHaveRPVehicle(vehicleHandle))
            {
                RPVehicle vehicle = VehicleManager.Instance().GetVehicleForHandle(vehicleHandle);
                if (IsTaxiMeterSetForCharacterOnCar(character, vehicle.id))
                {
                    SetTaxiMeterHUDForCharacter(character, vehicle.id);
                }
            }
        }

        /// <summary>
        /// When character exits taxi, this is ran
        /// </summary>
        /// <param name="character">Character</param>
        /// <param name="vehicleHandle">Vehicle handle</param>
        /// <param name="seat">Seat</param>
        private void CharacterExitedTaxi(Character character, NetHandle vehicleHandle, int seat)
        {
            if (VehicleManager.Instance().DoesVehicleHandleHaveRPVehicle(vehicleHandle))
            {
                RPVehicle vehicle = VehicleManager.Instance().GetVehicleForHandle(vehicleHandle);
                if (IsTaxiMeterSetForCharacterOnCar(character, vehicle.id))
                {
                    DisableTaxiMeterHUDForCharacter(character);
                }
            }
        }

        /// <summary>
        /// Gets taxi meter for character in a taxi
        /// </summary>
        /// <param name="character">Character</param>
        /// <param name="taxiId">Taxi id</param>
        /// <returns></returns>
        private TaxiMeter GetTaxiMeterForCharacterInTaxi(Character character, int taxiId)
        {
            if (!taxiMeters.ContainsKey(taxiId) || !taxiMeters[taxiId].ContainsKey(character.ID))
            {
                return null;
            }
            return taxiMeters[taxiId][character.ID];
        }

        /// <summary>
        /// Gets taxi fee for character in car
        /// </summary>
        /// <param name="character">Character</param>
        /// <param name="taxiId">Taxi id</param>
        /// <returns></returns>
        private int GetTaxiFeeForCharacterInTaxi(Character character, int taxiId)
        {
            return taxiMeters[taxiId][character.ID].money;
        }

        public override string GetChatColor()
        {
            return "~y~";
        }

        /// <summary>
        /// Pays a taxi meter fee in a car
        /// </summary>
        /// <param name="payer">Payer</param>
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
                payer.SendSuccessNotification(String.Format("You payed a taxi fee of ${0}", meter.money));
                meter.setter.SendNotification(String.Format("You received taxi fee payment of ${0} from {1}", meter.money, payer.fullName));
                // Destroy the taxi meter
                RemoveTaxiMeterFromCharacterInTaxi(payer, v.id);

            }
        }

        /// <summary>
        /// Sets taxi meter for character in a car
        /// </summary>
        /// <param name="character">Character</param>
        /// <param name="carId">Car id</param>
        public void SetTaxiMeterHUDForCharacter(Character character, int carId)
        {
            character.TriggerEvent("EVENT_SET_ASSIST_TEXT", 237, 195, 68, "Taxi Fee: $" + GetTaxiFeeForCharacterInTaxi(character, carId), 1); // replace 1000 with taxi fee
        }

        /// <summary>
        /// Disables the taxi meter for character
        /// </summary>
        /// <param name="character">Character</param>
        public void DisableTaxiMeterHUDForCharacter(Character character)
        {
            character.TriggerEvent("EVENT_REMOVE_ASSIST_TEXT", 1);
        }

        /// <summary>
        /// Sets taxi meter for character
        /// </summary>
        /// <param name="setter">Setter</param>
        /// <param name="character">Character to who the meter is set</param>
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

            if (setter.ID == character.ID)
            {
                setter.SendNotification("You can't set the taxi meter on yourself");
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

            if (!taxiMeters.ContainsKey(taxiVehicle.id))
            {
                taxiMeters[taxiVehicle.id] = new Dictionary<int, TaxiMeter>();
            }

            taxiMeters[taxiVehicle.id][character.ID] = new TaxiMeter(setter.ID, 200, new GTRPTimer(RPTimerEvent, taxiFeeInterval, new KeyValuePair<int, int>(taxiVehicle.id, character.ID), true)); // Starting money
            SetTaxiMeterHUDForCharacter(character, taxiVehicle.id);
            setter.SendNotification(String.Format("Taxi meter set for customer {0}", character.fullName));
            taxiMeters[taxiVehicle.id][character.ID].taxiTimer.Start();
        }

        /// <summary>
        /// Stops a taxi meter for character
        /// </summary>
        /// <param name="setter">Setter</param>
        /// <param name="character">Character</param>
        public void StopTaxiMeterForCharacter(Character setter, Character character)
        {
            RPVehicle v = VehicleManager.Instance().GetVehicleForCharacter(setter);
            TaxiMeter meter = GetTaxiMeterForCharacterInTaxi(character, v.id);
            if (meter == null)
            {
                return;
            }
            GetTaxiMeterForCharacterInTaxi(character, v.id).taxiTimer.Stop();
            setter.SendNotification(String.Format("Taxi meter stopped for {0}", character.fullName));
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
            KeyValuePair<int, int> pAndCar = (KeyValuePair<int, int>)timer.data;
            UpdateTaxiFeeForCharacterInTaxi(pAndCar.Key, pAndCar.Value);
        }

        public override void HandleOnDutyCommand(Character c)
        {
            c.onDuty = !c.onDuty;
            if (c.onDuty)
            {
                // TODO
            }
            else
            {
                // TODO
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
