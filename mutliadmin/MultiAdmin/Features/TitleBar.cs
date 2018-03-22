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
            playerCount++;
            UpdateTitlebar();
        }

        public void OnPlayerDisconnect(String name)
        {
            playerCount--;
            UpdateTitlebar();
        }

        public void OnServerStart()
        {
            UpdateTitlebar();
        }

        public void UpdateTitlebar()
        {
			if (Server.SkipProcessHandle() || Process.GetCurrentProcess().MainWindowHandle != IntPtr.Zero)
			{
                var displayPlayerCount = playerCount;
                var smod = "";

				if (Server.HasServerMod)
				{
					smod = "SMod " + Server.ServerModVersion;
				}
                else if (displayPlayerCount != 0)
                {
                    smod = "Проверка на Сервер Мод . . .";
                }
                else {
                    smod = "Установите СерверМод!";
                }
				if (playerCount == -1) displayPlayerCount = 0;
				string proccessId = (Server.GetGameProccess() == null) ? "Запуск сервера . . ." : Server.GetGameProccess().Id.ToString();
				Console.Title = "MultiAdmin-RU " + Server.MA_VERSION + " | " + Server.ConfigKey + " | Session:" + Server.GetSessionId() + " PID: " + proccessId + " | " + displayPlayerCount + "/" + maxPlayers + " | " + smod;
			}
        }
    }
}