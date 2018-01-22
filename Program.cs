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
                var input = Console.ReadLine();
                if (input.ToLowerInvariant() == "exit")
                {
                    bot.Disconnect();
                    Environment.Exit(0);
                }
                else
                {
                    if (input.StartsWith("/w"))
                    {
                        string[] substrings = input.Split();
                        string receiver = substrings[1];
                        string message = "";
                        int target = substrings.Length;

                        for (int i = 2; i < target; i++)
                        {
                            message += $"{substrings[i]} ";
                        }
                        bot.ManualWhisper(receiver, message);
                    }
                    else
                    {
                        bot.ManualMessage(input);   // Dev testing to send messages to chat, remove when unnecessary
                    }
                }
            } while (GlobalOptions.IsConnected == true);
        }
        public static void ClearCurrentConsoleLine()    // snipped code to clear console line
        {
            int currentLineCursor = Console.CursorTop;
            Console.SetCursorPosition(0, Console.CursorTop);
            Console.Write(new string (' ', Console.WindowWidth));
            Console.SetCursorPosition(0, currentLineCursor);
        }
        public static void SendToConsole(string[] args, bool overwrite=false) // It just makes sense to do it here, rathe than repeat myself, only to have to remove it all later
        {
            //  move cursor to replace line if overwrite is true
            if (overwrite == true)
            {
                Console.SetCursorPosition(0, Console.CursorTop - 1);
                ClearCurrentConsoleLine();
            }

            var foregroundColor = ConsoleColor.White;   // Set to default color, just in case.  Also makes less code writing later in the switches.

            // write timestamp first
            Console.ForegroundColor = ConsoleColor.DarkBlue;
            Console.Write($"\r[");
            Console.ForegroundColor = ConsoleColor.DarkYellow;
            Console.Write($"{DateTime.Now.ToString("MM/dd - HH:mm")} ");
            Console.ForegroundColor = ConsoleColor.DarkBlue;
            Console.Write($"|| ");

            // Check location and color appropriately
            switch (args[0])
            {
                case "Twitch":
                    foregroundColor = ConsoleColor.DarkMagenta;
                    break;
                case "Discord":
                    foregroundColor = ConsoleColor.DarkCyan;
                    break;
                default:
                    foregroundColor = ConsoleColor.White;   // Default color should always be white
                    break;
            };
            Console.ForegroundColor = foregroundColor;
            Console.Write($"{args[0]}.{args[1]}");   // Write location 
            
            // We need to append the channel name to the location for some messages
            switch (args[1])
            {
                case "#":
                    Console.Write($"{args[4]}");
                    break;
                default:
                    break;
            }

            Console.Write("] ");
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.Write($"{args[2]} ");   // write direction

            switch (args[2])    // Check direciton of message
            {
                case ">>>":
                    foregroundColor = ConsoleColor.Green;
                    break;
                case "<<<":
                    foregroundColor = ConsoleColor.Cyan;
                    break;
                default:
                    foregroundColor = ConsoleColor.White;
                    break;
            }

            // Write event sender username
            Console.Write($"{args[3]}: ");

            // Reset to white and writeline message, then append prompt
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine(args[5]);
            Console.Write(">");
        }
        public static class GlobalOptions   // Is there a better way to create globally accessible variables??
        {
            public static bool IsLogging = false;
            public static bool IsConnected = false;
        }
    }
}