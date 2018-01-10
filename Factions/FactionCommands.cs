using GrandTheftMultiplayer.Server.API;
using GrandTheftMultiplayer.Server.Elements;
using GrandTheftMultiplayer.Server.Managers;
using GrandTheftMultiplayer.Shared;
using GrandTheftMultiplayer.Shared.Math;

namespace GTA_RP.Factions
{
    class FactionCommands : Script
    {
        private NetHandle entity;
        private float rotX;
        private float rotY;
        private float rotZ;
        private float posX;
        private float posY;
        private float posZ;

        [Command("srx")]
        public void setRotX(Client c, string rot)
        {
            this.rotX = int.Parse(rot);
            this.testObject(c);
        }

        [Command("sry")]
        public void setRotY(Client c, string rot)
        {
            this.rotY = int.Parse(rot);
            this.testObject(c);
        }

        [Command("srz")]
        public void setRotZ(Client c, string rot)
        {
            this.rotZ = int.Parse(rot);
            this.testObject(c);
        }

        [Command("spx")]
        public void setPosX(Client c, string rot)
        {
            this.posX = float.Parse(rot);
            this.testObject(c);
        }

        [Command("spy")]
        public void setPosY(Client c, string rot)
        {
            this.posY = float.Parse(rot);
            this.testObject(c);
        }

        [Command("spz")]
        public void setPosZ(Client c, string rot)
        {
            this.posZ = float.Parse(rot);
            this.testObject(c);
        }

        /// <summary>
        /// Handles /duty command and directs it to the right class
        /// </summary>
        /// <param name="player"></param>
        [Command("duty")]
        public void toggleDuty(Client player)
        {
            if (PlayerManager.Instance().IsClientUsingCharacter(player))
            {
                Character character = PlayerManager.Instance().GetActiveCharacterForClient(player);
                FactionManager.Instance().HandleDutyCommand(character);
            }
        }

        [Command("testr")]
        public void testObject(Client player)
        {
            if (PlayerManager.Instance().IsClientUsingCharacter(player))
            {
                API.shared.consoleOutput("rotX: " + this.rotX + " - rotY: " + this.rotY + " - rotZ: " + this.rotZ);
                API.shared.consoleOutput("posX: " + this.posX + " - posY: " + this.posY + " - posZ: " + this.posZ);
                API.deleteEntity(entity);
                Character character = PlayerManager.Instance().GetActiveCharacterForClient(player);
                character.PlayAnimation((int)(AnimationFlags.AllowPlayerControl | AnimationFlags.Loop | AnimationFlags.OnlyAnimateUpperBody), "amb@world_human_stand_fishing@base", "base");
                entity = API.shared.createObject(1338703913, character.position, new Vector3(0, 0, 0));
                character.AttachObject(entity, "57005", new Vector3(posX,posY,posZ), new Vector3(rotX,rotY,rotZ));
            }
        }

    }
}
