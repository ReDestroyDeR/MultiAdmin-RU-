using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MultiAdmin.MultiAdmin.Commands
{
    class ChainStart : Feature, IEventServerStart
    {
        private Boolean dontstart;

        public ChainStart(Server server) : base(server)
        {
            dontstart = false;
        }

        public override void Init()
        {
        }

        public override string GetFeatureDescription()
        {
            return "Автоматически запускает следующий сервер, когда текущий прогружается.";
        }

        public override string GetFeatureName()
        {
            return "Цепной-Старт";
        }

        public override void OnConfigReload()
        {
        }


        public void OnServerStart()
        {
            if ((!(String.IsNullOrWhiteSpace(Server.ConfigChain) || Server.ConfigChain.Trim().Equals("\"\""))) && !dontstart)
            {
                dontstart = true;
                Server.Write("Запуск следующего сервера с цепным конфигом:" + Server.ConfigChain);
                Server.NewInstance(Server.ConfigChain);
            }
        }
    }
}
