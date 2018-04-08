﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MultiAdmin.MultiAdmin.Commands
{
    class MemoryChecker : Feature, IEventTick
    {
        private int lowMb;
        private int tickCount;
        public MemoryChecker(Server server) : base(server)
        {
        }

        public override void Init()
        {
            tickCount = 0;
        }

        public override string GetFeatureDescription()
        {
            return "Restarts the server if the working memory becomes too low";
        }

        public override string GetFeatureName()
        {
            return "Restart On Low Memory";
        }

        public void OnTick()
        {
            Server.GetGameProccess().Refresh();
            long workingMemory = Server.GetGameProccess().WorkingSet64 / 1048576L; // process memory in MB
            long memoryLeft = 2048 - workingMemory; // 32 bit limited to 2GB

            if (memoryLeft < lowMb)
            {
                Server.Write("Внимание: программа запущенна с недостаточной памятью (" + memoryLeft + " MB осталось)");
                tickCount++;
            }
            else
            {
                tickCount = 0;
            }

            if (tickCount == 10)
            {
                Server.Write("Перезапуск в связи с недосатком памяти");
                Server.SoftRestartServer();
            }
 
        }

        public override void OnConfigReload()
        {
            lowMb = Server.ServerConfig.GetIntValue("RESTART_LOW_MEMORY", 400);
        }
    }
}
