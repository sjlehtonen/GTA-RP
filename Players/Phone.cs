using GrandTheftMultiplayer.Server.API;
using GrandTheftMultiplayer.Server.Elements;
using GrandTheftMultiplayer.Shared;
using GrandTheftMultiplayer.Shared.Math;
using GTA_RP.Factions;
using GTA_RP.Vehicles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Timers;

/// <summary>
/// TODO: Refactor this whole class and move everything phone related
/// to PhoneManager class or something like that.
/// </summary>
namespace GTA_RP
{
    /// <summary>
    /// Tells how player is using phone
    /// </summary>
    enum PhoneState
    {
        PHONE_NOT_USING,
        PHONE_READING,
        PHONE_CALLING,
        PHONE_SELFIE
    }

    /// <summary>
    /// Address book contact
    /// </summary>
    public struct Address
    {
        public String number;
        public String name;
    }

    /// <summary>
    /// Text message
    /// </summary>
    public struct TextMessage
    {
        public int id;
        public String senderNumber;
        public String time;
        public String message;
    }

    /// <summary>
    /// Class for player phone
    /// Every player has a phone and it's represented by this class
    /// </summary>
    public class Phone
    {
        public Character owner { get; private set; }
        private int gender = 0;
        private NetHandle entity;
        private PhoneState phoneState = PhoneState.PHONE_NOT_USING;
        private List<Address> addressBook = new List<Address>();
        private List<TextMessage> receivedMessages = new List<TextMessage>();



        private Vector3 femaleUsingPhonePosition = new Vector3(0.14, 0, -0.021);
        private Vector3 femaleUsingPhoneRotation = new Vector3(130, 115, 0);
        private Vector3 femaleCallingPhonePosition = new Vector3(0.1, 0, -0.021);
        private Vector3 femaleCallingPhoneRotation = new Vector3(90, 100, 0);

        private Vector3 maleUsingPhoneRotation = new Vector3(130, 100, 0);
        private Vector3 maleUsingPhonePosition = new Vector3(0.17, 0.021, -0.009);
        private Vector3 maleCallingPhoneRotation = new Vector3(120, 100, 0);
        private Vector3 maleCallingPhonePosition = new Vector3(0.16, 0.02, -0.01);


        private Timer secondAnimationTimer = new Timer();

        public String phoneNumber { get; private set; }

        public Boolean isCalling { get; private set; }
        public String callingNumber { get; private set; }

        public String callerNumber { get; private set; }
        public Boolean phoneCallActive { get; private set; }
        public Phone activeCallPhone { get; private set; }

        public Timer simulateCallTimer = new Timer(4000);



        public Phone(Character character, String phoneNumber)
        {
            owner = character;
            this.phoneNumber = phoneNumber;

            this.isCalling = false;
            this.callerNumber = null;
            this.phoneCallActive = false;
            this.activeCallPhone = null;
            this.gender = PlayerManager.Instance().GetGenderForModel(owner.model);

            PlayerManager.Instance().SubscribeToPlayerDisconnectEvent(this.PlayerDisconnectedEvent);
            VehicleManager.Instance().SubscribeToVehicleEnterEvent(this.OwnerEnteredVehicle);
            VehicleManager.Instance().SubscribeToVehicleExitEvent(this.OwnerExitedVehicle);
        }

        /// <summary>
        /// Ran when owner of the phone entered vehicle
        /// </summary>
        /// <param name="client">Client</param>
        /// <param name="vehicle">Vehicle</param>
        private void OwnerEnteredVehicle(Client client, NetHandle vehicle, int seat)
        {
            if (client == this.owner.owner.client)
            {
                if (phoneState == PhoneState.PHONE_CALLING)
                {
                    SetPhoneCallingAnimation();
                }
                else if (phoneState == PhoneState.PHONE_READING)
                {
                    SetPhoneUsingAnimation();
                }
            }
        }

        /// <summary>
        /// Ran when owner of the phone exits vehicle
        /// </summary>
        /// <param name="client"></param>
        /// <param name="vehicle"></param>
        private void OwnerExitedVehicle(Client client, NetHandle vehicle, int seat)
        {
            if (client == this.owner.owner.client)
            {
                if (phoneState == PhoneState.PHONE_CALLING)
                {
                    SetPhoneCallingAnimation();
                }
                else if (phoneState == PhoneState.PHONE_READING)
                {
                    SetPhoneUsingAnimation();
                }
            }
        }

