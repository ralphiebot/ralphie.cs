// Twitch Chat Functionality using TwitchLib

using System;
using TwitchLib;
using TwitchLib.Models.Client;
using TwitchLib.Events.Client;
using ralphie.Config;

namespace ralphie.twitch.chat
{
    internal class TwitchChatBot
    {
        static string botUserName = Configs.GetString("Twitch_BotName");
        static string botToken = Configs.GetString("Twitch_BotToken");
        static string streamerChannel = Configs.GetString("Twitch_StreamerChannel");
        static bool isConnected = false;

        readonly ConnectionCredentials credentials = new ConnectionCredentials(botUserName, botToken);
        TwitchClient client;


        // Call bot to connect
        internal void Connect()
        {
            Console.WriteLine("\rInitializing Twitch Chat Bot...");
            client = new TwitchClient(credentials, streamerChannel, logging: false);

            string test = "!";   // Replace this later with logic to read from commands.xml
            char cmd = test[0];

            Console.WriteLine($"\rSetting command identifier to {cmd}.");
            client.AddChatCommandIdentifier(cmd);
            client.AddWhisperCommandIdentifier(cmd);

            Console.WriteLine("\rSetting Twitch chat throttle set to 19 messages every 30 seconds.");
            client.ChatThrottler = new TwitchLib.Services.MessageThrottler(client, 19, TimeSpan.FromSeconds(30));
            client.WhisperThrottler = new TwitchLib.Services.MessageThrottler(client, 19, TimeSpan.FromSeconds(30));

            Console.WriteLine("\rRegistering Twitch chat events...");
            // Connection events
            client.OnLog += Client_OnLog;
            client.OnIncorrectLogin += Client_OnIncorrectLogin;
            client.OnConnectionError += Client_OnConnecitonError;
            client.OnConnected += Client_OnConnected;
            client.OnDisconnected += Client_OnDisconnected;

            // Message events
            client.OnMessageReceived += Client_OnMessageReceived;
            client.OnChatCommandReceived += Client_OnChatCommandReceived;
            client.OnMessageSent += Client_OnMessageSent;

            // Whisper events
            client.OnWhisperReceived += Client_OnWhisperReceived;
            client.OnWhisperCommandReceived += Client_OnWhisperCommandReceived;
            client.OnWhisperSent += Client_OnWhisperSent;

            // User events
            client.OnUserJoined += Client_OnUserJoined;
            client.OnUserLeft += Client_OnUserLeft;
            client.OnUserTimedout += Client_OnUserTimedout;

            // Channel events
            client.OnJoinedChannel += Client_OnJoinedChannel;
            client.OnLeftChannel += Client_OnLeftChannel;

            Console.WriteLine("\rConnecting to Twitch chat...");
            client.Connect();


        }


        // Call bot to disconnect
        internal void Disconnect()
        {
            Console.WriteLine("Disconnecting...");
            client.Disconnect();
        }
        // Send message as bot from console (dev only - remove when unnecessary for testing)
        internal void ManualMessage(string message)
        {
            client.SendMessage(message, false);
        }

        // Connection events
        private void Client_OnLog(object sender, OnLogArgs e)
        {
            //Console.WriteLine(e.Data);
        }
        private void Client_OnIncorrectLogin(object sender, OnIncorrectLoginArgs e)
        {
            Console.WriteLine("\rIncorrect Login!");
        }
        private void Client_OnConnecitonError(object sender, OnConnectionErrorArgs e)
        {
            Console.WriteLine($"Error!! {e.Error}");
        }
        private void Client_OnConnected(object sender, OnConnectedArgs e)
        {
            client.ChatThrottler.StartQueue();
            Console.WriteLine("Connected.");
            Program.GlobalOptions.IsConnected = true;
        }
        private void Client_OnDisconnected(object sender, OnDisconnectedArgs e)
        {
            Console.WriteLine("\rDisconnected");
        }

        // Message events
        private void Client_OnMessageReceived(object sender, OnMessageReceivedArgs e)
        {
            client.SendMessage(e.ChatMessage.Message);
            Console.WriteLine($"Twitch:[{DateTime.Now.ToString("MM/dd - HH:mm")}]{e.ChatMessage.Username}: {e.ChatMessage.Message}");
        }
        private void Client_OnChatCommandReceived(object sender, OnChatCommandReceivedArgs e)
        {
            Console.WriteLine("\rChat command received, you need to do something about it!");
            Console.Write(">");
        }
        private void Client_OnMessageSent(object sender, OnMessageSentArgs e)
        {
            //Console.SetCursorPosition(0, Console.CursorTop - 1);
            //Console.WriteLine($"[Twitch:{DateTime.Now.ToShortTimeString()}]> {e.SentMessage.Message}");
        }

        // Whisper events
        private void Client_OnWhisperReceived(object sender, OnWhisperReceivedArgs e)
        {
            Console.WriteLine($"Twitch Whisper:[{DateTime.Now.ToString("MM/dd - HH:mm")}]{e.WhisperMessage.Username}: {e.WhisperMessage.Message}");
            client.SendWhisper(e.WhisperMessage.Username, $"You said: {e.WhisperMessage.Message}");
        }        
        private void Client_OnWhisperCommandReceived(object sender, OnWhisperCommandReceivedArgs e)
        {
            throw new NotImplementedException();
        }
        private void Client_OnWhisperSent(object sender, OnWhisperSentArgs e)
        {
            throw new NotImplementedException();
        }

        // User events
        private void Client_OnUserJoined(object sender, OnUserJoinedArgs e)
        {
            // TODO: Add code to start timer for user
            // check for new user, if new, add user to system (ralphie.users ?)
            // mark start time for user
            client.SendMessage($"{e.Username} Just joined.");
        }
        private void Client_OnUserLeft(object sender, OnUserLeftArgs e)
        {
            // TODO:
            // Stop counting user time for the user
            client.SendMessage($"{e.Username} Just left.");
        }
        private void Client_OnUserTimedout(object sender, OnUserTimedoutArgs e)
        {
            client.SendMessage($"Get rekt {e.Username}");
        }

        // Channel events
        private void Client_OnJoinedChannel(object sender, OnJoinedChannelArgs e)
        {
            Console.WriteLine($"\rAttempting to send message to {e.Channel}.");
            client.SendMessage("Hello",false);
            Console.WriteLine("\rMessage Sent?");
        }
        private void Client_OnLeftChannel(object sender, OnLeftChannelArgs e)
        {
            throw new NotImplementedException();
        }
    }
}