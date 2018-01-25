using System;
using TwitchLib;
using TwitchLib.Events.PubSub;

namespace Ralphie.Twitch.PubSub
{
    class PubSub
    {
        public TwitchPubSub pubsub;
        string[] consoleMessage = {"Twitch","PubSub",">>>","","",""};

        internal void Connect()
        {        
            consoleMessage[5] = "Initializing PubSub...";            
            Program.SendToConsole(consoleMessage);
            pubsub = new TwitchPubSub();


            consoleMessage[5] = "Registering PubSub events...";
            Program.SendToConsole(consoleMessage);
            pubsub.OnPubSubServiceConnected += Twitch_OnPubSubConnected;
            pubsub.OnListenResponse += Twitch_OnListenResponse;
            pubsub.OnBitsReceived += Twitch_OnBitsReceived;

            consoleMessage[5] = "Connecting to PubSub...";
            Program.SendToConsole(consoleMessage);
            pubsub.Connect();
            pubsub.ListenToWhispers("123456");
        }

        private void Twitch_OnPubSubConnected(object sender, EventArgs e)
        {
            consoleMessage[5] = "Subscribing to PubSub events...";
            Program.SendToConsole(consoleMessage);
            pubsub.ListenToWhispers("12345678");
        }

        private void Twitch_OnListenResponse(object sender, OnListenResponseArgs e)
        {
            if (e.Successful)
            {
                consoleMessage[5] = $"Listening to topic: {e.Topic}";
                Program.SendToConsole(consoleMessage);
            }
        }

        internal void Twitch_OnBitsReceived(object sender, OnBitsReceivedArgs e)
        {
            throw new NotImplementedException();
        }


    }
}