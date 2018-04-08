using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MultiAdmin.MultiAdmin.Commands
{
    class Titlebar : Feature, IEventPlayerConnect, IEventPlayerDisconnect, IEventServerStart
    {
        private int playerCount;
        private int maxPlayers;

        public Titlebar(Server server) : base(server)
        {
        }


        public override string GetFeatureDescription()
        {
            return "Updates the title bar with instance based information, such as session id and player count. (Requires servermod to function fully)";
        }

        public override string GetFeatureName()
        {
            return "Titlebar";
        }

        public override void Init()
        {
            maxPlayers = Server.ServerConfig.GetIntValue("MAX_PLAYERS", 20);
            playerCount = -1; // -1 for the "server" player, once the server starts this will increase to 0.
            UpdateTitlebar();
        }

        public override void OnConfigReload()
        {
        }

        public void OnPlayerConnect(String name)
        {
            Console.Write("Player Connected " + name + Environment.NewLine);
            playerCount++;
            UpdateTitlebar();
        }

        public void OnPlayerDisconnect(String name)
        {
            Console.Write("Player Disconnected " + name + Environment.NewLine);
            playerCount--;
            UpdateTitlebar();
        }

        public void OnServerStart()
        {
            UpdateTitlebar();
        }

        public void UpdateTitlebar()
        {
			if (Server.SkipProcessHandle() || Process.GetCurrentProcess().
                MainWindowHandle != IntPtr.Zero)
			{
                var displayPlayerCount = playerCount;
                var smod = "";

				if (Server.HasServerMod)
				{
					smod = Server.ServerModVersion;
				}
                else {
                    smod = "SMod Не найден";
                }
				if (playerCount == -1) displayPlayerCount = 0;
				string proccessId = (Server.GetGameProccess() == null) ? "Запуск сервера . . ." : Server.GetGameProccess().Id.ToString();
                // MA_Version
                // ServerName
                // Players
                // MaxPlayers
                // SMod Version
                GUI.ServerData[0] = Server.MA_VERSION;
                GUI.ServerData[1] = Server.MultiAdminCfg.GetValue("server_tag");
                GUI.ServerData[2] = displayPlayerCount.ToString();
                GUI.ServerData[3] = maxPlayers.ToString();
                GUI.ServerData[4] = smod;
                GUI.UpdateServerInfo();
			}
        }
    }
}