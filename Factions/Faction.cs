using System;

namespace GTA_RP.Factions
{
    /// <summary>
    /// Base class for faction
    /// </summary>
    abstract public class Faction
    {
        public FactionEnums id { get; private set; }
        public String name { get; private set; }
        public int colorR { get; private set; }
        public int colorG { get; private set; }
        public int colorB { get; private set; }

        /// <summary>
        /// Returns the HEX color code for the faction
        /// </summary>
        public string hexColor
        {
            get
            {
                return "#" + colorR.ToString("X2") + colorG.ToString("X2") + colorB.ToString("X2");
            }
        }

        /// <summary>
        /// Initializes data needed for the faction
        /// </summary>
        public abstract void Initialize();

        public abstract string GetRankText(Character character);


        /// <summary>
        /// A method that defines what to do when player uses /duty command
        /// </summary>
        /// <param name="character">Character who used the command</param>
        public abstract void HandleOnDutyCommand(Character character);

        /// <summary>
        /// A method that returns the color that this faction's character's name displays in chat
        /// </summary>
        /// <returns>Color that is used to display character's name in chat</returns>
        public abstract string GetChatColor();

        public virtual void PaySalary() { }

        public Faction(FactionEnums id, String name, int colorR, int colorG, int colorB)
        {
            this.id = id;
            this.name = name;
            this.colorR = colorR;
            this.colorG = colorG;
            this.colorB = colorB;
        }
    }
}
