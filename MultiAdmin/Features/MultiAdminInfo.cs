﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MultiAdmin.MultiAdmin.Commands
{
    class MultiAdminInfo : Feature, IEventServerPreStart, ICommand
    {
        public MultiAdminInfo(Server server) : base(server)
        {
        }

        public override void Init()
        {
        }

        public override void OnConfigReload()
        { 
        }

        public void PrintInfo()
        {
            Server.Write("MultiAdmin для SCP: Secret Laboratory сделан Courtney (Grover_c13), это ПАРЕНЬ.", ConsoleColor.DarkMagenta);
            Server.Write("Сильно модифицированная версия LocalAdmin сделаного Хубертом Мозайкой.", ConsoleColor.DarkMagenta);
            Server.Write("Вы можете запросить код LocalAdmin на moszka.hubert@gmail.com.", ConsoleColor.DarkMagenta);
            Server.Write("Вы можете найти код MultiAdmin на https://github.com/Grover-c13/MultiAdmin/.", ConsoleColor.DarkMagenta);
            Server.Write("Реализованно под Mozzila Public License 2.0", ConsoleColor.DarkMagenta);
            Server.Write("У Вас много бабок? Довольны multiadmin/servermod? Отправьте мне на дошик!", ConsoleColor.Gray);
            Server.Write("Paypal: Grover.c13@gmail.com", ConsoleColor.Gray);
            Server.Write("Переведено и улучшено : ReDestroyDeR");
        }

        public override string GetFeatureDescription()
        {
            return "Выводит информацию о лицензии/авторе";
        }

        public override string GetFeatureName()
        {
            return "Информация о MultiAdmin";
        }


        public void OnServerPreStart()
        {
            PrintInfo();
        }

        public void OnCall(string[] args)
        {
            PrintInfo();
        }

        public string GetCommand()
        {
            return "INFO";
        }

        public bool PassToGame()
        {
            return false;
        }

        public string GetCommandDescription()
        {
            return "Выводит лицензию и информацию об авторе.";
        }


        public string GetUsage()
        {
            return "";
        }
    }
}
