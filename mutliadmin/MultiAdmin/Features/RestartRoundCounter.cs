using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MultiAdmin.MultiAdmin.Commands
{
    class RestartRoundCounter : Feature, IEventRoundEnd
    {
        private int count;
        private int restartAfter;

        public RestartRoundCounter(Server server) : base(server)
        {
        }

        public override void Init()
        {
            count = 0;
        }

        public override void OnConfigReload()
        {
            restartAfter = Server.ServerConfig.GetIntValue("RESTART_EVERY_NUM_ROUNDS", -1);
        }

        public void OnRoundEnd()
        {
            if (restartAfter < 0) return;
            count++; 
            if (count > restartAfter) base.Server.SoftRestartServer();
        }


        public override string GetFeatureDescription()
        {
            return "Перезапускает сервер после N'ного количества раундов.";
        }

        public override string GetFeatureName()
        {
            return "Перезапускает сервер после N раундов";
        }


    }
}
