using System;
using System.IO;

namespace MultiAdmin.MultiAdmin.Features
{
	class ModLog : Feature, IEventAdminAction
	{
		private Boolean logToOwnFile;
		private String modLogLocation;
        private String fromWho;

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
            this.fromWho = Server.MultiAdminCfg.GetValue("server_tag", "false");
            Server.modLogPath = this.modLogLocation;

            while (true) 
            {
                if (this.fromWho == "false")
                {
                    this.fromWho = Server.MultiAdminCfg.GetValue("server_tag", "false");
                }
                else
                {
                    break;
                }
            }
        }

		public void OnAdminAction(string message)
		{
			if (logToOwnFile)
			{
				lock (this)
				{
					using (StreamWriter sw = File.AppendText(this.modLogLocation))
					{
                        string data = fromWho + ": " + message.Substring(9, message.Length - 9);
                        // Проверка по параше
                        if (!(data.Contains("::ffff:")))
                        {
                            data += "`%";
                        }
                        sw.WriteLine(data);
                        GUI.UpdateModLog(data.Substring(0, data.Length - 2));
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
