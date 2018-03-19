using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MultiAdmin.MultiAdmin.Commands
{
    class NewCommand : Feature, ICommand
    {
        private String config;

        public NewCommand(Server server) : base(server)
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
            return "Стартует новый сервак.";
        }

        public override string GetFeatureName()
        {
            return "Новый";
        }
        

        public void OnCall(string[] args)
        {
            if (args.Length == 0)
            {
                Server.Write("Имя конфига гони сюда", ConsoleColor.Magenta);
            }
            else
            {
                // maybe check if the config exists?
                Server.NewInstance(args[0].ToLower());
            }
        }

        public string GetCommand()
        {
            return "NEW";
        }

        public bool PassToGame()
        {
            return false;
        }

        public string GetCommandDescription()
        {
            return "Запускает новый сервак с подогнанным конфигом.";
        }


        public string GetUsage()
        {
            return "<config_id>";
        }
    }
}
