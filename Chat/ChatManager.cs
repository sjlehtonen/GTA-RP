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
        /// <param name="character">Character</param>
        /// <param name="message">Message</param>
        /// <param name="modifier">Modifier</param>
        private void sendPhoneChatMessage(Character character, string message, string modifier = "")
        {
            if (character.phone.phoneCallActive)
            {
                Character target = character.phone.activeCallPhone.owner;
                string nameOrNumber = character.phone.activeCallPhone.phoneNumber;

                if (character.phone.HasContactForNumber(nameOrNumber))
                {
                    nameOrNumber = character.phone.GetNameForNumber(nameOrNumber);
                }
                
                API.sendChatMessageToPlayer(target.owner.client, loocTextColor, String.Format("{0}[PHONE CALL - {1}]: {2}", modifier, nameOrNumber, message));
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
            PlayerManager.runMethodIfUsingCharacter(player, (Character character) =>
            {
                List<Character> characters = PlayerManager.Instance().GetCharactersInDistance(character.owner.client.position, distance);
                characters.ForEach(x => API.sendChatMessageToPlayer(x.owner.client, String.Format("{0}{1}{2}{3}: ~s~{4}", character.faction.GetChatColor(), modifier, character.fullName, modifier2, message)));
                sendPhoneChatMessage(character, String.Format("{0}{1}{2}", modifier, modifier2, message));
            });
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
            PlayerManager.runMethodIfUsingCharacter(player, (Character character) =>
            {
                List<Character> characters = PlayerManager.Instance().GetCharactersInDistance(character.owner.client.position, distance);
                characters.ForEach(x => API.sendChatMessageToPlayer(x.owner.client, color, message));
            });
        }

        /// <summary>
        /// Normal chat message, small radius
        /// </summary>
        /// <param name="player"></param>
        /// <param name="message"></param>
        /// <param name="eventArgs"></param>
        private void handleChatMessage(Client player, string message, CancelEventArgs eventArgs)
        {
            eventArgs.Cancel = true;
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
            PlayerManager.runMethodIfUsingCharacter(player, (Character character) =>
            {
                this.sendDistanceCommandMessageWithColor(player, String.Format("[LOOC] {0}: {1}", character.fullName, text), normalChatDistance, loocTextColor);
            });
        }

        [Command("advertisement", GreedyArg = true, Alias = "adv", Description = "Sends an advertisement for $3000")]
        public void handleAdvertisementCommand(Client player, string text)
        {
            PlayerManager.runMethodIfUsingCharacter(player, (Character character) =>
            {
                if (character.money >= advertisementPrice)
                {
                    List<Character> activeCharacters = PlayerManager.Instance().GetActiveCharacters();
                    activeCharacters.ForEach(x => API.sendChatMessageToPlayer(x.client, advertisementTextColor, String.Format("[ADVERTISEMENT] {0}", text)));
                    character.SetMoney(character.money - advertisementPrice);
                    character.SendSuccessNotification("Advertisement sent!");
                }
                else
                {
                    character.SendErrorNotification("You don't have enough money to send a notification.");
                }
            });
        }


        [Command("ooc", GreedyArg = true)]
        public void handleOocCommand(Client player, string text)
        {
            if (this.oocEnabled)
            {
                PlayerManager.runMethodIfUsingCharacter(player, (Character character) =>
                {
                    List<Client> clients = API.getAllPlayers();
                    clients.ForEach(x => API.sendChatMessageToPlayer(x, oocTextColor, String.Format("[OOC] {0}: {1}", character.fullName, text)));
                });
            }
            else
            {
                API.sendChatMessageToPlayer(player, "[NOTE] OOC is not currently enabled!");
            }
        }

        [Command("toggleooc")]
        public void handleToggleOocCommand(Client player)
        {
            PlayerManager.runMethodIfUsingCharacter(player, (Character character) =>
            {
                if (character.owner.adminLevel != 0)
                {
                    this.oocEnabled = !this.oocEnabled;
                    if (this.oocEnabled)
                    {
                        API.sendChatMessageToAll(String.Format("[NOTE] OOC was enabled by {0}", character.fullName));
                    }
                    else
                    {
                        API.sendChatMessageToAll(String.Format("[NOTE] OOC was disabled by {0}", character.fullName));
                    }
                }
            });
        }

        [Command("announce", GreedyArg = true)]
        public void handleAnnounceCommand(Client player, string text)
        {
            PlayerManager.runMethodIfUsingCharacter(player, (Character character) =>
            {
                if (character.owner.adminLevel != 0)
                {
                    List<Client> clients = API.getAllPlayers();
                    clients.ForEach(x => API.sendChatMessageToPlayer(x, announcementTextColor, String.Format("[ANNOUNCEMENT] {0}", text)));
                }
            });
        }

        [Command("me", GreedyArg = true)]
        public void handleMeCommand(Client player, string text)
        {
            PlayerManager.runMethodIfUsingCharacter(player, (Character character) =>
            {
                this.sendDistanceCommandMessageWithColor(player, String.Format("** {0} {1}", character.fullName, text), normalChatDistance, meTextColor);
            });
        }

        [Command("it", GreedyArg = true)]
        public void handleItCommand(Client player, string text)
        {
            PlayerManager.runMethodIfUsingCharacter(player, (Character character) =>
            {
                this.sendDistanceCommandMessageWithColor(player, String.Format("** {0} **", text), normalChatDistance, meTextColor);
            });
        }
    }
}
