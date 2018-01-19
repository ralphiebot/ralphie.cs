﻿using System;
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
        public static void SendToConsole(string consoleMessage, bool overwrite=false) // It just makes sense to do it here, rathe than repeat myself, only to have to remove it all later
        {
            if (overwrite == true)
            {
                Console.SetCursorPosition(0, Console.CursorTop - 1);
                ClearCurrentConsoleLine();
            }
            Console.WriteLine($"\r{consoleMessage}");
            Console.Write(">");
        }
    public static class GlobalOptions   // Is there a better way to create globally accessible variables??
        {
            public static bool IsLogging = false;
            public static bool IsConnected = false;
        }
    }
}