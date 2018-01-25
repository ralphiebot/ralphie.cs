// Twitch Chat Functionality using TwitchLib

using System;
using TwitchLib;
using TwitchLib.Models.Client;
using TwitchLib.Events.Client;
using Ralphie.Config;
using Ralphie.Commands;

namespace Ralphie.Twitch.Chat
{
    internal class TwitchChatBot
    {
        readonly static string botUserName = Configs.GetString("Twitch_BotName");
        readonly static string botToken = Configs.GetString("Twitch_BotToken");
        readonly static string streamerChannel = Configs.GetString("Twitch_StreamerChannel");
        string[] consoleMessage = {"Twitch","Chat",">>>","","","" };

        readonly ConnectionCredentials credentials = new ConnectionCredentials(botUserName, botToken);
        TwitchClient client;


        // Call bot to connect
        internal void Connect()
        {
            consoleMessage[5] = "Initializing Twitch Chat Bot...";
            Program.SendToConsole(consoleMessage);
            client = new TwitchClient(credentials, streamerChannel, logging: false);            

            consoleMessage[5] = $"Setting command identifier to {Commands.Commands.CommandPrefix}.";
            Program.SendToConsole(consoleMessage);
            client.AddChatCommandIdentifier(Commands.Commands.CommandPrefix);
            client.AddWhisperCommandIdentifier(Commands.Commands.CommandPrefix);

            // set message throttling
            consoleMessage[5] = "Setting Twitch chat throttle set to 19 messages every 30 seconds.";
            Program.SendToConsole(consoleMessage);
            client.ChatThrottler = new TwitchLib.Services.MessageThrottler(client, 19, TimeSpan.FromSeconds(30));
            client.WhisperThrottler = new TwitchLib.Services.MessageThrottler(client, 19, TimeSpan.FromSeconds(30));

            consoleMessage[5] = "Registering Twitch chat events...";
            Program.SendToConsole(consoleMessage);

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

            consoleMessage[5] = "Connecting to Twitch chat...";
            Program.SendToConsole(consoleMessage);
            client.Connect();
        }

        // Call bot to disconnect
        internal void Disconnect()
        {
            consoleMessage[5] = "Disconnecting";
            Program.SendToConsole(consoleMessage);
            
            client.Disconnect();
        }
        // Send message as bot from console (dev only - remove when unnecessary for testing)
        internal void ManualMessage(string message)
        {
            client.SendMessage(message);
        }

        // Send whisper as bot from console (dev only - remove when unnecessary for testing)
        internal void ManualWhisper(string receiver, string message)
        {
            client.SendWhisper(receiver, message);
        }

        // Connection events
        private void Client_OnLog(object sender, OnLogArgs e)
        {
            //Program.SendToConsole(e.Data);
        }
        private void Client_OnIncorrectLogin(object sender, OnIncorrectLoginArgs e)
        {
            consoleMessage[3] = "Incorrect Login!";
            consoleMessage[5] = e.Exception.Message;
            Program.SendToConsole(consoleMessage);
        }
        private void Client_OnConnecitonError(object sender, OnConnectionErrorArgs e)
        {
            consoleMessage[3] = "Connection Error!";
            consoleMessage[5] = e.Error.Message;
            Program.SendToConsole(consoleMessage);

            //Program.SendToConsole($"Error!! {e.Error}");
        }
        private void Client_OnConnected(object sender, OnConnectedArgs e)
        {
            // Start up chatt throttlers
            client.ChatThrottler.StartQueue();
            client.WhisperThrottler.StartQueue();
            
            consoleMessage[5] = "Connected";
            Program.SendToConsole(consoleMessage);
            
            Program.GlobalOptions.IsConnected = true;
        }
        private void Client_OnDisconnected(object sender, OnDisconnectedArgs e)
        {
            consoleMessage[5] = "Disconnected";
            Program.SendToConsole(consoleMessage);
        }

        // Message events
        private void Client_OnMessageReceived(object sender, OnMessageReceivedArgs e)
        {
            consoleMessage[1] = "#";
            consoleMessage[3] = e.ChatMessage.Username;
            consoleMessage[4] = e.ChatMessage.Channel;
            consoleMessage[5] = e.ChatMessage.Message;
            
            //string message2 = $"[{DateTime.Now.ToString("MM/dd - HH:mm")} | Twitch.#{e.ChatMessage.Channel}>] {e.ChatMessage.Username}: {e.ChatMessage.Message}";
            Program.SendToConsole(consoleMessage);
        }
        private void Client_OnChatCommandReceived(object sender, OnChatCommandReceivedArgs e)
        {
            consoleMessage[1] = "#";
            consoleMessage[3] = e.Command.ChatMessage.Username;
            consoleMessage[4] = e.Command.ChatMessage.Channel;
            consoleMessage[5] = e.Command.ChatMessage.Message;
            //string message = "Chat command received, you need to do something about it!";
            Program.SendToConsole(consoleMessage);
        }
        private void Client_OnMessageSent(object sender, OnMessageSentArgs e)
        {
            consoleMessage[1] = "#";
            consoleMessage[3] = e.SentMessage.DisplayName;
            consoleMessage[4] = e.SentMessage.Channel;
            consoleMessage[5] = e.SentMessage.Message;

            //string message = $"[{DateTime.Now.ToString("MM/dd - HH:mm")} | Twitch.#{e.SentMessage.Channel}<<] {e.SentMessage.DisplayName}: {e.SentMessage.Message}";
            Program.SendToConsole(consoleMessage, true);
        }

