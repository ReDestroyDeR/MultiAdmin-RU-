using System;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using MultiAdmin.MultiAdmin;

namespace MultiAdmin
{
    class OutputThread
    {
		public static readonly Regex smodRegex = new Regex(@"(\[.*?\]) (\[.*?\]) (.*)");

		public static void Read(Server server)
        {
            while (!server.IsStopping())
            {
                string[] strArray = null;
				String dir = "SCPSL_Data" + Path.DirectorySeparatorChar + "Dedicated" + Path.DirectorySeparatorChar + server.GetSessionId();

				try
                {
					if (Directory.Exists(dir))
					{
						strArray = Directory.GetFiles(dir, "sl*.mapi", SearchOption.TopDirectoryOnly).OrderBy(f => f).ToArray<String>();
					}
                }
                catch
                {
					if (!server.IsStopping())
					{
						server.Write("Ошибка выводителя сообщений: Директория 'SCPSL_Data/Dedicated' не найдена.");
					}                    
                }

                if (strArray == null) continue;
                foreach (string path in strArray)
                {

                    string gameMessage = "";
                    string fileCommand = "open";
                    int attempts = 0;
                    Boolean read = false;

                    while (attempts < 100 && !read && !server.IsStopping())
                    {
                        try
                        {
                            StreamReader streamReader = new StreamReader(path);
                            gameMessage = streamReader.ReadToEnd();
                            fileCommand = "close";
                            streamReader.Close();
                            fileCommand = "delete";
                            File.Delete(path);
                            read = true;
                        }
                        catch
                        {
                            attempts++;
							if (attempts >= 100)
							{
								server.Write("Ошибка выводителя сообщений: Невозможно " + fileCommand + " файл " + path + ". Убедитесь что MultiAdmin.exe имеет все права для Чтения/Записи.");
								server.Write("пропуск");
							}
                        }
                        Thread.Sleep(300);
                    }

                    if (server.IsStopping()) break;

                    Boolean display = true;

                    if (!string.IsNullOrEmpty(gameMessage.Trim()))
                    {
                        if (gameMessage.Contains("LOGTYPE"))
                        {
                            String type = gameMessage.Substring(gameMessage.IndexOf("LOGTYPE")).Trim();
                            gameMessage = gameMessage.Substring(0, gameMessage.IndexOf("LOGTYPE")).Trim();
                        }
                       
                    }

					// Smod2 loggers pretty printing


					var match = smodRegex.Match(gameMessage);
					if (match.Success)
					{
						if (match.Groups.Count >= 2)
						{
							server.WritePart("", 0, true, false);
							server.WritePart(match.Groups[1].Value + " ", 0, false, false);
							server.WritePart(match.Groups[2].Value + " ", 0, false, false);
							server.WritePart(match.Groups[3].Value, 0, false, true);
							display = false;
						}

					}


					if (gameMessage.Contains("Mod Log:"))
					{
						foreach (Feature f in server.Features)
						{
							if (f is IEventAdminAction)
							{
								((IEventAdminAction)f).OnAdminAction(gameMessage.Replace("Действие Админа:", ""));
							}
						}
					}

                    if (gameMessage.Contains("ServerMod"))
                    {
                        server.HasServerMod = true;
                        server.ServerModVersion = gameMessage.Replace("СерверМод - Версия", "").Trim();
                    }

					if (server.ServerModCheck(1, 7, 2))
					{
						if (gameMessage.Contains("Round restarting"))
						{
							foreach (Feature f in server.Features)
							{
								if (f is IEventRoundEnd)
								{
									((IEventRoundEnd)f).OnRoundEnd();
								}
							}
						}

						if (gameMessage.Contains("Waiting for players"))
						{
                            if (!server.InitialRoundStarted)
							{
								server.InitialRoundStarted = true;
								foreach (Feature f in server.Features)
								{
									if (f is IEventRoundStart)
									{
										((IEventRoundStart)f).OnRoundStart();
									}
								}
							}

                            if (server.ServerModCheck(1, 5, 0) && server.fixBuggedPlayers)
                            {
                                server.SendMessage("ROUNDRESTART");
                                server.fixBuggedPlayers = false;
                            }
                        }
					}
					else
					{
						if (gameMessage.Contains("Waiting for players"))
						{
							if (!server.InitialRoundStarted)
							{
								server.InitialRoundStarted = true;
								foreach (Feature f in server.Features)
								{
									if (f is IEventRoundStart)
									{
										((IEventRoundStart)f).OnRoundStart();
									}
								}
							}
							else
							{
								foreach (Feature f in server.Features)
								{
									if (f is IEventRoundEnd)
									{
										((IEventRoundEnd)f).OnRoundEnd();
									}
								}
							}

                            if (server.ServerModCheck(1, 5, 0) && server.fixBuggedPlayers)
                            {
                                server.SendMessage("ROUNDRESTART");
                                server.fixBuggedPlayers = false;
                            }
                        }
					}



                    if (gameMessage.Contains("New round has been started"))
                    {
                        foreach (Feature f in server.Features)
                        {
                            if (f is IEventRoundStart)
                            {
                                ((IEventRoundStart)f).OnRoundStart();
                            }
                        }
                    }

                    if (gameMessage.Contains("Level loaded. Creating match..."))
                    {
                        foreach (Feature f in server.Features)
                        {
                            if (f is IEventServerStart)
                            {
                                ((IEventServerStart)f).OnServerStart();
                            }
                        }
                    }


                    if (gameMessage.Contains("Server full"))
                    {
                        foreach (Feature f in server.Features)
                        {
                            if (f is IEventServerFull)
                            {
                                ((IEventServerFull)f).OnServerFull();
                            }
                        }
                    }


                    if (gameMessage.Contains("Player connect"))
                    {
                    	display = false;

                        server.Log("Player connect event");
                        foreach (Feature f in server.Features)
                        {
                            if (f is IEventPlayerConnect)
                            {
                                String name = gameMessage.Substring(gameMessage.IndexOf(":"));
                                ((IEventPlayerConnect)f).OnPlayerConnect(name);
                            }
                        }
                    }

                    if (gameMessage.Contains("Player disconnect"))
                    {
                    	display = false;

                        server.Log("Player disconnect event");
                        foreach (Feature f in server.Features)
                        {
                            if (f is IEventPlayerDisconnect)
                            {
                                String name = gameMessage.Substring(gameMessage.IndexOf(":"));
                                ((IEventPlayerDisconnect)f).OnPlayerDisconnect(name);
                            }
                        }
                    }

                    if (gameMessage.Contains("Player has connected before load is complete"))
                    {
                        if (server.ServerModCheck(1, 5, 0))
                        {
                            server.fixBuggedPlayers = true;
                        }
                    }

                    if (display) {

                    	if (gameMessage.Contains("Round restarting")) {
                    		server.Write("Перезапуск раунда");
                    	}
                    		

                    	else if (gameMessage.Contains("Waiting for players")) {
                    		server.Write("Ожидание игроков");
                    	}
                    		
						else if (gameMessage.Contains("New round has been started")) {
							server.Write("Новый раунд был начат");
						}
								

						else if (gameMessage.Contains("Level loaded. Creating match...")) {
							server.Write("Уровень загружен. Создание матча...");
						}
							

						else if (gameMessage.Contains("Server full")) {
							server.Write("Сервер полон");
						}
								

						else if (gameMessage.Contains("Player has connected before load is complete")) {
							server.Write("Игрок подключился до завершения загрузки");
						}
								

                    	else
                        {
                            server.Write(gameMessage.Trim());
                        }
                    }
                }
                Thread.Sleep(10);
            }
        }

    }
}