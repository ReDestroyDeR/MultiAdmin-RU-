using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MultiAdmin.MultiAdmin.Features
{
    class ConfigReload : Feature, ICommand
    {
        Boolean pass;

        public ConfigReload(Server server) : base(server)
        {
        }

        public string GetCommand()
        {
            return "CONFIG";
        }

        public string GetCommandDescription()
        {
            return "Перезагружает конфиг";
        }

        public override string GetFeatureDescription()
        {
            return "Перезагрузка конфигов поменяет их местами";
        }

        public override string GetFeatureName()
        {
            return "Перезапуск";
        }

        public string GetUsage()
        {
            return "<reload>";
        }

        public override void Init()
        {
            pass = true;
        }

        public void OnCall(string[] args)
        {
            if (args.Length == 0) return;
            if (args[0].ToLower().Equals("reload"))
            {
                Server.SwapConfigs();
                pass = true;
                Server.Write("Перезапуск конфига");
                Server.Write("если конфиг открывается в блокноте, НЕ ПАНИКУЙ, СОЛДАТ, ща все заработает.");
                Server.ServerConfig.Reload();
                foreach (Feature feature in Server.Features)
                {
                    feature.OnConfigReload();
                }
            }

        }

        public override void OnConfigReload()
        {
        }

        public bool PassToGame()
        {
            return pass;
        }
    }
}
