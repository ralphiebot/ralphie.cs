using System;
using ralphie.twitch.chat;
using ralphie.twitch.api;
using ralphie.Config;

namespace ralphie
{
    class Program
    {
        static void Main(string[] args)
        {
            if (Configs.CheckSetup() == false)
            {
                Configs.FirstTimeSetup();
            }

            TwitchChatBot bot = new TwitchChatBot();
            bot.Connect();

            do
            {
                Console.Write(">");
                var input = Console.ReadLine();
                if (input.ToLowerInvariant() == "exit")
                {
                    bot.Disconnect();
                    Environment.Exit(0);
                }
                else
                {
                    bot.ManualMessage(input);   // Dev testing to send messages to chat, remove when unnecessary
                }
            } while (GlobalOptions.IsConnected == true);
        }
        public static class GlobalOptions   // Is there a better way to create globally accessible variables??
        {
            public static bool IsLogging = false;
            public static bool IsConnected = false;
        }
    }
}