using System;
using GrandTheftMultiplayer.Shared.Math;
using System.Collections.Generic;
using GTA_RP.Map;
using GTA_RP.Misc;

namespace GTA_RP.Factions
{
    /// <summary>
    /// Structure for a ticket given by a cop.
    /// </summary>
    struct Ticket
    {
        public int id;
        public string giverName;
        public string reason;
        public int fine;
        public int characterId;

        public Ticket(int id, string giver, string reason, int fine, int characterId)
        {
            this.id = id;
            this.giverName = giver;
            this.reason = reason;
            this.fine = fine;
            this.characterId = characterId;
        }
    }

    /// <summary>
    /// Class that represents arrest.
    /// Contains time and reason.
    /// Is mapped to character ID in the law enforcement faction.
    /// </summary>
    class Arrest
    {
        public int minutes;
        public string reason;

        public Arrest(int minutes, string reason)
        {
            this.minutes = minutes;
            this.reason = reason;
        }
    }

    class LawEnforcement : RankedFaction
    {
        private Dictionary<int, Arrest> arrestedCharacters = new Dictionary<int, Arrest>();
        private Dictionary<int, List<Ticket>> characterTickets = new Dictionary<int, List<Ticket>>();

        private Checkpoint jailCheckpoint;
        private int currentTicketId = 0;

        ///  Config
        private const float ticketGiveDistance = 3.0f;
        private Vector3 prisonEntranceLocation = new Vector3(1690.596, 2596.603, 44.56489);
        private Vector3 prisonSpawnLocation = new Vector3(1644.265, 2531.904, 44.56488);
        private Vector3 prisonReleaseLocation = new Vector3(1853.729, 2606.51, 45.67206);
        private float prisonEntranceCircleSize = 5.0f;
        ///

        public LawEnforcement(FactionI id, string name, int colorR, int colorG, int colorB) : base(id, name, colorR, colorG, colorB)
        {
        }

        /// <summary>
        /// Initialize ranks for the faction
        /// </summary>
        private void InitializeRanks()
        {
            AddRank(0, "Police Officer I", 500);
            AddRank(1, "Police Officer II", 1000);
            AddRank(2, "Traffic Police", 1200);
            AddRank(3, "Detective", 2000);
            AddRank(4, "Chief of Police", 4000);
        }

        /// <summary>
        /// Adds a ticket
        /// </summary>
        /// <param name="ticket"></param>
        /// <param name="saveToDB"></param>
        private void AddTicket(Ticket ticket, bool saveToDB = false)
        {
            if (!characterTickets.ContainsKey(ticket.characterId))
            {
                this.characterTickets[ticket.characterId] = new List<Ticket>();
            }
            this.characterTickets[ticket.characterId].Add(ticket);

            if (saveToDB)
            {
                DBManager.InsertQuery("INSERT INTO character_tickets VALUES (@id, @issuer, @character_id, @fine, @reason)")
                    .AddValue("@id", ticket.id)
                    .AddValue("@issuer", ticket.giverName)
                    .AddValue("@character_id", ticket.characterId)
                    .AddValue("@fine", ticket.fine)
                    .AddValue("@reason", ticket.reason)
                    .Execute();
            }
        }

        /// <summary>
        /// Loads jailed characters from DB
        /// </summary>
        private void LoadJailedCharactersFromDB()
        {
            DBManager.SelectQuery("SELECT * FROM jailed_characters", (MySql.Data.MySqlClient.MySqlDataReader reader) =>
            {
                Arrest arrest = new Arrest(reader.GetInt32(1), reader.GetString(2));
                arrestedCharacters.Add(reader.GetInt32(0), arrest);
            }).Execute();
        }

        /// <summary>
        /// Loads tickets from the DB
        /// </summary>
        private void LoadTicketsFromDB()
        {
            DBManager.SelectQuery("SELECT * FROM character_tickets", (MySql.Data.MySqlClient.MySqlDataReader reader) =>
            {
                Ticket newTicket = new Ticket(reader.GetInt32(0), reader.GetString(1), reader.GetString(4), reader.GetInt32(3), reader.GetInt32(2));
                AddTicket(newTicket);
            }).Execute();

        }

