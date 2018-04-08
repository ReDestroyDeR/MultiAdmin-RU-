using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace MultiAdmin.MultiAdmin
{
    public partial class GUI : Form
    {

        private static string configKey;
        private static string configLocation;
        private static string configChain;
        private static Config multiadminConfig;
        private static Server server;

        private static string[] a_SERVER = { "Перезапуск", "Перезагрузка", "Остановка" };
        private static string[] a_END = { "Перезапуск ( END ROUND )", "Остановка ( END ROUND )" };
        private static string[] a_PLAYER = { "Выдать предмет", "Выдать класс", "Кикнуть", "Бан на 15 мин.", "Бан на 30 мин.",
            "Бан на 1 час", "Бан на 2 часа", "Бан на 3 часа", "Бан на 4 часа", "Бан на 6 часов", "Бан на 8 часов", "Бан на 12 часов",
            "Бан на 1 день", "Бан на 3 дня", "Бан на 5 дней", "Бан на 7 дней", "Бан на 14 дней", "Бан на месяц", "Пермаментный Бан"};
        private static string[] a_ITEMS = { "Janitor keycard", "Scientist keycard", "Major scientist keycard", "Zone manager keycard",
            "Guard keycard", "Senior guard keycard", "Containment engineer keycard", "MTF lieutenant keycard", "MTF commander keycard",
            "Facility manager keycard", "Chaos Insurgency device", "O5 level keycard", "Radio", "M1911 Pistol", "Medkit", "Flashlight",
            "MicroHID", "Coin", "Cup", "Ammometer"};
        private static string[] a_CLASS = { "SCP-173", "Class-D Personnel", "Spectator", "SCP-106", "Nine-Tailed Fox Scientist",
            "SCP-049", "Scientist", "SCP-079", "Chaos Insurgency", "SCP-457", "SCP-049-2", "Nine-Tailed Fox Lieutenant",
            "Nine-Tailed Fox Commander", "Nine-Tailed Fox Guard", "TUTORIAL" };

        private static string[] args;
        public static string[] ServerData = new string[5];
        // MA_Version
        // ServerName
        // Players
        // MaxPlayers
        // SMod Version

        public static void UpdateServerInfo()
        {
            double UPT = (DateTime.Now - Server.serverStart).TotalSeconds;
            string ADD = " секунд";
            if (UPT > 60 && ADD == " секунд")
            {
                UPT = UPT / 60;
                ADD = " минут";
            }
            if (UPT > 60 && ADD == " минут")
            {
                UPT = UPT / 60;
                ADD = " часов";
            }
            if (UPT > 24 && ADD == " часов")
            {
                UPT = UPT / 24;
                ADD = " дней";
            }

            MultiAdmin_Version.Text = "MA-RU: " + ServerData[0];
            ServerName.Text = ServerData[1];
            PlayerCount.Text = ServerData[2] + "/" + ServerData[3];
            ServerMod_Version.Text = ServerData[4];
            Uptime.Text = "Uptime: " + Math.Round(UPT) + ADD;
            UniqueLogins.Text = "Unique Logins: " + Server.UniqueBase.Count;
            ServerIP.Text = "IP: " + multiadminConfig.GetValue("server_ip") + ":" + multiadminConfig.GetValue("port_queue");
        }

        private static void UpdateShell(String data)
        {
            Shell.AppendText(data);
            Shell.AppendText(Environment.NewLine);
        }

        public static void UpdateModLog(String data)
        {
            ModeratorLog.AppendText(data);
            ModeratorLog.AppendText(Environment.NewLine);
        }

        // Ниже предоставлен сектор программы

        public static void Write(String message)
        {
            if (Server.SkipProcessHandle() || Process.GetCurrentProcess().MainWindowHandle != IntPtr.Zero)
            {
                DateTime now = DateTime.Now;
                if (message[0].ToString() != "[")
                {
                    string str = "[" + now.Hour.ToString("00") + ":" + now.Minute.ToString("00") + ":" + now.Second.ToString("00") + "] ";
                    UpdateShell(message == "" ? "" : str + message);
                }
                else
                {
                    string str = null;
                    UpdateShell(message == "" ? "" : str + message);
                }
            }
        }

        public static bool FindConfig()
        {
            var defaultLoc = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "/SCP Secret Laboratory/config.txt";
            var path = multiadminConfig.GetValue("cfg_loc", defaultLoc);
            var backup = path.Replace(".txt", "_backup.txt");

            if (!File.Exists(path))
            {
                Write("Стандартный конфиг файл ожидался (" + path + "), копирование config_template.txt");
                File.Copy("config_template.txt", path);
            }

            if (File.Exists(path))
            {
                configLocation = path;
                Write("Конфиг файл создан: " + path);

                if (!File.Exists(backup))
                {
                    Write("У конфига нету бекапа, создание бекапа под: " + backup);
                    File.Copy(path, backup);
                }
            }
            else
            {
                // should never happen
                throw new FileNotFoundException("Config.txt не найден! Что то пошло не так при инсталяции, попробуйте запустить LocalAdmin.exe first");
            }

            return true;
        }

        public static Boolean StartHandleConfigs(string[] args)
        {
            Boolean hasServerToStart = false;
            if (args.Length > 0)
            {
                configKey = args[0];
                hasServerToStart = true;
                multiadminConfig = new Config(Directory.GetCurrentDirectory() + Path.DirectorySeparatorChar + "servers" + Path.DirectorySeparatorChar + configKey + Path.DirectorySeparatorChar + "config.txt");
                Write("Запуск новой инстанции с конфигом:" + configKey);
                // chain the rest
                string[] newArgs = args.Skip(1).Take(args.Length - 1).ToArray();
                configChain = "\"" + string.Join("\" \"", newArgs).Trim() + "\"";
            }
            else
            {
                // start all servers, the first server will be this one
                bool first = true;
                if (!Directory.Exists(Directory.GetCurrentDirectory() + Path.DirectorySeparatorChar + "servers"))
                {
                    Write("Серверная директория не была найдена, создайте ее по формату:");
                    Write(Directory.GetCurrentDirectory() + "servers\\<Server id>\\config.txt");
                    Write("Когда будет исправленно, перезапустите данный EXE.");
                    return false;
                }

                String[] dirs = Directory.GetDirectories(Directory.GetCurrentDirectory() + Path.DirectorySeparatorChar + "servers" + Path.DirectorySeparatorChar);
                foreach (string file in dirs)
                {
                    String name = new DirectoryInfo(file).Name;
                    if (first)
                    {
                        multiadminConfig = new Config(file + Path.DirectorySeparatorChar + "config.txt");
                        Write(Directory.GetCurrentDirectory() + Path.DirectorySeparatorChar + "servers" + Path.DirectorySeparatorChar + name + Path.DirectorySeparatorChar + "config.txt");
                        if (multiadminConfig.GetValue("MANUAL_START", "false").Equals("true"))
                        {
                            Write("Пропуск авто старта для: " + name);
                        }
                        else
                        {
                            hasServerToStart = true;
                            configKey = name;
                            Write("Запуск инстанции с конфигом: " + name);
                            first = false;
                        }

                    }
                    else
                    {
                        var other_config = new Config(Directory.GetCurrentDirectory() + Path.DirectorySeparatorChar + "servers" + Path.DirectorySeparatorChar + name + Path.DirectorySeparatorChar + "config.txt");
                        Write(Directory.GetCurrentDirectory() + Path.DirectorySeparatorChar + "servers" + Path.DirectorySeparatorChar + name + Path.DirectorySeparatorChar + "config.txt");
                        if (other_config.GetValue("MANUAL_START", "false").Equals("true"))
                        {
                            Write("Пропуск авто старта для: " + name);
                        }
                        else
                        {
                            configChain += "\"" + name + "\" ";
                        }

                    }

                    // make log folder

                    if (!Directory.Exists(file + Path.DirectorySeparatorChar + "logs"))
                    {
                        Directory.CreateDirectory(file + Path.DirectorySeparatorChar + "logs");
                    }
                }

            }

            if (!hasServerToStart)
            {
                Write("Все сервера на ручном запуске! Вы должны иметь хотя бы один конфиг который будет на авто старте.");
            }

            return hasServerToStart;
        }

        public static String GetServerDirectory()
        {
            return Directory.GetCurrentDirectory() + Path.DirectorySeparatorChar + "servers";
        }

        static void OnExit(object sender, EventArgs e)
        {
            Write("Остановка сервера");
            Debug.Write("Остановка сервера");
        }

        private static void FixTypo()
        {
            // some idiot (courtney) accidently made the config file spc_multiadmin.cfg instead of scp_multiadmin.cfg
            // this method fixes it
            if (File.Exists("spc_multiadmin.cfg"))
            {
                Write("Переименование с spc_multiadmin.cfg на scp_multiadmin.cfg");
                File.Move("spc_multiadmin.cfg", "scp_multiadmin.cfg");
            }
        }

        public GUI(string[] args = null)
        {
            if (args != null)
            {
                GUI.args = args;
                Console.Write("Called with args" + Environment.NewLine);
                InitializeComponent();
                UpdateShell("Executing Server . . .");
                FixTypo();
                Console.Write("Setting Up Items in ActionType box" + Environment.NewLine);
                ActionType.Items.Add("Сервер");
                ActionType.Items.Add("Сервер ( В конце раунда )");
                ActionType.Items.Add("Игрок");
                Console.WriteLine("Checking for SCPSL Monitor");
                ExistingManager();
                multiadminConfig = new Config("scp_multiadmin.cfg");
                if (!FindConfig())
                {
                    return;
                }
                configChain = "";
                if (StartHandleConfigs(args))
                {
                    server = new Server(GetServerDirectory(), configKey, multiadminConfig, configLocation, configChain);
                }
            }
            else
            {
                Console.Write("Called without args");
            }
        }

        private void GUI_Load(object sender, EventArgs e)
        {
        }

        private void ActionType_SelectedIndexChanged(object sender, EventArgs e)
        {
            Argument.Enabled = false;
            Username.ReadOnly = true;

            switch (ActionType.SelectedIndex)
            {
                // Сервер
                case 0:
                    Action.Enabled = true;
                    Action.Text = "";
                    Action.Items.Clear();
                    Action.Items.AddRange(a_SERVER);
                    break;

                // Сервер ( В Конце Раунда )
                case 1:
                    Action.Enabled = true;
                    Action.Text = "";
                    Action.Items.Clear();
                    Action.Items.AddRange(a_END);
                    break;

                // Игрок
                case 2:
                    Action.Enabled = true;
                    Action.Text = "";
                    Action.Items.Clear();
                    Action.Items.AddRange(a_PLAYER);
                    break;
            }
        }

        private void InputLine_TextChanged(object sender, EventArgs e)
        {
            Console.Write("Input Line Text Changed" + Environment.NewLine);
            var tb = (TextBox)sender;

            // Input Line
            try
            {
                if (tb.Text[tb.TextLength - 1].ToString() == "/")
                {
                    bool kill = false;

                    if (tb.Text[tb.TextLength - 2].ToString() == "`") kill = true;

                    string message = tb.Text.Trim(new Char[] { '/', '`' });

                    tb.Text = "";
                    server.Write(">>> " + message, -1);
                    string[] strArray = message.ToUpper().Split(' ');
                    if (strArray.Length > 0)
                    {
                        ICommand command;
                        Boolean callServer = true;
                        server.Commands.TryGetValue(strArray[0].ToLower().Trim(), out command);
                        if (command != null)
                        {
                            command.OnCall(strArray.Skip(1).Take(strArray.Length - 1).ToArray());
                            callServer = command.PassToGame();
                        }

                        if (callServer) server.SendMessage(message);
                    }
                    if (kill)
                    {
                        Process proc = Process.GetCurrentProcess();
                        proc.Kill();
                    }
                }
            }
            catch
            {
                Console.Write("Line is Empty" + Environment.NewLine);
            }
        }

        private void Action_SelectedIndexChanged(object sender, EventArgs e)
        {
            switch (Action.Items[Action.SelectedIndex])
            {
                case "Выдать предмет":
                    Console.WriteLine("Item ACTION");
                    Argument.Text = "";
                    Argument.Items.Clear();
                    Argument.Items.AddRange(a_ITEMS);
                    Argument.Enabled = true;
                    break;
                case "Выдать класс":
                    Console.WriteLine("Class ACTION");
                    Argument.Text = "";
                    Argument.Items.Clear();
                    Argument.Items.AddRange(a_CLASS);
                    Argument.Enabled = true;
                    break;
                default:
                    String text = Action.Text.Split(" ".ToCharArray()[0])[0];
                    if (text == "Бан" || text == "Пермаментный" || text == "Кикнуть")
                    {
                        Console.WriteLine("Ban ACTION");
                        Argument.Text = "";
                        Argument.Items.Clear();
                        Argument.Enabled = false;
                        Username.ReadOnly = false;
                    }
                    else
                    {
                        Console.WriteLine("Default ACTION");
                        Argument.Text = "";
                        Argument.Items.Clear();
                        Argument.Enabled = false;
                    }
                    break;
            }
        }

        private void Banner2000()
        {
            int time;
            String target = Username.Text;

            /*private static string[] a_PLAYER = { "Выдать предмет", "Выдать класс", "Кикнуть", "Бан на 15 мин.", "Бан на 30 мин.",
            *   "Бан на 1 час", "Бан на 2 часа", "Бан на 3 часа", "Бан на 4 часа", "Бан на 6 часов", "Бан на 8 часов", "Бан на 12 часов",
            *   "Бан на 1 день", "Бан на 3 дня", "Бан на 5 дней", "Бан на 7 дней", "Бан на 14 дней", "Бан на месяц", "Пермаментный Бан"};
            */

            switch (Action.SelectedIndex)
            {
                case 2:
                    time = 0;
                    break;
                case 3:
                    time = 15;
                    break;
                case 4:
                    time = 30;
                    break;
                case 5:
                    time = 60;
                    break;
                case 6:
                    time = 2*60;
                    break;
                case 7:
                    time = 3*60;
                    break;
                case 8:
                    time = 4*60;
                    break;
                case 9:
                    time = 6*60;
                    break;
                case 10:
                    time = 8*60;
                    break;
                case 11:
                    time = 12*60;
                    break;
                case 12:
                    time = 24*60;
                    break;
                case 13:
                    time = 3*24*60;
                    break;
                case 14:
                    time = 5*24*60;
                    break;
                case 15:
                    time = 7*24*60;
                    break;
                case 16:
                    time = 2*7*24*60;
                    break;
                case 17:
                    time = 4*7*24*60;
                    break;
                case 18:
                    time = 50*4*7*24*60;
                    break;
                default:
                    return;
            }

            InputLine.Text = "ban " + target + " " + time + "/";
            UpdateModLog("Забанен игрок " + target + " на " + Action.Items[Action.SelectedIndex]);
        }

        private bool ClassNItemGiver(String id, Boolean Class)
        {
            string[] items;
            string target = Username.Text;

            // Определить что мы используем
            // Класс или Предметы?
            if (Class) items = a_CLASS;
            else items = a_ITEMS;

            // Действия при выдаче класса
            if (Class)
            {
                InputLine.Text = "forceclass " + id + " " + target + "/";
                return true;
            }
            // Действия при выдаче предмета
            else
            {
                InputLine.Text = "give " + id + " " + target + "/";
                return true;
            }
        }

        private void SendButton_Click(object sender, EventArgs e)
        {
            var item = Action.Text;
            Console.Write(item + Environment.NewLine);
            String message = "";
            switch (item)
            {
                case "Перезапуск":
                    message = "Перезапуск сервера через консоль";
                    InputLine.Text = "restart/";
                    break;
                case "Перезагрузка":
                    message = "Перезагрузка сервера через консоль";
                    InputLine.Text = "new " + configKey + "`/";
                    break;
                case "Остановка":
                    message = "Остановка сервера через консоль";
                    Process proc = Process.GetCurrentProcess();
                    proc.Kill();
                    break;
                case "Перезапуск ( END ROUND )":
                    message = "Перезапуск сервера в конце раунда через консоль";
                    InputLine.Text = "restartnextround/";
                    break;
                case "Остановка ( END ROUND )":
                    message = "Остановка сервера в конце раунда";
                    InputLine.Text = "stopnextround/";
                    break;
                case "Выдать предмет":
                    UpdateModLog("Игроку " + Username.Text + " был выдан предмет " + Argument.Text);
                    ClassNItemGiver(Argument.SelectedIndex.ToString(), false);
                    break;
                case "Выдать класс":
                    UpdateModLog("Игроку " + Username.Text + " был выдан класс " + Argument.Text);
                    ClassNItemGiver(Argument.SelectedIndex.ToString(), true);
                    break;
                default:
                    String text = item.Split(" ".ToCharArray()[0])[0];
                    if (text == "Бан" || text == "Пермаментный" || text == "Кикнуть")
                    {
                        Banner2000();
                    }
                    break;
            }
            try
            {
                Write(message);
            }
            catch
            {
                Console.WriteLine("Success...");
            }
            ActionType.Text = "";
            Action.Text = "";
            Argument.Text = "";
            Username.Text = "";
            Action.Items.Clear();
            Argument.Items.Clear();
            Action.Enabled = false;
            Username.ReadOnly = true;
            Argument.Enabled = false;
        }

        private void Argument_SelectedIndexChanged(object sender, EventArgs e)
        {
            switch (Argument.Items[Argument.SelectedIndex])
            {
                case "":
                    Username.ReadOnly = true;
                    break;

                default:
                    Username.ReadOnly = false;
                    break;
            }
        }

        private void GUI_FormClosing(object sender, FormClosingEventArgs e)
        {
            Process proc = Process.GetCurrentProcess();
            proc.Kill();
        }

        // Ниже представлен сектор SCPSL Монитора

        private bool CheckForExisting()
        {
            bool dirEX = Directory.Exists(Directory.GetCurrentDirectory() + Path.DirectorySeparatorChar + "monitor");
            Console.WriteLine("'monitor' directory exists.");
            if (dirEX) return File.Exists(Directory.GetCurrentDirectory() + Path.DirectorySeparatorChar + "monitor" + Path.DirectorySeparatorChar + "shell.exe");
            else
            {
                Console.WriteLine("'shell.exe' Not found!");
                return false;
            }
        }

        private void ExistingManager()
        {
            Console.WriteLine("[Existing Manager Started]");
            bool exist = CheckForExisting();
            if (!exist)
            {
                RetryButton.Enabled = true;
                RetryButton.Visible = true;
                NoMonitor.Visible = true;

                OpenMonitorConfig.Visible = false;
                OpenMonitorConfig.Enabled = false;
                StartMonitor.Enabled = false;
                StartMonitor.Visible = false;
                StopMonitor.Visible = false;
                StopMonitor.Enabled = false;

                Console.WriteLine("Fault!");
            }
            else
            {
                RetryButton.Enabled = false;
                RetryButton.Visible = false;
                NoMonitor.Visible = false;

                OpenMonitorConfig.Visible = true;
                OpenMonitorConfig.Enabled = true;
                StartMonitor.Enabled = true;
                StartMonitor.Visible = true;
                StopMonitor.Visible = true;
                StopMonitor.Enabled = true;
                Console.WriteLine("Success...");
            }
            Console.WriteLine("[Existing Manager Stopped]");
        }

        private void OpenMonitorConfig_Click(object sender, EventArgs e)
        {
            String path = Directory.GetCurrentDirectory() + Path.DirectorySeparatorChar + "monitor" + Path.DirectorySeparatorChar + "config.cfg";
            try
            {
                Process.Start(path);
            }
            catch
            {
                DialogResult res = MessageBox.Show("Конфиг не был найден. Желаете ли Вы создать его автоматически и"
                    + Environment.NewLine + "заполнить его пустыми данными?", "Ошибка", MessageBoxButtons.YesNo, MessageBoxIcon.Error);
                if (res == DialogResult.Yes)
                {
                    try
                    {
                        using (FileStream fs = File.Create(path))
                        {
                            Byte[] info = new UTF8Encoding(true).GetBytes("Working Directory" + Environment.NewLine +
                                "--INSERT PATH TO MODLOGS HERE--" + Environment.NewLine +
                                "JS Bot Adress" + Environment.NewLine + "127.0.0.1");
                            fs.Write(info, 0, info.Length);
                        }
                    }
                    catch
                    {
                        DialogResult fault = MessageBox.Show("Не удалось создать конфиг файл" + Environment.NewLine +
                            "Устраните ошибку или создайте его сами", "Критическая Ошибка",
                            MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
                if (res == DialogResult.No)
                {
                    DialogResult fault = MessageBox.Show("Создайте конфиг файл по пути:" + Environment.NewLine +
                        path, "Отмена",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
        } 

        private void StartMonitor_Click(object sender, EventArgs e)
        {
            StartMonitor.Enabled = false;
            StopMonitor.Enabled = true;
            OpenMonitorConfig.Enabled = false;

            String path = Directory.GetCurrentDirectory() + Path.DirectorySeparatorChar + "monitor" + Path.DirectorySeparatorChar + "shell.exe";

            try
            {
                Process proc = Process.Start(path);
                Server.Monitor = proc;
            }
            catch
            {
                DialogResult fault = MessageBox.Show("Не удалось запусть процесс shell.exe" + Environment.NewLine +
                            "Скорее всего, его не существует на диске" + Environment.NewLine + "Проверьте путь: " + Environment.NewLine +
                            path, "Критическая Ошибка",
                            MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void StopMonitor_Click(object sender, EventArgs e)
        {
            StartMonitor.Enabled = true;
            StopMonitor.Enabled = false;
            OpenMonitorConfig.Enabled = true;

            try
            {
                Server.Monitor.Kill();
                Server.Monitor = new Process();
                Console.WriteLine("Stopped SCPSL Monitor.");
            }
            catch (Exception error)
            {
                Console.WriteLine(error.Message);
            }
        }

        private void RetryButton_Click(object sender, EventArgs e)
        {
            ExistingManager();
        }
    }
}
