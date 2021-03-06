﻿using System;
using System.IO;

namespace MultiAdmin.MultiAdmin.Features
{
	class ModLog : Feature, IEventAdminAction
	{
		private Boolean logToOwnFile;
		private String modLogLocation;

		public ModLog(Server server) : base(server)
		{
		}

		public override string GetFeatureDescription()
		{
			return "Логирование админ-действий в файл, их вывод.";
		}

		public override string GetFeatureName()
		{
			return "Админ-Лог";
		}

		public override void Init()
		{
			logToOwnFile = false;
			this.modLogLocation = "modlogs" + Path.DirectorySeparatorChar + System.DateTime.Now.ToString("yyyy-MM-dd") + "_MODERATOR_output_log.txt";
		}

		public void OnAdminAction(string message)
		{
			if (logToOwnFile)
			{
				lock (this)
				{
					using (StreamWriter sw = File.AppendText(this.modLogLocation))
					{
						sw.WriteLine(message);
					}
				}
			}
		}

		public override void OnConfigReload()
		{
			logToOwnFile = Server.ServerConfig.GetBoolean("log_mod_actions_to_own_file", false);
		}
	}
}
