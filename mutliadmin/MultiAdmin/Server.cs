using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using MultiAdmin.MultiAdmin.Commands;
using MultiAdmin.MultiAdmin.Features;
using MutliAdmin;

namespace MultiAdmin.MultiAdmin
{
    public class Server
    {
        public static readonly string MA_VERSION = "Pre 2.0";

        public Boolean HasServerMod { get; set; }
        public String ServerModVersion { get; set; }
        public Config MultiAdminCfg { get; }
        public Config ServerConfig
        {
            get
            {
                return serverConfig;
            }
        }
        public String ConfigKey { get; }
        public String MainConfigLocation { get; }
        public String ConfigChain { get; }
        public String ServerDir { get; }

        private Config serverConfig;
        public Boolean InitialRoundStarted { get; set; }

        public List<Feature> Features { get; }
        public Dictionary<String, ICommand> Commands { get; }
        private List<IEventTick> tick; // we want a tick only list since its the only event that happens constantly, all the rest can be in a single list

        private Thread loopThread;
        private Thread printerThread;

        private int logID;
        private Process gameProcess;
        private Boolean stopping;
        private String session_id;
        private String maLogLocation;
        public String StartDateTime { get; }
        public String LogFolder { get; }
        public Boolean fixBuggedPlayers;

        public static Boolean GUIIsOn;
        public static DateTime serverStart;
        public static String modLogPath;

        public static List<String> UniqueBase = new List<String>();
        public static Process Monitor { get; set; }

        private String currentLine = "";

        public Server(String serverDir, String configKey, Config multiAdminCfg, String mainConfigLocation, String configChain)
        {
            serverStart = DateTime.Now;
            MainConfigLocation = mainConfigLocation;
            ConfigKey = configKey;
            ConfigChain = configChain;
            ServerDir = serverDir;
            session_id = Utils.GetUnixTime().ToString();
            Commands = new Dictionary<string, ICommand>();
            Features = new List<Feature>();
            tick = new List<IEventTick>();
            MultiAdminCfg = multiAdminCfg;
            LogFolder = "servers" + Path.DirectorySeparatorChar + ConfigKey + Path.DirectorySeparatorChar + "logs" + Path.DirectorySeparatorChar;
            StartDateTime = Utils.GetDate();
            maLogLocation = LogFolder + StartDateTime + "_MA_output_log.txt";
            stopping = false;
            InitialRoundStarted = false;
            loopThread = new Thread(new ThreadStart(() => MainLoop()));
            printerThread = new Thread(new ThreadStart(() => OutputThread.Read(this)));

            // Register all features 
            RegisterFeatures();
            // Load config 
            serverConfig = new Config(ServerDir + Path.DirectorySeparatorChar + ConfigKey + Path.DirectorySeparatorChar + "config.txt");
            // Init features
            InitFeatures();
            // Start the server and threads
            if (StartServer())
            {
                printerThread.Start();
                loopThread.Start();
            }
        }


        private void RegisterFeatures()
        {
            RegisterFeature(new Autoscale(this));
            RegisterFeature(new ChainStart(this));
            RegisterFeature(new ConfigReload(this));
            RegisterFeature(new ExitCommand(this));
            // RegisterFeature(new EventTest(this));
            RegisterFeature(new GithubGenerator(this));
            RegisterFeature(new GithubLogSubmitter(this));
            RegisterFeature(new HelpCommand(this));
            RegisterFeature(new InactivityShutdown(this));
            RegisterFeature(new MemoryChecker(this));
            RegisterFeature(new MemoryCheckerSoft(this));
            RegisterFeature(new ModLog(this));
            RegisterFeature(new MultiAdminInfo(this));
            RegisterFeature(new NewCommand(this));
            RegisterFeature(new Restart(this));
            RegisterFeature(new RestartNextRound(this));
            RegisterFeature(new RestartRoundCounter(this));
            RegisterFeature(new StopNextRound(this));
            RegisterFeature(new UniqueLogins(this));
            RegisterFeature(new Titlebar(this));
        }

        private void InitFeatures()
        {
            foreach (Feature feature in Features)
            {
                feature.Init();
                feature.OnConfigReload();
            }
        }