        /// <summary>
        /// Is ran when the fake call timer elapses
        /// Closes the fake call
        /// </summary>
        /// <param name="source">Timer</param>
        /// <param name="args">Timer arguments</param>
        private void TimerElapsed(System.Object source, ElapsedEventArgs args)
        {
            secondAnimationTimer.Enabled = false;
            DeletePhone();
        }

        /// <summary>
        /// Creates the phone object
        /// </summary>
        private void CreatePhone()
        {
            entity = API.shared.createObject(-1038739674, owner.position, new Vector3(0, 0, 0));
            if (!IsMale())
            {
                owner.AttachObject(entity, "57005", femaleUsingPhonePosition, femaleUsingPhoneRotation);
            }
            else
            {
                owner.AttachObject(entity, "57005", maleUsingPhonePosition, maleUsingPhoneRotation);
            }
        }

        /// <summary>
        /// Create phone in calling position
        /// </summary>
        private void CreatePhoneCalling()
        {
            entity = API.shared.createObject(-1038739674, owner.position, new Vector3(0, 0, 0));
            if (!IsMale())
            {
                owner.AttachObject(entity, "57005", femaleCallingPhonePosition, femaleCallingPhoneRotation);
            }
            else
            {
                owner.AttachObject(entity, "57005", maleCallingPhonePosition, maleCallingPhoneRotation);
            }
        }

        /// <summary>
        /// Recreates the phone in wanted state
        /// </summary>
        /// <param name="state">State to create phone in</param>
        private void RecreatePhone(PhoneState state)
        {
            if (state == PhoneState.PHONE_CALLING)
            {
                DeletePhone();
                CreatePhoneCalling();
            }
            else
            {
                DeletePhone();
                CreatePhone();
            }
        }

