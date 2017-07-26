using GrandTheftMultiplayer.Server.API;
using GrandTheftMultiplayer.Server.Elements;
using GTA_RP.Vehicles;

namespace GTA_RP.Events
{
    /// <summary>
    /// A class responsible for handling events that are sent by the client
    /// </summary>
    class EventHandler : Script
    {
        public EventHandler()
        {
            API.onClientEventTrigger += HandleEvent;
        }

        /// <summary>
        /// Main function to handle all kinds of events from the client and forward the message to the relevant classes
        /// </summary>
        /// <param name="player">Sender of the event</param>
        /// <param name="eventName">Name of the event</param>
        /// <param name="arguments">Arguments that are sent with the event</param>
        private void HandleEvent(Client player, string eventName, params object[] arguments)
        {
            switch(eventName)
            {
                case "EVENT_REQUEST_ENTER_HOUSE":
                    int houseId = (int)arguments[0];
                    HouseManager.Instance().RequestEnterHouse(player, houseId);
                    break;

                case "EVENT_REQUEST_EXIT_HOUSE":
                    int teleId = (int)arguments[0];
                    int destinationId = (int)arguments[1];
                    HouseManager.Instance().RequestExitHouse(player, teleId, destinationId);
                    break;

                case "EVENT_REQUEST_CREATE_ACCOUNT":
                    PlayerManager.Instance().RequestCreateAccount(player, (string)arguments[0], (string)arguments[1]);
                    break;

                case "EVENT_REQUEST_SELECT_CHARACTER":
                    PlayerManager.Instance().RequestSelectCharacter(player, (string)arguments[0]);
                    break;

                case "EVENT_REQUEST_CREATE_CHARACTER_MENU":
                    PlayerManager.Instance().RequestCreateCharacterMenu(player);
                    break;

                case "EVENT_REQUEST_CREATE_CHARACTER":
                    PlayerManager.Instance().RequestCreateCharacter(player, (string)arguments[0], (string)arguments[1], (string)arguments[2]);
                    break;

                case "EVENT_REQUEST_OWNED_HOUSES":
                    HouseManager.Instance().SendListOfOwnedHousesToClient(player);
                    break;

                case "EVENT_SET_PLAYER_USING_PHONE":
                    PlayerManager.Instance().SetPlayerUsingPhone(player);
                    break;

                case "EVENT_SET_PLAYER_CALLING":
                    PlayerManager.Instance().SetPlayerPhoneCalling(player);
                    break;

                case "EVENT_SET_PLAYER_NOT_USING_PHONE":
                    PlayerManager.Instance().SetPlayerPhoneOut(player);
                    break;

                case "EVENT_SEND_TEXT_MESSAGE":
                    PlayerManager.Instance().TrySendTextMessage(player, (string)arguments[0], (string)arguments[1]);
                    break;

                case "EVENT_ADD_PHONE_CONTACT":
                    PlayerManager.Instance().TryAddNewContact(player, (string)arguments[0], (string)arguments[1]);
                    break;

                case "EVENT_REMOVE_PHONE_CONTACT":
                    PlayerManager.Instance().TryDeleteContact(player, (string)arguments[0]);
                    break;

                case "EVENT_REMOVE_TEXT_MESSAGE":
                    PlayerManager.Instance().TryDeleteTextMessage(player, (int)arguments[0]);
                    break;

                case "EVENT_START_PHONE_CALL":
                    PlayerManager.Instance().TryStartPhoneCall(player, (string)arguments[0]);
                    break;

                case "EVENT_ACCEPT_PHONE_CALL":
                    PlayerManager.Instance().TryAcceptPhoneCall(player);
                    break;

                case "EVENT_END_PHONE_CALL":
                    PlayerManager.Instance().TryHangupPhoneCall(player);
                    break;

                case "EVENT_EXIT_VEHICLE_SHOP":
                    VehicleManager.Instance().TryExitVehicleShop(player);
                    break;

                case "EVENT_BUY_VEHICLE":
                    VehicleManager.Instance().TryPurchaseVehicle(player, (string)arguments[0], (int)arguments[1], (int)arguments[2]);
                    break;
            }
        }
    }
}
