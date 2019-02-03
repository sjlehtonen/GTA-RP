using System;
using GrandTheftMultiplayer.Server.API;
using GrandTheftMultiplayer.Server.Elements;
using GrandTheftMultiplayer.Server.Managers;
using GrandTheftMultiplayer.Shared.Math;

namespace GTA_RP.Admin
{
    /// <summary>
    /// TODO: Implement all admin commands here
    /// This file is heavily work in progress and is only used for testing purposes
    /// </summary>
    class AdminCommands : Script
    {
        int id = 0;
        Vehicle testVehicle = null;

        [Command("login")]
        public void login(Client client, string password)
        {
            PlayerManager.Instance().HandlePlayerLogin(client, password);
        }

        [Command("spawnvehicle", Alias = "sv")]
        public void spawnVehicle(Client client, String vehicle, String color1, String color2)
        {
            testVehicle = API.createVehicle(API.vehicleNameToModel(vehicle), client.position, client.rotation, Int32.Parse(color1), Int32.Parse(color2));
        }

        [Command("loadipl", Alias = "li")]
        public void loadIpl(Client client, String ipl)
        {
            IPLManager.IPLManager.Instance().LoadIPL(ipl);
        }

        [Command("coords")]
        public void getCoords(Client client)
        {
            API.sendChatMessageToPlayer(client, client.position.X.ToString() + " " + client.position.Y.ToString() + " " + client.position.Z.ToString());
        }

        [Command("rot")]
        public void getRot(Client client)
        {
            API.sendChatMessageToPlayer(client, client.rotation.X.ToString() + " " + client.rotation.Y.ToString() + " " + client.rotation.Z.ToString());
        }

        [Command("setid", Alias = "si")]
        public void setid(Client client, String id)
        {
            this.id = Int32.Parse(id);
        }

        [Command("tele", Alias = "t")]
        public void teleport(Client client, float x, float y, float z)
        {
            client.position = new Vector3(x, y, z);
        }


        [Command("spawnvehicle2", Alias = "sv2")]
        public void spawnVehicletest(Client client, String vehicle)
        {
            API.createVehicle(API.vehicleNameToModel(vehicle), client.aimingPoint, client.rotation, 0, 0);
        }

        [Command("setmodel")]
        public void changeModel(Client client, String model)
        {
            API.setPlayerSkin(client, API.pedNameToModel(model));
        }

        [Command("weather")]
        public void changeWeather(Client client, String weather)
        {
            API.setWeather(Convert.ToInt32(weather));
        }

        [Command("time")]
        public void changeTime(Client client, String hours, String minutes)
        {
            API.setTime(Convert.ToInt32(hours), Convert.ToInt32(minutes));
        }

        [Command("weapon", Alias = "wp")]
        public void giveWeapon(Client client, String weapon)
        {
            client.giveWeapon(API.weaponNameToModel(weapon), 100, true, true);
        }

        public void createFactionVehicle(Client client, String faction, String plateText)
        {
            var cmd = DBManager.SimpleQuery("INSERT INTO vehicles VALUES (@id, @ownerId, @faction, @model, @park_x, @park_y, @park_z, @park_rot_x, @park_rot_y, @park_rot_z, @platetxt, @style, @color1, @color2)");
            Vehicle veh = client.vehicle;
            cmd.Parameters.AddWithValue("@id", id);
            id++;
            cmd.Parameters.AddWithValue("@ownerId", -1);
            cmd.Parameters.AddWithValue("@faction", Int32.Parse(faction));
            cmd.Parameters.AddWithValue("@model", veh.model);
            cmd.Parameters.AddWithValue("@park_x", veh.position.X);
            cmd.Parameters.AddWithValue("@park_y", veh.position.Y);
            cmd.Parameters.AddWithValue("@park_z", veh.position.Z);
            cmd.Parameters.AddWithValue("@park_rot_x", veh.rotation.X);
            cmd.Parameters.AddWithValue("@park_rot_y", veh.rotation.Y);
            cmd.Parameters.AddWithValue("@park_rot_z", veh.rotation.Z);
            cmd.Parameters.AddWithValue("@platetxt", "");
            cmd.Parameters.AddWithValue("@style", 0);
            cmd.Parameters.AddWithValue("@color1", veh.primaryColor);
            cmd.Parameters.AddWithValue("@color2", veh.secondaryColor);
            cmd.ExecuteNonQuery();
        }

        [Command("createpolicevehicle")]
        public void createPoliceVehicle(Client client)
        {
            createFactionVehicle(client, "1", "LSPD");
        }

        [Command("tenginea")]
        public void ToggleEngineA(Client client)
        {
            API.setVehicleEngineStatus(API.getPlayerVehicle(client), true);
        }
    }
}
