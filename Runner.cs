using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;
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

            Console.WriteLine("Autentication...");
            Auth.Init(ClientId, RedirectUri, RedirectPort);
            while (!Auth.IsAuthenticated) { }

            Console.Clear();
            new SpotifyProgram().Run();
            Console.ReadLine();
        }
    }
}
