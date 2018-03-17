using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MultiAdmin.MultiAdmin.Commands
{
    class EventTest : Feature, IEventCrash, IEventMatchStart, IEventPlayerConnect, IEventPlayerDisconnect, IEventRoundEnd, IEventRoundStart, IEventServerFull, IEventServerPreStart, IEventServerStart, IEventServerStop
    {

        public EventTest(Server server) : base(server)
        {
        }

        public override void Init()
        {
        }

        public override void OnConfigReload()
        {
        }

        public override string GetFeatureDescription()
        {
            return "Тестирует Event'ы";
        }

        public override string GetFeatureName()
        {
            return "Тест";
        }

        public void OnServerFull()
        {
            Server.Write("EVENTTEST Сервер полон");
        }

        public void OnCrash()
        {
            Server.Write("EVENTTEST Краш");
        }

        public void OnMatchStart()
        {
            Server.Write("EVENTTEST Начало Матча");
        }

        public void OnPlayerConnect(string name)
        {
            Server.Write("EVENTTEST игрок подключился " + name);
        }

        public void OnPlayerDisconnect(string name)
        {
            Server.Write("EVENTTEST игрок отключился " + name);
        }

        public void OnRoundEnd()
        {
            Server.Write("EVENTTEST на конце раунда");
        }

        public void OnRoundStart()
        {
            Server.Write("EVENTTEST на начале раунда");
        }

        public void OnServerPreStart()
        {
            Server.Write("EVENTTEST на пред-старте");
        }

        public void OnServerStart()
        {
            Server.Write("EVENTTEST на старте");
        }

        public void OnServerStop()
        {
            Server.Write("EVENTTEST на остановке");
        }
    }
}
