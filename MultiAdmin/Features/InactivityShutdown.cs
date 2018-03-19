using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MultiAdmin.MultiAdmin.Commands
{
    class InactivityShutdown : Feature, IEventRoundStart, IEventRoundEnd, IEventTick
    {
        private Boolean waiting;
        private long roundEndTime;
        private int waitFor;

        public InactivityShutdown(Server server) : base(server)
        {
        }

        public override void Init()
        {
            roundEndTime = Utils.GetUnixTime();
        }

        public override void OnConfigReload()
        {
            waitFor = Server.ServerConfig.GetIntValue("SHUTDOWN_ONCE_EMPTY_FOR", -1);
        }

        public void OnRoundEnd()
        {
            roundEndTime = Utils.GetUnixTime();
            waiting = true;
        }


        public override string GetFeatureDescription()
        {
            return "Остановка сервера после неактивности.";
        }

        public override string GetFeatureName()
        {
            return "Остановить Сервер За Неактивность.";
        }

        public void OnRoundStart()
        {
            waiting = false;
        }

        public void OnTick()
        {
            if (waitFor > 0 && waiting)
            {
                long elapsed = Utils.GetUnixTime() - roundEndTime;

                if (elapsed >= waitFor)
                {
                    Server.Write("Сервер был неактивен " + waitFor + " секунд, остановка...");
                    Server.StopServer();
                }
            }
        }
    }
}
