using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MultiAdmin.MultiAdmin.Commands
{
    class RestartNextRound : Feature, ICommand, IEventRoundEnd
    {
        private Boolean restart;

        public RestartNextRound(Server server) : base(server)
        {
        }

        public override void Init()
        {
            restart = false;
        }

        public string GetCommandDescription()
        {
            return "Перезапуск сервера на конце раунда.";
        }


        public void OnCall(string[] args)
        {
            Server.Write("Сервер перезапустится на следующем раунде");
            restart = true;
        }

        public void OnRoundEnd()
        {
            if (restart) base.Server.SoftRestartServer();
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
            return "Перезапускает сервер когда текущий раунд подходит к концу.";
        }

        public override string GetFeatureName()
        {
            return "Перезапуск Следующего Раунда";
        }

        public string GetCommand()
        {
            return "RESTARTNEXTROUND";
        }

        public string GetUsage()
        {
            return "";
        }

        public override void OnConfigReload()
        {
        }
    }
}
