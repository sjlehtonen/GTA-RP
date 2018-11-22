using System;
using System.Collections.Generic;
using GrandTheftMultiplayer.Server.API;
using GrandTheftMultiplayer.Server.Elements;
using GrandTheftMultiplayer.Server.Managers;

namespace GTA_RP.Chat
{
    /// <summary>
    /// Class reponsible for managing the chat.
    /// </summary>
    class ChatManager : Script
    {
        /// <summary>
        /// Distances for different speech types and colors for different messages.
        /// </summary>
        private const float whisperChatDistance = 5.0f;
        private const float normalChatDistance = 10.0f;
        private const float yellChatDistance = 20.0f;
        private const string meTextColor = "~#ea884f~";
        private const string loocTextColor = "~#4ea0e8~";
        private const string oocTextColor = "~#4ebfe8~";
        private const string announcementTextColor = "~#f72c3a~";
        private const string advertisementTextColor = "~#217f2a";
        private int advertisementPrice = 3000;
        private Boolean oocEnabled = true;

        public ChatManager()
        {
            API.onChatMessage += this.handleChatMessage;
        }

        /// <summary>
        /// Sends a message related to phone call
        /// </summary>
        /// <param name="c">Character</param>
        /// <param name="message">Message</param>
        /// <param name="modifier">Modifier</param>
        private void sendPhoneChatMessage(Character c, string message, string modifier = "")
        {
            if (c.phone.phoneCallActive)
            {
                Character target = c.phone.activeCallPhone.owner;
                string nameOrNumber = c.phone.activeCallPhone.phoneNumber;

                if (c.phone.HasContactForNumber(nameOrNumber))
                {
                    nameOrNumber = c.phone.GetNameForNumber(nameOrNumber);
                }
                
                API.sendChatMessageToPlayer(target.owner.client, loocTextColor, modifier + "[PHONE CALL - " + nameOrNumber + "]: " + message);
            }
        }

        /// <summary>
        /// Sends a distance chat message
        /// </summary>
        /// <param name="player">Client</param>
        /// <param name="message">Message</param>
        /// <param name="distance">Distance</param>
        /// <param name="modifier">Modifier</param>
        /// <param name="modifier2">Modifier 2</param>
        private void sendDistanceChatMessage(Client player, string message, float distance, string modifier = "", string modifier2 = "")
        {
            if (PlayerManager.Instance().IsClientUsingCharacter(player))
            {
                Character c = PlayerManager.Instance().GetActiveCharacterForClient(player);
                List<Character> characters = PlayerManager.Instance().GetCharactersInDistance(player.position, distance);
                characters.ForEach(x => API.sendChatMessageToPlayer(x.owner.client, c.faction.GetChatColor() + modifier + c.fullName + modifier2 + ": " + "~s~" + message));
                sendPhoneChatMessage(c, modifier + modifier2 + message);
            }
        }

        /// <summary>
        /// Sends distance message with color
        /// </summary>
        /// <param name="player">Player</param>
        /// <param name="message">Message</param>
        /// <param name="distance">Distance</param>
        /// <param name="color">Message color</param>
        private void sendDistanceCommandMessageWithColor(Client player, string message, float distance, string color)
        {
            if (PlayerManager.Instance().IsClientUsingCharacter(player))
            {
                Character c = PlayerManager.Instance().GetActiveCharacterForClient(player);
                List<Character> characters = PlayerManager.Instance().GetCharactersInDistance(player.position, distance);
                characters.ForEach(x => API.sendChatMessageToPlayer(x.owner.client, color, message));
            }
        }

        /// <summary>
        /// Normal chat message, small radius
        /// </summary>
        /// <param name="player"></param>
        /// <param name="message"></param>
        /// <param name="e"></param>
        private void handleChatMessage(Client player, string message, CancelEventArgs e)
        {
            e.Cancel = true;
            sendDistanceChatMessage(player, message, normalChatDistance, "");
        }