        /// <summary>
        /// Creates a jail point
        /// </summary>
        private void CreateJailPoint()
        {
            jailCheckpoint = new Checkpoint(this.prisonEntranceLocation, this.OnEnterCheckpoint, this.OnExitCheckpoint, 1, prisonEntranceCircleSize, 0);
        }


        /// <summary>
        /// Is ran when character enters checkpoint
        /// </summary>
        /// <param name="point">Checkpoint</param>
        /// <param name="character">Character</param>
        private void OnEnterCheckpoint(Checkpoint point, Character character)
        {
            if (character.factionID == FactionI.LAW_ENFORCEMENT)
            {
                character.PlayFrontendSound("SELECT", "HUD_LIQUOR_STORE_SOUNDSET");
                character.SendChatMessage("[NOTE]: Arrest a player in this zone by using /arrest [id] [time] [reason]");
            }
        }

        private void OnExitCheckpoint(Checkpoint point, Character character) { }


        /// <summary>
        /// Sets a character to jail
        /// </summary>
        /// <param name="arrester">Arrester</param>
        /// <param name="characterToJail">Character to send to jail</param>
        /// <param name="time">Time for which to jail</param>
        /// <param name="reason">Reason of jailing</param>
        private void SetCharacterToJail(Character arrester, Character characterToJail, int time, string reason)
        {
            if (arrestedCharacters.ContainsKey(characterToJail.ID))
            {
                arrester.SendErrorNotification("Error: That character is already jailed!");
                return;
            }

            arrestedCharacters.Add(characterToJail.ID, new Arrest(time, reason));
            DBManager.InsertQuery("INSERT INTO jailed_characters VALUES (@id, @time, @reason)")
                .AddValue("@id", characterToJail.ID)
                .AddValue("@time", time)
                .AddValue("@reason", reason)
                .Execute();
            characterToJail.position = prisonSpawnLocation;
        }

        /// <summary>
        /// Initializes timer to check for prisoner status
        /// </summary>
        private void InitializeJailTimer()
        {
            MapManager.Instance().SubscribeToOnMinuteChange(this.CheckJailTimes);
        }

        /// <summary>
        /// Releases a character from prison
        /// </summary>
        /// <param name="character">Character to release</param>
        private void ReleaseCharacterFromJail(Character character)
        {
            arrestedCharacters.Remove(character.ID);
            DBManager.DeleteQuery("DELETE FROM jailed_characters WHERE id=@id")
                .AddValue("@id", character.ID)
                .Execute();
            character.position = prisonReleaseLocation;
            character.SendSuccessNotification("You have been released from prison!");
        }

        /// <summary>
        /// Checks jail times for all players in server and minuses one if character online
        /// </summary>
        /// <param name="time">Current time</param>
        private void CheckJailTimes(TimeSpan time)
        {
            List<Character> characters = PlayerManager.Instance().GetActiveCharacters();
            foreach (Character character in characters)
            {
                if (arrestedCharacters.ContainsKey(character.ID))
                {
                    arrestedCharacters[character.ID].minutes -= 1;
                    if (arrestedCharacters[character.ID].minutes <= 0) {
                        ReleaseCharacterFromJail(character);
                    }
                    else
                    {
                        DBManager.UpdateQuery("UPDATE jailed_characters SET time=@time WHERE id=@id")
                        .AddValue("@time", arrestedCharacters[character.ID].minutes)
                        .AddValue("@id", character.ID)
                        .Execute();
                    }
                }
            }
        }

        /// <summary>
        /// Initializes the faction
        /// </summary>
        public override void Initialize()
        {
            base.Initialize();
            CreateJailPoint();
            InitializeRanks();
            LoadTicketsFromDB();
            LoadJailedCharactersFromDB();
            InitializeJailTimer();
            MapManager.Instance().AddBlipToMap(188, "Los Santos Prison", prisonEntranceLocation.X, prisonEntranceLocation.Y, prisonEntranceLocation.Z);
        }

        /// <summary>
        /// Teleports character to jail if he/she has time left to server
        /// </summary>
        /// <param name="character">Character to teleport to jail</param>
        /// <returns>True if has jail time left, otherwise false</returns>
        public bool MoveCharacterToJailIfJailTimeLeft(Character character)
        {
            if (arrestedCharacters.ContainsKey(character.ID))
            {
                character.position = prisonSpawnLocation;
                return true;
            }
            return false;
        }

