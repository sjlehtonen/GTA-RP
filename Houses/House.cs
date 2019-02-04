using System;
using System.Collections.Generic;

namespace GTA_RP.Houses
{
    /// <summary>
    /// Class for a building that can have players in it
    /// </summary>
    class House
    {
        public String name { get; private set; }
        public int id { get; private set; }
        public int ownerId { get; private set; }
        public int templateId { get; private set; }
        private List<Character> occupants = new List<Character>();
        private List<Character> invitedPersons = new List<Character>();

        public House(int id, int ownerId, int templateId, String name)
        {
            this.name = name;
            this.id = id;
            this.ownerId = ownerId;
            this.templateId = templateId;
        }

        /// <summary>
        /// Adds an occupant to the building
        /// </summary>
        /// <param name="character">Character to be added inside</param>
        public void AddOccupant(Character character)
        {
            occupants.Add(character);
        }

        /// <summary>
        /// Removes an occupant from the building
        /// </summary>
        /// <param name="character">Character to be removed</param>
        public void RemoveOccupant(Character character)
        {
            occupants.Remove(character);
        }

        /// <summary>
        /// Adds a character to the invited persons list of the building
        /// Now the invited character can access the building
        /// </summary>
        /// <param name="character">Character to be added to the list</param>
        public void AddInvitation(Character character)
        {
            invitedPersons.Add(character);
        }

        /// <summary>
        /// Removes a character from the invited persons list
        /// Now the uninvited character can no longer access the building
        /// </summary>
        /// <param name="character">Character to be uninvited from the building</param>
        public void RemoveInvitation(Character character)
        {
            invitedPersons.Remove(character);
        }

        /// <summary>
        /// Checks if a character is invited to the building
        /// </summary>
        /// <param name="character">Character that is checked</param>
        /// <returns>True if character is on the invited list, otherwise false</returns>
        public Boolean IsInvited(Character character)
        {
            return invitedPersons.Contains(character);
        }

        /// <summary>
        /// Checks if character is inside the building
        /// </summary>
        /// <param name="character"></param>
        /// <returns>True if character is inside the building, otherwise false</returns>
        public Boolean HasOccupant(Character character)
        {
            return occupants.Contains(character);
        }

        /// <summary>
        /// Checks if two houses are the same
        /// </summary>
        /// <param name="obj">House</param>
        /// <returns>True if house ids are same, otherwise false</returns>
        public override bool Equals(object obj)
        {
            var second = obj as House;

            if (second == null)
            {
                return false;
            }

            return this.id.Equals(second.id);
        }

        /// <summary>
        /// Gets hashcode of house id
        /// </summary>
        /// <returns>Hashcode of house id</returns>
        public override int GetHashCode()
        {
            return this.id.GetHashCode();
        }
    }
}
