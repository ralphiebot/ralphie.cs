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

        readonly ConnectionCredentials credentials = new ConnectionCredentials(botUserName, botToken);
        TwitchClient client;


        // Call bot to connect
        internal void Connect()
        {
            Program.SendToConsole("Initializing Twitch Chat Bot...");
            client = new TwitchClient(credentials, streamerChannel, logging: false);

            string test = "!";   // Replace this later with logic to read from commands.xml
            char cmd = test[0];

            Program.SendToConsole($"Setting command identifier to {cmd}.");
            client.AddChatCommandIdentifier(cmd);
            client.AddWhisperCommandIdentifier(cmd);

            Program.SendToConsole("Setting Twitch chat throttle set to 19 messages every 30 seconds.");
            client.ChatThrottler = new TwitchLib.Services.MessageThrottler(client, 19, TimeSpan.FromSeconds(30));
            client.WhisperThrottler = new TwitchLib.Services.MessageThrottler(client, 19, TimeSpan.FromSeconds(30));

            Program.SendToConsole("Registering Twitch chat events...");
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

            Program.SendToConsole("Connecting to Twitch chat...");
            client.Connect();


        }


        // Call bot to disconnect
        internal void Disconnect()
        {
            Program.SendToConsole("Disconnecting...");
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
            //Program.SendToConsole(e.Data);
        }
        private void Client_OnIncorrectLogin(object sender, OnIncorrectLoginArgs e)
        {
            Program.SendToConsole("Incorrect Login!");
        }
        private void Client_OnConnecitonError(object sender, OnConnectionErrorArgs e)
        {
            Program.SendToConsole($"Error!! {e.Error}");
        }
        private void Client_OnConnected(object sender, OnConnectedArgs e)
        {
            client.ChatThrottler.StartQueue();
            Program.SendToConsole("Connected.");
            Program.GlobalOptions.IsConnected = true;
        }
        private void Client_OnDisconnected(object sender, OnDisconnectedArgs e)
        {
            Console.WriteLine("\rDisconnected");
        }

        // Message events
        private void Client_OnMessageReceived(object sender, OnMessageReceivedArgs e)
        {
            string message = $"[{DateTime.Now.ToString("MM/dd - HH:mm")} | Twitch] {e.ChatMessage.Username}: {e.ChatMessage.Message}";
            Program.SendToConsole(message);
        }
        private void Client_OnChatCommandReceived(object sender, OnChatCommandReceivedArgs e)
        {
            string message = "Chat command received, you need to do something about it!";
            Program.SendToConsole(message);
        }
        private void Client_OnMessageSent(object sender, OnMessageSentArgs e)
        {
            string message = $"[Twitch:{DateTime.Now.ToShortTimeString()}]{e.SentMessage.Channel}> {e.SentMessage.Message}";
            Program.SendToConsole(message, true);
        }

        // Whisper events
        private void Client_OnWhisperReceived(object sender, OnWhisperReceivedArgs e)
        {
            string message = $"[{DateTime.Now.ToString("MM/dd - HH:mm")} | Twitch] **{e.WhisperMessage.Username}: {e.WhisperMessage.Message}";
            Program.SendToConsole(message);
        }        
        private void Client_OnWhisperCommandReceived(object sender, OnWhisperCommandReceivedArgs e)
        {
            string message = $"[Twitch:{DateTime.Now.ToShortTimeString()} | Twitch] **{e.WhisperMessage.Username}: {e.WhisperMessage.Message}";
            Program.SendToConsole(message);
        }
        private void Client_OnWhisperSent(object sender, OnWhisperSentArgs e)
        {
            string message = $"[Twitch:{DateTime.Now.ToShortTimeString()}]{e.Receiver}> {e.Message}";
            Program.SendToConsole(message, true);
        }

        // User events
        private void Client_OnUserJoined(object sender, OnUserJoinedArgs e)
        {
            // TODO: Add code to start timer for user
            // check for new user, if new, add user to system (ralphie.users ?)
            // mark start time for user
            string message = $"[{DateTime.Now.ToString("MM/dd - HH:mm")} | Twitch] {e.Username} has joined #{e.Channel}";
            Program.SendToConsole(message);
        }
        private void Client_OnUserLeft(object sender, OnUserLeftArgs e)
        {
            // TODO:
            // Stop counting user time for the user
            string message = $"[{DateTime.Now.ToString("MM/dd - HH:mm")} | Twitch] {e.Username} has left #{e.Channel}";
            Program.SendToConsole(message);
        }
        private void Client_OnUserTimedout(object sender, OnUserTimedoutArgs e)
        {
            string message = $"[{DateTime.Now.ToString("MM/dd - HH:mm")} | Twitch] {e.Username} has been timed out.";
            Program.SendToConsole(message);

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