        public void MainLoop()
        { 
            while (!stopping)
            {
                if (gameProcess != null && !gameProcess.HasExited)
                {
                    foreach (IEventTick tickEvent in tick)
                    {
                        tickEvent.OnTick();
                    }
                }
                else if (!stopping)
                {
                    foreach (Feature f in Features)
                    {
                        if (f is IEventCrash)
                        {
                            ((IEventCrash)f).OnCrash();
                        }
                    }

                    Write("Проблемма с игрой *Lag.Crash.ServerIsFull* ( выход/краш/закрытие/перезапуск )");
                    Write("Очищение сессии");
                    session_id = Utils.GetUnixTime().ToString();
                    Write("Запуск новой сессии");
                    try
                    {
                        // Connection Data
                        int port = Convert.ToInt32(MultiAdminCfg.GetValue("bot_port", "false"));
                        string ip = MultiAdminCfg.GetValue("bot_ip", "false");

                        // null checks

                        if (port < 80)
                        {
                            port = 80;
                        }

                        if (ip == null || ip == "")
                        {
                            ip = "127.0.0.1";
                        }

                        // Links
                        Encoding encoding = Encoding.UTF8;
                        Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);


                        // Body
                        socket.Connect(ip, port);
                        byte[] buffer = encoding.GetBytes("%' " + MultiAdminCfg.GetValue("server_tag", "false") + ", CRASH");
                        socket.Send(buffer);
                        socket.Disconnect(true);
                        Write("Отправлено CRUSH сообщение");

                    }
                    catch
                    {

                        Write("Ошибка в отправке CRASH сообщения");

                    }
                    StartServer();
                    InitFeatures();
                }
                Thread.Sleep(1000);
            }
            Thread.Sleep(100);
            CleanUp();
        }

        public Boolean IsStopping()
        {
            return stopping;
        }

        public void RegisterFeature(Feature feature)
        {
            if (feature is IEventTick) tick.Add((IEventTick)feature);
            if (feature is ICommand)
            {
                ICommand command = (ICommand)feature;
                Commands.Add(command.GetCommand().ToLower().Trim(), command);
            }
            Features.Add(feature);
        }



        public void PrepareFiles()
        {
            try
            {
                Directory.CreateDirectory("SCPSL_Data" + Path.DirectorySeparatorChar + "Dedicated" + Path.DirectorySeparatorChar + session_id);
                Write("Начата новая сессия.");
            }
            catch
            {
                Write("Неудача - Пожалуйста закройте все файлы в SCPSL_Data/Dedicated и перезапустите игру!");
                Write("Закройте сервер самостоятельно.");
            }

        }

        public void Write(String message, int height = 0)
        {
            Log(message);
            if (Server.SkipProcessHandle() || Process.GetCurrentProcess().MainWindowHandle != IntPtr.Zero)
            {
                DateTime now = DateTime.Now;
                string str = "[" + now.Hour.ToString("00") + ":" + now.Minute.ToString("00") + ":" + now.Second.ToString("00") + "] ";
                GUI.Write(message == "" ? "" : str + message);
            }
        }

        public static bool SkipProcessHandle()
        {
            int p = (int)Environment.OSVersion.Platform;
            return (p == 4) || (p == 6) || (p == 128); // Outputs true for Unix
        }

        public void WritePart(String part, int height = 0, bool date = false, bool lineEnd = false)
        {
            String datepart = "";
            if (date)
            {
                DateTime now = DateTime.Now;
                datepart = "[" + now.Hour.ToString("00") + ":" + now.Minute.ToString("00") + ":" + now.Second.ToString("00") + "] ";
            }
            if (lineEnd)
            {
                GUI.Write(datepart + part + Environment.NewLine);
                Log(currentLine);
                currentLine = "";
            }
            else
            {
                GUI.Write(datepart + part);
            }
        }

        public void Log(String message)
        {
            lock (this)
            {
                using (StreamWriter sw = File.AppendText(this.maLogLocation))
                {
                    DateTime now = DateTime.Now;
                    string date = "[" + now.Hour.ToString("00") + ":" + now.Minute.ToString("00") + ":" + now.Second.ToString("00") + "] ";
                    sw.WriteLine(date + message);
                }
            }

        }

        public void SoftRestartServer()
        {
            if (ServerModCheck(1, 5, 0))
            {
                SendMessage("RECONNECTRS");
                session_id = Utils.GetUnixTime().ToString();
            }
            else
            {
                gameProcess.Kill();
            }
        }

        public Boolean ServerModCheck(int major, int minor, int fix)
        {
            if (this.ServerModVersion == null)
            {
                return false;
            }

            String[] parts = ServerModVersion.Split('.');
            int verMajor = 0;
            int verMinor = 0;
            int verFix = 0;
            if (parts.Length == 3)
            {
                Int32.TryParse(parts[0], out verMajor);
                Int32.TryParse(parts[1], out verMinor);
                Int32.TryParse(parts[2], out verFix);
            }
            else if (parts.Length == 2)
            {
                Int32.TryParse(parts[0], out verMajor);
                Int32.TryParse(parts[1], out verMinor);
            }
            else
            {
                return false;
            }

            if (major == 0 && minor == 0 && verFix == 0)
            {
                return false;
            }

            return (verMajor > major) || (verMajor >= major && verMinor > minor) || (verMajor >= major && verMinor >= minor && verFix >= fix);

        }

