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
            return "Обновляет ТайтлБар. ( Требуется servermod для полной функциональности )";
        }

        public override string GetFeatureName()
        {
            return "ТайтлБар";
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
				var smod = "";
				if (Server.HasServerMod)
				{
					smod = "SMod " + Server.ServerModVersion;
				}
				var displayPlayerCount = playerCount;
				if (playerCount == -1) displayPlayerCount = 0;
				string proccessId = (Server.GetGameProccess() == null) ? "" : Server.GetGameProccess().Id.ToString();
				Console.Title = "MultiAdmin " + Server.MA_VERSION + " | Сервер: " + Server.ConfigKey + " | Сессия:" + Server.GetSessionId() + " PID: " + proccessId + " | " + displayPlayerCount + "/" + maxPlayers + " | " + smod;
			}
        }
    }
}
