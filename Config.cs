using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;

namespace ralphie.Config
{
    internal class Configs
    {
        public static bool CheckSetup()
        {
            if (GetString("FirstTimeSetup") == "Not Found")
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        internal static void FirstTimeSetup()
        {
            // Gather required settings and write them to file
            Console.Write("Enter bot username: ");
            SetConfig("Twitch_BotName", Console.ReadLine());
            Console.Write("Enter bot oAuth token: ");
            SetConfig("Twitch_BotToken", Console.ReadLine());
            Console.Write("Enter bot ClientID: ");
            SetConfig("Twitch_BotClientId", Console.ReadLine());
            Console.Write("Enter streamer channel: ");
            SetConfig("Twitch_StreamerChannel", Console.ReadLine());
            // This list will grow over time, so will likely want to replace this will something more efficient. Maybe a dictionary list?
            SetConfig("FirstTimeSetup", "Done");
        }

        internal static string GetString(string key, bool isEncrypted = false)
        {
            var appSettings = ConfigurationManager.AppSettings;
            string result = appSettings[key] ?? "Not Found";
            return result;
        }

        internal static bool GetBool(string name)
        {
            var appSettings = ConfigurationManager.AppSettings;
            string setting = appSettings[name] ?? "Not Found";
            if (setting == "true")
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        internal static void SetConfig(string name, string option, bool isEncrypted = false)
        {
            var configFile = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            var settings = configFile.AppSettings.Settings;

            if (isEncrypted == true)
            {
                // TODO: Implement config encryption for storage of sensitive options i.e. bot token/clientid
            }
            else
            {
                if (settings[name] == null)
                {
                    settings.Add(name, option);
                }
                else
                {
                    settings[name].Value = option;
                }
            }

            configFile.Save(ConfigurationSaveMode.Modified);
            ConfigurationManager.RefreshSection(configFile.AppSettings.SectionInformation.Name);

        }
    }
}