        [Command("yell", GreedyArg = true, Alias = "y")]
        public void handleYellMessage(Client player, string text)
        {
            sendDistanceChatMessage(player, text.ToUpper(), yellChatDistance, "~h~", " [YELL]");
        }

        [Command("whisper", GreedyArg = true, Alias = "w")]
        public void handleWhisperMessage(Client player, string text)
        {
            sendDistanceChatMessage(player, text, whisperChatDistance, "", " [WHISPER]");
        }

        [Command("looc", GreedyArg = true, Alias = "lo")]
        public void handleLoocCommand(Client player, string text)
        {
            Character c = PlayerManager.Instance().GetActiveCharacterForClient(player);
            if (c != null)
            {
                this.sendDistanceCommandMessageWithColor(player, "[LOOC] " + c.fullName + ": " + text, normalChatDistance, loocTextColor);
            }
        }

        [Command("advertisement", GreedyArg = true, Alias = "adv", Description = "Sends an advertisement for $3000")]
        public void handleAdvertisementCommand(Client player, string text)
        {
            if (PlayerManager.Instance().IsClientUsingCharacter(player))
            {
                Character sender = PlayerManager.Instance().GetActiveCharacterForClient(player);
                if (sender.money >= advertisementPrice)
                {
                    List<Character> activeCharacters = PlayerManager.Instance().GetActiveCharacters();
                    activeCharacters.ForEach(x => API.sendChatMessageToPlayer(x.client, advertisementTextColor, "[ADVERTISEMENT] " + text));
                    sender.SetMoney(sender.money - advertisementPrice);
                    sender.SendSuccessNotification("Advertisement sent!");
                }
                else
                {
                    sender.SendErrorNotification("You don't have enough money to send a notification.");
                }
            }
        }


        [Command("ooc", GreedyArg = true)]
        public void handleOocCommand(Client player, string text)
        {
            if (this.oocEnabled)
            {
                Character c = PlayerManager.Instance().GetActiveCharacterForClient(player);
                if (c != null)
                {
                    List<Client> clients = API.getAllPlayers();
                    clients.ForEach(x => API.sendChatMessageToPlayer(x, oocTextColor, "[OOC] " + c.fullName + ": " + text));
                }
            }
            else
            {
                API.sendChatMessageToPlayer(player, "[NOTE] OOC is not currently enabled!");
            }
        }

        [Command("toggleooc")]
        public void handleToggleOocCommand(Client player)
        {
            Character c = PlayerManager.Instance().GetActiveCharacterForClient(player);
            if (c != null && c.owner.adminLevel != 0)
            {
                this.oocEnabled = !this.oocEnabled;
                if (this.oocEnabled)
                {
                    API.sendChatMessageToAll("[NOTE] OOC was enabled by " + c.fullName);
                }
                else
                {
                    API.sendChatMessageToAll("[NOTE] OOC was disabled by " + c.fullName);
                }
            }
        }

        [Command("announce", GreedyArg = true)]
        public void handleAnnounceCommand(Client player, string text)
        {
            Character c = PlayerManager.Instance().GetActiveCharacterForClient(player);
            if (c != null && c.owner.adminLevel != 0)
            {
                List<Client> clients = API.getAllPlayers();
                clients.ForEach(x => API.sendChatMessageToPlayer(x, announcementTextColor, "[ANNOUNCEMENT] " + text));
            }
        }

        [Command("me", GreedyArg = true)]
        public void handleMeCommand(Client player, string text)
        {
            Character c = PlayerManager.Instance().GetActiveCharacterForClient(player);
            if (c != null)
            {
                this.sendDistanceCommandMessageWithColor(player, "** " + c.fullName + " " + text, normalChatDistance, meTextColor);
            }
        }

        [Command("it", GreedyArg = true)]
        public void handleItCommand(Client player, string text)
        {
            Character c = PlayerManager.Instance().GetActiveCharacterForClient(player);
            if (c != null)
            {
                this.sendDistanceCommandMessageWithColor(player, "** " + text + " **", normalChatDistance, meTextColor);
            }
        }
    }
}
