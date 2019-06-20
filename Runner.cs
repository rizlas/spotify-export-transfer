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
        private static SpotifyWebAPI _Spotify;
        public static SpotifyWebAPI Spotify { get => _Spotify; }
        public static bool Header { get => _Header; }

        private static ManualResetEvent SignalEvent = new ManualResetEvent(false);
        private static string ClientId;
        private static string RedirectUri;
        private static int RedirectPort;
        private static bool _Header;

        static void Main(string[] args)
        {
            var builder = new ConfigurationBuilder().SetBasePath(Directory.GetCurrentDirectory()).AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);
            IConfigurationRoot configuration = builder.Build();

            ClientId = configuration.GetSection("Settings").GetSection("ClientId").Value;
            RedirectUri = configuration.GetSection("Settings").GetSection("RedirectUri").Value;
            RedirectPort = int.Parse(configuration.GetSection("Settings").GetSection("RedirectPort").Value);
            RedirectUri += $":{RedirectPort}";
            _Header = bool.Parse(configuration.GetSection("Settings").GetSection("CsvHeader").Value);

            _Spotify = new SpotifyWebAPI();
            Console.WriteLine("Autentication...");
            Thread AuthThread = new Thread(Authentication);
            AuthThread.Start();

            SignalEvent.WaitOne(); //This thread will block here until the reset event is sent.
            SignalEvent.Reset();

            Console.Clear();
            new SpotifyProgram().Run();
            Console.ReadLine();
        }

        private static void Authentication(object obj)
        {
            ImplicitGrantAuth auth = new ImplicitGrantAuth();

            auth.ClientId = ClientId;
            auth.RedirectUri = RedirectUri;
            auth.Scope = Scope.PlaylistReadCollaborative;

            auth.OnResponseReceivedEvent += Auth_OnResponseReceivedEvent;
            auth.StartHttpServer(RedirectPort); // Starts an internal HTTP Server
            auth.DoAuth();
        }

        private static void Auth_OnResponseReceivedEvent(Token token, string state)
        {
            int ExpiresIn = token.ExpiresIn;
            _Spotify.AccessToken = token.AccessToken;
            _Spotify.TokenType = token.TokenType;
            SignalEvent.Set();
        }
    }
}
