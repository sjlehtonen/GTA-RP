using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GrandTheftMultiplayer.Server.API;
using GrandTheftMultiplayer.Server.Elements;
using GrandTheftMultiplayer.Server.Managers;
using System.Reflection;

namespace GTA_RP.Factions
{
    struct FactionCommand
    {
        public FactionCommand(string handlerMethod, params Type[] arguments)
        {
            this.arguments = new List<Type>(arguments);
            this.handlerMethod = handlerMethod;
        }

        public List<Type> arguments;
        public string handlerMethod;
    }

    /// <summary>
    /// Base class for faction
    /// </summary>
    abstract public class Faction
    {
        public FactionI id { get; private set; }
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
        /// <param name="c">Character who used the command</param>
        public abstract void HandleOnDutyCommand(Character c);

        /// <summary>
        /// A method that returns the color that this faction's character's name displays in chat
        /// </summary>
        /// <returns>Color that is used to display character's name in chat</returns>
        public abstract string GetChatColor();

        public Faction(FactionI id, String name, int colorR, int colorG, int colorB)
        {
            this.id = id;
            this.name = name;
            this.colorR = colorR;
            this.colorG = colorG;
            this.colorB = colorB;
        }
    }
}
