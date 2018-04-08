using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MultiAdmin.MultiAdmin.Features
{
    public class HelpCommand : Feature, ICommand
    {
        public HelpCommand(Server server) : base(server)
        {
        }

        public string GetCommand()
        {
            return "HELP";
        }

        public override void OnConfigReload()
        {
        }

        public string GetCommandDescription()
        {
            return "Выводит список команд и что они делают.";
        }

        public override string GetFeatureDescription()
        {
            return "Выводит полный список MultiAdmin команд и команд которые доступны в игре.";
        }

        public override string GetFeatureName()
        {
            return "Помощь";
        }

        public override void Init()
        {
        }

        public void OnCall(string[] args)
        {
            Server.Write("Команды Консоли:");
            List<String> helpOutput = new List<String>();
            foreach (KeyValuePair<String, ICommand> command in base.Server.Commands)
            {
                String usage = command.Value.GetUsage();
                if (usage.Length > 0) usage = " " + usage;
                helpOutput.Add(String.Format("{0}{1}: {2}", command.Key.ToUpper(), usage, command.Value.GetCommandDescription()));
            }

            helpOutput.Sort();

            foreach (String line in helpOutput)
            {
                Server.Write(line);
            }

            Server.Write("Команды Игры:");
        }

        public bool PassToGame()
        {
            return true;
        }

        public string GetUsage()
        {
            return "";
        }
    }
}
