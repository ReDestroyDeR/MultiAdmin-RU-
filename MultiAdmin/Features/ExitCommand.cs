using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MultiAdmin.MultiAdmin.Features
{
    class ExitCommand : Feature, ICommand
    {
        Boolean pass;

        public ExitCommand(Server server) : base(server)
        {
        }

        public string GetCommand()
        {
            return "EXIT";
        }

        public override void OnConfigReload()
        {
        }

        public string GetCommandDescription()
        {
            return "Остановка Сервера";
        }

        public override string GetFeatureDescription()
        {
            return "Добавляет прекрасную команду для элегантной остановки сервера.";
        }

        public override string GetFeatureName()
        {
            return "Остановка сервера";
        }

        public string GetUsage()
        {
            return "";
        }

        public override void Init()
        {
            pass = true;
        }

        public void OnCall(string[] args)
        {
            Server.StopServer(false);
        }

        public bool PassToGame()
        {
            return pass;
        }
    }
}