        /// <summary>
        /// Gets the remaining jail time for character
        /// </summary>
        /// <param name="character">Character</param>
        /// <returns>Remaining jail time</returns>
        public int GetJailTimeLeftForCharacter(Character character)
        {
            if (arrestedCharacters.ContainsKey(character.ID))
            {
                return arrestedCharacters[character.ID].minutes;
            }
            return 0;
        }


        /// <summary>
        /// Returns chat color for the law enforcement
        /// </summary>
        /// <returns>Color to use in chat for law enforcement players' names</returns>
        public override string GetChatColor()
        {
            return "~b~";
        }

        /// <summary>
        /// Arrests a character and teleports him/her to jail
        /// </summary>
        /// <param name="arrester">Arrester</param>
        /// <param name="characterToArrestId">Id of character to arrest</param>
        /// <param name="time">Time to spend in jail</param>
        /// <param name="reason">Reason for arrest</param>
        public void ArrestCharacter(Character arrester, int characterToArrestId, int time, string reason)
        {
            // 1. Player has to be in certain area
            // 2. Teleport player to jail and set timer
            // 3. Every minute check the timer and remove one minute if player online
            if (PlayerManager.Instance().IsCharacterWithIdOnline(characterToArrestId))
            {
                Character characterToArrest = PlayerManager.Instance().GetCharacterWithId(characterToArrestId);
                if (!jailCheckpoint.IsCharacterInsideCheckpoint(arrester))
                {
                    arrester.SendErrorNotification("Error: You have to be within the jail checkpoint to arrest a character!");
                    return;
                }

                if (!jailCheckpoint.IsCharacterInsideCheckpoint(characterToArrest))
                {
                    arrester.SendErrorNotification("Error: The character you are trying to arrest must be within the jail checkpoint!");
                    return;
                }

                characterToArrest.SendNotification("You have been arrested by officer " + arrester.fullName + " for " + time.ToString() + " minutes. Reason: " + reason);
                this.SetCharacterToJail(arrester, characterToArrest, time, reason);

            }
            else
            {
                arrester.SendNotification("Error: Character with that id is not online!");
            }
        }


        /// <summary>
        /// Gives a fine for a character
        /// </summary>
        /// <param name="sender">The character who wrote the ticket</param>
        /// <param name="characterToFine">Character to who the ticket is written for</param>
        /// <param name="reason">Reason of the ticket</param>
        /// <param name="fineAmount"></param>
        public void FineCharacter(Character sender, int characterToFine, string reason, int fineAmount)
        {
            if (PlayerManager.Instance().IsCharacterWithIdOnline(characterToFine))
            {
                Character character = PlayerManager.Instance().GetCharacterWithId(characterToFine);
                if (character.position.DistanceTo(sender.position) <= ticketGiveDistance)
                {
                    Ticket ticket = new Ticket(currentTicketId, sender.fullName, reason, fineAmount, character.ID);
                    AddTicket(ticket, true);
                    character.SendNotification("You were given a ticket by " + sender.fullName + " with a fine of $" + fineAmount.ToString());
                    character.PlayFrontendSound("SELECT", "HUD_LIQUOR_STORE_SOUNDSET");
                }
                else
                {
                    sender.SendNotification("You need to be closer to the character!");
                }
            }
            else
            {
                sender.SendNotification("Character with ID " + characterToFine.ToString() + " is not online!");
            }
        }


        /// <summary>
        /// Handles on duty command for law enforcement players
        /// </summary>
        /// <param name="c">Character object</param>
        public override void HandleOnDutyCommand(Character c)
        {
            if (IsCharacterPartOfFaction(c))
            {
                if (!c.onDuty)
                {
                    CharacterInfo info = GetInfoForCharacter(c);
                    Rank rank = GetRank(info.rank);

                    c.onDuty = true;
                    c.SetModel(info.onDutyModel);

                    // Determine what to do based on rank
                    // Add weapons, items etc

                }
                else
                {
                    c.onDuty = false;
                    c.SetModel(c.model);
                }
            }
        }
    }
}
