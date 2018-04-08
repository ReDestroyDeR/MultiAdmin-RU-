using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MultiAdmin.MultiAdmin.Features
{
    public class UniqueLogins : Feature, IEventPlayerConnect
    {
        public UniqueLogins(Server server) : base(server)
        {
        }

        public override void OnConfigReload()
        {
        }

        public override string GetFeatureDescription()
        {
            return "Записыавет количество уникальных подключений.";
        }

        public override string GetFeatureName()
        {
            return "Уникальные Подключения";
        }

        public override void Init()
        {
        }

        public void OnCall(string[] args)
        {
        }

        public bool PassToGame()
        {
            return false;
        }

        public void OnPlayerConnect(string name)
        {
            if (!Server.UniqueBase.Contains(name)) Server.UniqueBase.Add(name);
        }

        public string GetUsage()
        {
            return "";
        }
    }
}