        // Whisper events
        private void Client_OnWhisperReceived(object sender, OnWhisperReceivedArgs e)
        {
            consoleMessage[1] = "**";
            consoleMessage[3] = e.WhisperMessage.Username;
            consoleMessage[4] = e.WhisperMessage.BotUsername;
            consoleMessage[5] = e.WhisperMessage.Message;
            //string message = $"[{DateTime.Now.ToString("MM/dd - HH:mm")} | Twitch.W**>>] {e.WhisperMessage.Username}: {e.WhisperMessage.Message}";
            Program.SendToConsole(consoleMessage);
        }        
        private void Client_OnWhisperCommandReceived(object sender, OnWhisperCommandReceivedArgs e)
        {
            consoleMessage[1] = "**";
            consoleMessage[3] = e.WhisperMessage.Username;
            consoleMessage[4] = e.WhisperMessage.BotUsername;
            consoleMessage[5] = e.WhisperMessage.Message;
            //string message = $"[Twitch:{DateTime.Now.ToShortTimeString()} | Twitch.!W**>>] {e.WhisperMessage.Username}: {e.WhisperMessage.Message}";
            Program.SendToConsole(consoleMessage);
        }
        private void Client_OnWhisperSent(object sender, OnWhisperSentArgs e)
        {
            consoleMessage[1] = "**";
            consoleMessage[2] = "<<<";
            consoleMessage[3] = e.Username;
            consoleMessage[4] = e.Receiver;
            consoleMessage[5] = e.Message;
            //string message = $"[{DateTime.Now.ToString("MM/dd - HH:mm")} | Twitch.W**<<] {botUserName}>{e.Receiver}: {e.Message}";
            Program.SendToConsole(consoleMessage, true);
        }

        // User events
        private void Client_OnUserJoined(object sender, OnUserJoinedArgs e)
        {
            consoleMessage[1] = "#";
            consoleMessage[3] = e.Username;
            consoleMessage[4] = e.Channel;
            consoleMessage[5] = "Joined Channel";
            // TODO: Add code to start timer for user
            // check for new user, if new, add user to system (ralphie.users ?)
            // mark start time for user
            //string message = $"[{DateTime.Now.ToString("MM/dd - HH:mm")} | Twitch] {e.Username} has joined #{e.Channel}";
            Program.SendToConsole(consoleMessage);
        }
        private void Client_OnUserLeft(object sender, OnUserLeftArgs e)
        {
            consoleMessage[1] = "#";
            consoleMessage[3] = e.Username;
            consoleMessage[4] = e.Channel;
            consoleMessage[5] = "Left Channel";
            // TODO:
            // Stop counting user time for the user
            //string message = $"[{DateTime.Now.ToString("MM/dd - HH:mm")} | Twitch] {e.Username} has left #{e.Channel}";
            Program.SendToConsole(consoleMessage);
        }
        private void Client_OnUserTimedout(object sender, OnUserTimedoutArgs e)
        {
            consoleMessage[1] = "#";
            consoleMessage[3] = e.Username;
            consoleMessage[4] = e.Channel;
            consoleMessage[5] = "Timed Out";
            //string message = $"[{DateTime.Now.ToString("MM/dd - HH:mm")} | Twitch] {e.Username} has been timed out.";
            Program.SendToConsole(consoleMessage);

            client.SendMessage($"Get rekt {e.Username}");
        }

        // Channel events
        private void Client_OnJoinedChannel(object sender, OnJoinedChannelArgs e)
        {
            client.SendMessage($"Hello I am {botUserName}, powered by ralphiebot");
        }
        private void Client_OnLeftChannel(object sender, OnLeftChannelArgs e)
        {
            consoleMessage[1] = "#";
            consoleMessage[3] = e.BotUsername;
            consoleMessage[4] = e.Channel;
            consoleMessage[5] = "Left Channel";
            Program.SendToConsole(consoleMessage);
        }
    }
}