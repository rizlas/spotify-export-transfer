using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;
using EasyConsoleCore;
using Microsoft.Extensions.Configuration;
using Spotify.API.NetCore;
using Spotify.API.NetCore.Auth;
using Spotify.API.NetCore.Enums;
using Spotify.API.NetCore.Models;

namespace SpotifyClone
{
    class Runner
    {
        private static string ClientId;
        private static string RedirectUri;
        private static int RedirectPort;

        static void Main(string[] args)
        {
            var builder = new ConfigurationBuilder().SetBasePath(Directory.GetCurrentDirectory()).AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);
            IConfigurationRoot configuration = builder.Build();

            ClientId = configuration.GetSection("Settings").GetSection("ClientId").Value;
            RedirectPort = int.Parse(configuration.GetSection("Settings").GetSection("RedirectPort").Value);
            RedirectUri = $"{configuration.GetSection("Settings").GetSection("RedirectUri").Value}:{RedirectPort}";

            string Title = @"
   _____                   _                _____   _                        
  / ____|                 | |              / ____| | |                       
 | (___    _ __     ___   | |_   _   _    | |      | |   ___    _ __     ___ 
  \___ \  | '_ \   / _ \  | __| | | | |   | |      | |  / _ \  | '_ \   / _ \
  ____) | | |_) | | (_) | | |_  | |_| |   | |____  | | | (_) | | | | | |  __/
 |_____/  | .__/   \___/   \__|  \__, |    \_____| |_|  \___/  |_| |_|  \___|
          | |                     __/ |                                      
          |_|                    |___/                                       

##############################################################################";

            Console.WriteLine(Title);
            Output.WriteLine(ConsoleColor.Red, $"Useful info:{Environment.NewLine}- Read carefully each single console output{Environment.NewLine}- Every different input will treat as default value{Environment.NewLine}- Repository of this app is available on GitHub, user: rizlas{Environment.NewLine}{Environment.NewLine}Press [Enter] to continue...{Environment.NewLine}");
            
            Console.ReadLine();
            Console.WriteLine("Autentication...");
            Auth.Init(ClientId, RedirectUri, RedirectPort);
            while (!Auth.IsAuthenticated) { }

            Console.Clear();
            new SpotifyProgram().Run();
            Console.ReadLine();
        }
    }
}