        /// <summary>
        /// Checks whether the owner of the phone is male
        /// </summary>
        /// <returns>True if owner is male, otherwise false</returns>
        private Boolean IsMale()
        {
            if (gender == 0)
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// Deletes the phone object
        /// </summary>
        private void DeletePhone()
        {
            owner.DetachObject(entity);
            API.shared.deleteEntity(entity);
        }

        /// <summary>
        /// Adds received text message to databse
        /// </summary>
        /// <param name="message">Received text message</param>
        private void AddReceivedTextMessageToDatabase(TextMessage message)
        {
            DBManager.InsertQuery("INSERT INTO text_messages VALUES (@id, @sender_number, @receiver_number, @time, @message)")
                .AddValue("@id", message.id)
                .AddValue("@sender_number", message.senderNumber)
                .AddValue("@receiver_number", this.phoneNumber)
                .AddValue("@time", message.time)
                .AddValue("@message", message.message)
                .Execute();
        }

        /// <summary>
        /// Removes a text message from database
        /// </summary>
        /// <param name="id">Text message id</param>
        private void RemoveTextMessageFromDatabase(int id)
        {
            DBManager.DeleteQuery("DELETE FROM text_messages WHERE id=@id")
                .AddValue("@id", id)
                .Execute();
        }

        /// <summary>
        /// Adds a contact to database
        /// </summary>
        /// <param name="address">Contact</param>
        private void AddContactToDatabase(Address address)
        {
            DBManager.InsertQuery("INSERT INTO phone_contacts VALUES (@owner, @name, @number)")
                .AddValue("@owner", this.owner.ID)
                .AddValue("@name", address.name)
                .AddValue("@number", address.number)
                .Execute();
        }

        /// <summary>
        /// Removes a contact from the database
        /// </summary>
        /// <param name="number">Contact's number</param>
        private void RemoveContactFromDatabase(String number)
        {
            DBManager.DeleteQuery("DELETE FROM phone_contacts WHERE owner=@owner AND number=@number")
                .AddValue("@owner", this.owner.ID)
                .AddValue("@number", number)
                .Execute();
        }

        /// <summary>
        /// Is ran when the fake call timer expires
        /// </summary>
        /// <param name="source">Timer</param>
        /// <param name="args">Timer arguments</param>
        private void SimulatePhoneCallTimerEvent(System.Object source, ElapsedEventArgs args)
        {
            simulateCallTimer.Enabled = false;
            this.isCalling = false;
            API.shared.triggerClientEvent(owner.owner.client, "EVENT_PHONE_CALL_ENDED");
            SetPhoneNotUsing();
        }

        /// <summary>
        /// Checks whether player vehicle is allowed type for phone animation
        /// </summary>
        /// <returns>True if ok for phone animation, otherwise false</returns>
        private Boolean ValidatePlayerVehicle()
        {
            if (owner.isInVehicle && owner.vehicleClass != 13 && owner.vehicleClass != 9)
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// Sets calling animation accordingly
        /// Currently vehicle animations only seem to be correct for male models
        /// </summary>
        private void SetPhoneCallingAnimation()
        {
            if (ValidatePlayerVehicle() && IsMale())
            {
                if (owner.isDriver) owner.PlayAnimation((int)(AnimationFlags.AllowPlayerControl | AnimationFlags.Loop | AnimationFlags.OnlyAnimateUpperBody), "cellphone@in_car@ds", "cellphone_call_listen_base");
                else owner.PlayAnimation((int)(AnimationFlags.AllowPlayerControl | AnimationFlags.Loop | AnimationFlags.OnlyAnimateUpperBody), "cellphone@in_car@ps", "cellphone_call_listen_base");
            }
            else
            {
                if (!IsMale()) owner.PlayAnimation((int)(AnimationFlags.AllowPlayerControl | AnimationFlags.Loop | AnimationFlags.OnlyAnimateUpperBody), "cellphone@female", "cellphone_call_listen_base");
                else owner.PlayAnimation((int)(AnimationFlags.AllowPlayerControl | AnimationFlags.Loop | AnimationFlags.OnlyAnimateUpperBody), "cellphone@", "cellphone_call_listen_base");
            }
        }

        /// <summary>
        /// Sets phone using animation accordingly
        /// </summary>
        private void SetPhoneUsingAnimation()
        {
            if (ValidatePlayerVehicle() && IsMale())
            {
                if (owner.isDriver) owner.PlayAnimation((int)(AnimationFlags.AllowPlayerControl | AnimationFlags.Loop | AnimationFlags.OnlyAnimateUpperBody), "cellphone@in_car@ds", "cellphone_text_read_base");
                else owner.PlayAnimation((int)(AnimationFlags.AllowPlayerControl | AnimationFlags.Loop | AnimationFlags.OnlyAnimateUpperBody), "cellphone@in_car@ps", "cellphone_text_read_base");
            }
            else
            {
                if (!IsMale()) owner.PlayAnimation((int)(AnimationFlags.AllowPlayerControl | AnimationFlags.Loop | AnimationFlags.OnlyAnimateUpperBody), "cellphone@female", "cellphone_text_read_base");
                else owner.PlayAnimation((int)(AnimationFlags.AllowPlayerControl | AnimationFlags.Loop | AnimationFlags.OnlyAnimateUpperBody), "cellphone@", "cellphone_text_read_base");
            }
        }

        /// <summary>
        /// Sets phone away animation correctly
        /// </summary>
        private void SetPhoneNotUsingAnimation()
        {
            if (phoneState == PhoneState.PHONE_CALLING)
            {
                if (ValidatePlayerVehicle() && IsMale())
                {
                    if (owner.isDriver) owner.PlayAnimation((int)(AnimationFlags.AllowPlayerControl | AnimationFlags.OnlyAnimateUpperBody), "cellphone@in_car@ds", "cellphone_call_out");
                    else owner.PlayAnimation((int)(AnimationFlags.AllowPlayerControl | AnimationFlags.OnlyAnimateUpperBody), "cellphone@in_car@ps", "cellphone_call_out");
                }
                else
                {
                    if (!IsMale()) owner.PlayAnimation((int)(AnimationFlags.AllowPlayerControl | AnimationFlags.OnlyAnimateUpperBody), "cellphone@female", "cellphone_call_out");
                    else owner.PlayAnimation((int)(AnimationFlags.AllowPlayerControl | AnimationFlags.OnlyAnimateUpperBody), "cellphone@", "cellphone_call_out");
                }
            }
            else if (phoneState == PhoneState.PHONE_READING)
            {
                if (ValidatePlayerVehicle() && IsMale())
                {
                    if (owner.isDriver) owner.PlayAnimation((int)(AnimationFlags.AllowPlayerControl | AnimationFlags.OnlyAnimateUpperBody), "cellphone@in_car@ds", "cellphone_text_out");
                    else owner.PlayAnimation((int)(AnimationFlags.AllowPlayerControl | AnimationFlags.OnlyAnimateUpperBody), "cellphone@in_car@ps", "cellphone_text_out");
                }
                else
                {
                    if (!IsMale()) owner.PlayAnimation((int)(AnimationFlags.AllowPlayerControl | AnimationFlags.OnlyAnimateUpperBody), "cellphone@female", "cellphone_text_out");
                    else owner.PlayAnimation((int)(AnimationFlags.AllowPlayerControl | AnimationFlags.OnlyAnimateUpperBody), "cellphone@", "cellphone_text_out");

                }
            }
        }

        /// <summary>
        /// Stops the phone call when player disconnects
        /// </summary>
        /// <param name="character">Client</param>
        private void PlayerDisconnectedEvent(Character character)
        {
            if (character == this.owner) // ?
            {
                if (this.phoneCallActive || this.isCalling)
                {
                    this.HangUpCall();
                }
                else if (this.phoneState == PhoneState.PHONE_READING)
                {
                    this.DeletePhone();
                }
            }
            VehicleManager.Instance().UnsubscribeFromVehicleEnterEvent(this.OwnerEnteredVehicle);
            VehicleManager.Instance().UnsubscribeFromVehicleExitEvent(this.OwnerExitedVehicle);
            PlayerManager.Instance().UnsubscribeFromPlayerDisconnectEvent(this.PlayerDisconnectedEvent);
        }

        /// <summary>
        /// Simulates a fake phone call
        /// </summary>
        /// <param name="number">Number to call to</param>
        private void SimulatePhoneCalling(String number)
        {
            SetPhoneCalling();
            this.isCalling = true;

            simulateCallTimer.AutoReset = false;
            simulateCallTimer.Elapsed += SimulatePhoneCallTimerEvent;
            simulateCallTimer.Enabled = true;
            this.owner.TriggerEvent("EVENT_PHONE_CALL_STARTED", number);
        }

        /// <summary>
        /// Add a list of text messages
        /// </summary>
        /// <param name="messages">Messages to add</param>
        public void AddReceivedMessages(List<TextMessage> messages)
        {
            receivedMessages.AddRange(messages);
        }

        /// <summary>
        /// Add a list of contacts
        /// </summary>
        /// <param name="contacts">Contacts to add</param>
        public void AddContacts(List<Address> contacts)
        {
            addressBook.AddRange(contacts);
        }

        public bool IsUsingPhone()
        {
            if (phoneState != PhoneState.PHONE_NOT_USING) { return true; }
            return false;
        }

        /// <summary>
        /// Checks if contact with number exists
        /// </summary>
        /// <param name="number">Contact number</param>
        /// <returns></returns>
        public Boolean HasContactForNumber(String number)
        {
            return addressBook.Any(x => x.number.Equals(number));
        }

        /// <summary>
        /// Checks if phone has text message with id
        /// </summary>
        /// <param name="id">Text message id</param>
        /// <returns>True if phone has text message with id, otherwise false</returns>
        public Boolean HasTextMessageWithId(int id)
        {
            return receivedMessages.Any(x => x.id == id);
        }

        /// <summary>
        /// Gets all text messages
        /// </summary>
        /// <returns>All text messages</returns>
        public List<TextMessage> GetTextmessages()
        {
            return this.receivedMessages;
        }

        /// <summary>
        /// Gets all text message ids
        /// </summary>
        /// <returns>All text message ids</returns>
        public List<int> GetTextMessageIds()
        {
            return this.receivedMessages.Select(x => x.id).ToList();
        }

        /// <summary>
        /// Gets all text message senders
        /// </summary>
        /// <returns>All text message senders</returns>
        public List<string> GetTextMessageSenders()
        {
            List<string> senders = new List<string>();
            foreach (TextMessage m in receivedMessages)
            {
                if (HasContactForNumber(m.senderNumber))
                {
                    senders.Add(GetNameForNumber(m.senderNumber));
                }
                else
                {
                    senders.Add(m.senderNumber);
                }
            }

            return senders;
        }

        /// <summary>
        /// Gets all text message texts
        /// </summary>
        /// <returns>All text message texts</returns>
        public List<string> GetTextMessageTexts()
        {
            return this.receivedMessages.Select(x => x.message).ToList();
        }

        /// <summary>
        /// Gets all text message times
        /// </summary>
        /// <returns>All text message times</returns>
        public List<string> GetTextMessageTimes()
        {
            return this.receivedMessages.Select(x => x.time).ToList();
        }

        /// <summary>
        /// Gets all contact names
        /// </summary>
        /// <returns>All text message names</returns>
        public List<string> GetContactNames()
        {
            return this.addressBook.Select(x => x.name).ToList();
        }

        /// <summary>
        /// Gets all contact numbers
        /// </summary>
        /// <returns>All text message numbers</returns>
        public List<string> GetContactNumbers()
        {
            return this.addressBook.Select(x => x.number).ToList();
        }

        /// <summary>
        /// Gets a contact name for number
        /// </summary>
        /// <param name="number">Number to get name for</param>
        /// <returns>Contact name</returns>
        public String GetNameForNumber(String number)
        {
            return addressBook.Single(x => x.number.Equals(number)).name;
        }

        /// <summary>
        /// Sets phone to using state and animation
        /// Standing with phone on hand
        /// </summary>
        public void SetPhoneUsing()
        {
            if (phoneState == PhoneState.PHONE_READING) return;

            if (phoneState != PhoneState.PHONE_READING)
            {
                secondAnimationTimer.Stop();
                RecreatePhone(PhoneState.PHONE_READING);
            }


            SetPhoneUsingAnimation();
            phoneState = PhoneState.PHONE_READING;
        }

        /// <summary>
        /// Sets phone to calling state and animation
        /// Calling phone animation
        /// </summary>
        public void SetPhoneCalling()
        {
            if (phoneState == PhoneState.PHONE_CALLING)
            {
                return;
            }

            if (phoneState != PhoneState.PHONE_CALLING)
            {
                RecreatePhone(PhoneState.PHONE_CALLING);
            }

            SetPhoneCallingAnimation();
            phoneState = PhoneState.PHONE_CALLING;
        }

        /// <summary>
        /// Sets phone into pocket and not using state
        /// </summary>
        public void SetPhoneNotUsing()
        {
            if (phoneState == PhoneState.PHONE_NOT_USING)
            {
                return;
            }

            SetPhoneNotUsingAnimation();

            phoneState = PhoneState.PHONE_NOT_USING;
            secondAnimationTimer.Elapsed += TimerElapsed;
            secondAnimationTimer.Interval = 800;
            secondAnimationTimer.Enabled = true;
        }

        /// <summary>
        /// Adds new contact to address book
        /// </summary>
        /// <param name="name">Contact name</param>
        /// <param name="number">Contact number</param>
        public void AddNameToAddressBook(String name, String number)
        {
            Address a;
            a.name = name;
            a.number = number;
            addressBook.Add(a);
            this.AddContactToDatabase(a);
            this.owner.SendNotification("Contact added!");
        }

        /// <summary>
        /// Removes a contact from address book
        /// </summary>
        /// <param name="number">Contact number</param>
        public void RemoveContactFromAddressBook(String number)
        {
            int indexToRemove = -1;
            for (int i = 0; i < addressBook.Count; i++)
            {
                Address a = addressBook.ElementAt(i);
                if (a.number.Equals(number))
                {
                    indexToRemove = i;
                    break;
                }
            }

            if (indexToRemove != -1)
            {
                addressBook.RemoveAt(indexToRemove);
                RemoveContactFromDatabase(number);
            }

        }

        /// <summary>
        /// Removes a text message
        /// </summary>
        /// <param name="id">Text message id</param>
        public void RemoveTextMessage(int id)
        {
            int indexToRemove = -1;
            for (int i = 0; i < receivedMessages.Count; i++)
            {
                TextMessage t = receivedMessages.ElementAt(i);
                if (t.id == id)
                {
                    indexToRemove = i;
                    break;
                }
            }

            if (indexToRemove != -1)
            {
                receivedMessages.RemoveAt(indexToRemove);
                RemoveTextMessageFromDatabase(id);
            }
        }

        /// <summary>
        /// Sends a text message
        /// </summary>
        /// <param name="receiverNumber">Receiver number</param>
        /// <param name="text">Message</param>
        public void SendMessage(String receiverNumber, String text)
        {
            TextMessage message;
            message.senderNumber = this.phoneNumber;
            message.message = text;
            message.time = API.shared.getTime().ToString();
            message.id = -1;
            PlayerManager.Instance().DeliverTextMessageToNumber(receiverNumber, message);
        }

        /// <summary>
        /// Receives a text message
        /// </summary>
        /// <param name="id">Message id</param>
        /// <param name="senderNumber">Sender number</param>
        /// <param name="text">Message</param>
        /// <param name="time">Time sent</param>
        public void ReceiveMessage(int id, String senderNumber, String text, String time)
        {
            TextMessage message;
            message.senderNumber = senderNumber;
            message.time = time;
            message.message = text;
            message.id = id;

            this.AddReceivedTextMessageToDatabase(message);
            this.receivedMessages.Add(message);

            if (HasContactForNumber(senderNumber))
            {
                String senderName = GetNameForNumber(senderNumber);
                this.owner.SendPictureNotification(text, "CHAR_DEFAULT", senderName, senderNumber);
                this.owner.TriggerEvent("EVENT_NEW_TEXT_MESSAGE_RECEIVED", id, senderName, text, time);
            }
            else
            {
                this.owner.SendPictureNotification(text, "CHAR_DEFAULT", senderNumber, "");
                this.owner.TriggerEvent("EVENT_NEW_TEXT_MESSAGE_RECEIVED", id, senderNumber, text, time);
            }

            this.owner.SendNotification("You got a new text message!");

        }

        /// <summary>
        /// Sets call active
        /// </summary>
        /// <param name="number">Number who call is active with</param>
        public void SetCallActive(String number)
        {
            callingNumber = null;
            isCalling = false;

            phoneCallActive = true;
            callerNumber = number;
            activeCallPhone = PlayerManager.Instance().GetCharacterWithPhoneNumber(number).phone;
        }

        /// <summary>
        /// Sets call inactive
        /// </summary>
        public void SetCallInactive()
        {
            phoneCallActive = false;
            callerNumber = null;
            activeCallPhone = null;
        }

        /// <summary>
        /// Receives a call
        /// </summary>
        /// <param name="number">Number to receive call from</param>
        public void ReceiveCall(String number)
        {
            String caller = number;

            if (HasContactForNumber(number))
            {
                caller = GetNameForNumber(number);
            }

            this.callerNumber = number;
            this.owner.TriggerEvent("EVENT_RECEIVE_PHONE_CALL", caller);
        }

        /// <summary>
        /// Calls a phone with number
        /// </summary>
        /// <param name="number">Phone number</param>
        public void CallPhone(String number)
        {
            Character characterToCall = PlayerManager.Instance().GetCharacterWithPhoneNumber(number);
            if (characterToCall == null)
            {
                // busy tone
                this.SimulatePhoneCalling(number);
            }
            else
            {
                if (characterToCall.phone.isCalling || characterToCall.phone.phoneCallActive)
                {
                    // busy tone
                    this.SimulatePhoneCalling(number);
                }
                else
                {
                    characterToCall.phone.ReceiveCall(this.phoneNumber);
                    this.SetPhoneCalling();
                    String caller = number;

                    if (HasContactForNumber(number))
                    {
                        caller = GetNameForNumber(number);
                    }

                    this.owner.TriggerEvent("EVENT_PHONE_CALL_STARTED", caller);
                    this.isCalling = true;
                    this.callingNumber = number;
                }
            }
        }

        /// <summary>
        /// Picks up a phone call
        /// </summary>
        public void PickupCall()
        {
            if (this.callerNumber != null)
            {
                Character characterCaller = PlayerManager.Instance().GetCharacterWithPhoneNumber(this.callerNumber);
                if (characterCaller != null)
                {
                    if (characterCaller.phone.isCalling)
                    {
                        characterCaller.phone.SetCallActive(this.phoneNumber);
                        this.SetCallActive(this.callerNumber);
                        this.owner.TriggerEvent("EVENT_PHONE_CALL_PICKED_UP", this.callerNumber);
                    }
                }
            }
        }

        /// <summary>
        /// Closes a phone call
        /// </summary>
        public void CloseCall()
        {
            SetCallInactive();
            this.owner.TriggerEvent("EVENT_PHONE_CALL_ENDED");
            this.owner.SendNotification("Call ended");
            SetPhoneNotUsing();
        }

        /// <summary>
        /// Hangs up a phone call
        /// </summary>
        public void HangUpCall()
        {
            if (this.isCalling)
            {
                Character callingChar = PlayerManager.Instance().GetCharacterWithPhoneNumber(this.callingNumber);

                if (callingChar != null)
                {
                    this.owner.TriggerEvent("EVENT_PHONE_CALL_ENDED");
                }

                this.isCalling = false;
                this.callingNumber = null;
                this.simulateCallTimer.Stop();
                SetPhoneNotUsing();
            }
            else if (this.phoneCallActive)
            {
                this.activeCallPhone.CloseCall();
                SetCallInactive();
                SetPhoneNotUsing();
            }

            this.owner.TriggerEvent("EVENT_PHONE_CALL_ENDED");
        }
    }
}
