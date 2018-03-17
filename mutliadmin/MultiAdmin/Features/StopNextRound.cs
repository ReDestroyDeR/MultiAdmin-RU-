using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MultiAdmin.MultiAdmin.Commands
{
    class StopNextRound : Feature, ICommand, IEventRoundEnd
    {
        private Boolean stop;

        public StopNextRound(Server server) : base(server)
        {
            stop = false;
        }

        public string GetCommandDescription()
        {
            return "Останавливает сервер на конце раунда.";
        }

        public override void Init()
        {
            stop = false;
        }


        public override void OnConfigReload()
        {
        }

        public void OnCall(string[] args)
        {
			Server.Write("Сервер остановится на следующем раунде");
			stop = true;
        }

        public void OnRoundEnd()
        {
            if (stop) base.Server.StopServer();
        }

        public bool PassToGame()
        {
            return false;
        }

        public bool RequiresServerMod()
        {
            return false;
        }

        public override string GetFeatureDescription()
        {
            return "Останавливает сервер когда текущий раунд закончится.";
        }

        public override string GetFeatureName()
        {
            return "Останавливает Следующий Раунд";
        }

        public string GetCommand()
        {
            return "STOPNEXTROUND";
        }

        public string GetUsage()
        {
            return "";
        }
    }
}
