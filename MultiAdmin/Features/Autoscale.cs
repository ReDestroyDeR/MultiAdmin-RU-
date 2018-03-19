using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MultiAdmin.MultiAdmin.Commands
{
    class Autoscale : Feature, IEventServerFull
    {
        private String config;

        public Autoscale(Server server) : base(server)
        {
        }

        public override void Init()
        {
        }

        public override void OnConfigReload()
        {
            config = Server.ServerConfig.GetValue("START_CONFIG_ON_FULL", "disabled");
        }

        public override string GetFeatureDescription()
        {
            return "Автоматически запускает новый сервер, когда другой заполнен. (Требуется ServerMod для полной функциональности)";
        }

        public override string GetFeatureName()
        {
            return "Авто-Расширение";
        }

        public void OnServerFull()
        {
            if (!config.Equals("disabled"))
            {
                if (!Server.IsConfigRunning(config))
                {
                    Server.NewInstance(config);
                }
            }
        }
    }
}
