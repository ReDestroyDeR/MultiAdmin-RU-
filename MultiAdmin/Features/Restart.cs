using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MultiAdmin.MultiAdmin.Features
{
	class Restart : Feature, ICommand
	{
		public Restart(Server server) : base(server)
		{
		}

		public string GetCommand()
		{
			return "restart";
		}

		public string GetCommandDescription()
		{
			return "Перезапускает игру ( Не консоль! )";
		}

		public override string GetFeatureDescription()
		{
			return "Позволяет произвести перезапуск игры без перезапуска Консоли.";
		}

		public override string GetFeatureName()
		{
			return "Команда перезапуска.";
		}

		public string GetUsage()
		{
			return "";
		}

		public override void Init()
		{
		}

		public void OnCall(string[] args)
		{
			this.Server.SoftRestartServer();
		}

		public override void OnConfigReload()
		{
		}

		public bool PassToGame()
		{
			return false;
		}
	}
}
