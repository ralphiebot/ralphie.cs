using System;
using TwitchLib;
using TwitchLib.Events.PubSub;
using Ralphie.Twitch.Chat;
using Ralphie.Config;

namespace Ralphie.Twitch.PubSub
{
    class PubSub
    {
        public TwitchChatBot chatbot;
        readonly static string botToken = Configs.GetString("Twitch_BotToken");

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
            pubsub.OnPubSubServiceError += PubSub_OnPubSubServiceError;
            pubsub.OnHost += PubSub_OnHost;

            consoleMessage[5] = "Connecting to PubSub...";
            Program.SendToConsole(consoleMessage);
            pubsub.Connect();
        }

        internal void Disconnect()
        {
            pubsub.Disconnect();
        }


        private void PubSub_OnPubSubServiceError(object sender, OnPubSubServiceErrorArgs e)
        {
            consoleMessage[5] = $"PubSub Service Error: {e.Exception.Message}";
        }

        private void PubSub_OnPubSubConnected(object sender, EventArgs e)
        {
            consoleMessage[5] = "Connected to PubSub service. Subscribing to topics...";
            Program.SendToConsole(consoleMessage);
            pubsub.ListenToWhispers(botToken);
            pubsub.ListenToBitsEvents(botToken);
            pubsub.ListenToSubscriptions(botToken);
        }

        private void PubSub_OnListenResponse(object sender, OnListenResponseArgs e)
        {
            if (e.Successful)
            {
                consoleMessage[3] = "Listening to topic:";
                consoleMessage[5] = e.Topic;
                Program.SendToConsole(consoleMessage);
            }
            else
            {
                consoleMessage[3] = $"Failed to listen to {e.Topic}";
                consoleMessage[5] = e.Response.Error;
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

        private void PubSub_OnHost(object sender, OnHostArgs e)
        {
            chatbot.ManualMessage($"{e.HostedChannel}!!");
        }

        internal void PubSub_OnBitsReceived(object sender, OnBitsReceivedArgs e)
        {
            chatbot.ManualMessage($"Thanks for the {e.BitsUsed} bits, {e.Username}!!");
        }


    }
}