        public void RestartServer()
        {
            gameProcess.Kill();
            Process.Start(Directory.GetFiles(Directory.GetCurrentDirectory(), "MultiAdmin.*")[0], ConfigKey);
            stopping = true;
        }

        public void StopServer(bool killGame = true)
        {
            foreach (Feature f in Features)
            {
                if (f is IEventServerStop)
                {
                    ((IEventServerStop)f).OnServerStop();
                }
            }

            if (killGame) gameProcess.Kill();
            stopping = true;
        }

        public void CleanUp()
        {
            RemoveRunFile();
            DeleteSession();
        }


        public Boolean StartServer()
        {
            Boolean started = false;
            InitialRoundStarted = false;
            try
            {
                PrepareFiles();
                string[] files = Directory.GetFiles(Directory.GetCurrentDirectory(), "SCPSL.*", SearchOption.TopDirectoryOnly);
                Write("Исполнение: " + files[0]);
                SwapConfigs();
                string args = "-batchmode -nographics -key" + session_id + " -silent-crashes -id" + (object)Process.GetCurrentProcess().Id;
                Write("Запуск сервера с параметрами");
                Write(files[0] + " " + args);
                gameProcess = Process.Start(files[0], args);
                CreateRunFile();
                started = true;
                foreach (Feature f in Features)
                {
                    if (f is IEventServerPreStart)
                    {
                        ((IEventServerPreStart)f).OnServerPreStart();
                    }
                }
            }
            catch (Exception e)
            {
                Write("Неудача - Исполняемый файл не найден или конфиг поврежден!");
                Write(e.Message);
                Write("Закройте сервер самостоятельно.");
                RemoveRunFile();
            }

            return started;
        }

        public Process GetGameProccess()
        {
            return gameProcess;
        }

        public void SwapConfigs()
        {
            if (File.Exists("servers" + Path.DirectorySeparatorChar + ConfigKey + Path.DirectorySeparatorChar + "config.txt"))
            {
                var contents = File.ReadAllText(Directory.GetCurrentDirectory() + Path.DirectorySeparatorChar + "servers" + Path.DirectorySeparatorChar + ConfigKey + Path.DirectorySeparatorChar + "config.txt");
                File.WriteAllText(MainConfigLocation, contents);
                Write("Файл конфига сменен");
            }
            else
            {
                Write("Конфиг для этого сервера " + ConfigKey + " не существует! Предпологаемая локация:" + "servers\\" + ConfigKey + "\\config.txt");
                throw new FileNotFoundException("config file not found");
            }

        }

        private void RemoveRunFile()
        {
            File.Delete(ServerDir + Path.DirectorySeparatorChar + ConfigKey + Path.DirectorySeparatorChar + "running");
        }

        private void CreateRunFile()
        {
            File.Create(ServerDir + Path.DirectorySeparatorChar + ConfigKey + Path.DirectorySeparatorChar + "running").Close();
        }

        private void CleanSession()
        {
            String path = "SCPSL_Data" + Path.DirectorySeparatorChar + "Dedicated" + Path.DirectorySeparatorChar + session_id;
            if (Directory.Exists(path))
            {
                foreach (String file in Directory.GetFiles(path))
                {
                    File.Delete(file);
                }
            }

        }

        private void DeleteSession()
        {
            CleanSession();
            string path = "SCPSL_Data" + Path.DirectorySeparatorChar + "Dedicated" + Path.DirectorySeparatorChar + session_id;
            if (Directory.Exists(path)) Directory.Delete(path);
        }


        public String GetSessionId()
        {
            return session_id;
        }

        public Boolean IsConfigRunning(String config)
        {
            return File.Exists(ServerDir + Path.DirectorySeparatorChar + config + Path.DirectorySeparatorChar + "running");
        }

        public void NewInstance(String configChain)
        {
            String file = Directory.GetFiles(Directory.GetCurrentDirectory(), "MultiAdmin.*")[0];
            ProcessStartInfo psi = new ProcessStartInfo(file, configChain);
            Process.Start(psi);
        }

        public void SendMessage(string message)
        {
            StreamWriter streamWriter = new StreamWriter("SCPSL_Data" + Path.DirectorySeparatorChar + "Dedicated" + Path.DirectorySeparatorChar + session_id + Path.DirectorySeparatorChar + "cs" + logID + ".mapi");
            logID++;
            streamWriter.WriteLine(message + "terminator");
            streamWriter.Close();
            Write("Отправка запроса к игре...");
        }
    }
}