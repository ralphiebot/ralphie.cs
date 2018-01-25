using System;
using TwitchLib;
using TwitchLib.Events.PubSub;
using Ralphie.Twitch.Chat;

namespace Ralphie.Twitch.PubSub
{
    class PubSub
    {
        public TwitchChatBot chatbot;
        public PubSub(ref TwitchChatBot bot)
        {
            chatbot = bot;
        }

        public TwitchPubSub pubsub;
        string[] consoleMessage = {"Twitch","PubSub",">>>","","",""};


        internal void Connect()
        {        
            consoleMessage[5] = "Initializing PubSub...";            
            Program.SendToConsole(consoleMessage);
            pubsub = new TwitchPubSub();


            consoleMessage[5] = "Registering PubSub events...";
            Program.SendToConsole(consoleMessage);
            pubsub.OnPubSubServiceConnected += PubSub_OnPubSubConnected;
            pubsub.OnListenResponse += PubSub_OnListenResponse;
            pubsub.OnBitsReceived += PubSub_OnBitsReceived;
            pubsub.OnChannelSubscription += PubSub_OnChannelSubscription;

            consoleMessage[5] = "Connecting to PubSub...";
            Program.SendToConsole(consoleMessage);
            pubsub.Connect();
            pubsub.ListenToWhispers("123456");
        }


        private void PubSub_OnPubSubConnected(object sender, EventArgs e)
        {
            consoleMessage[5] = "Subscribing to PubSub topics...";
            Program.SendToConsole(consoleMessage);
            pubsub.ListenToWhispers("12345678");
        }

        private void PubSub_OnListenResponse(object sender, OnListenResponseArgs e)
        {
            if (e.Successful)
            {
                consoleMessage[3] = "Listening to topic:";
                consoleMessage[5] = e.Topic;
                Program.SendToConsole(consoleMessage);
            }
        }

        private void PubSub_OnChannelSubscription(object sender, OnChannelSubscriptionArgs e)
        {
            if(e.Subscription.Months > 1)
            {
                chatbot.ManualMessage($"Thanks for {e.Subscription.Months} months of subscription {e.Subscription.Username}!!");
            }
            else
            {
                chatbot.ManualMessage($"Thanks for the subscription {e.Subscription.Username}!");
            }
        }

        internal void PubSub_OnBitsReceived(object sender, OnBitsReceivedArgs e)
        {
            chatbot.ManualMessage($"Thanks for the {e.BitsUsed} bits, {e.Username}!!");
        }


    }
}