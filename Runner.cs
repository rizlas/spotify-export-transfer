using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using Spotify.API.NetCore;
using Spotify.API.NetCore.Auth;
using Spotify.API.NetCore.Enums;
using Spotify.API.NetCore.Models;

namespace SpotifyClone
{
    class Runner
    {
        private static SpotifyWebAPI _spotify;
        private static ManualResetEvent SignalEvent = new ManualResetEvent(false);

        public static SpotifyWebAPI Spotify { get => _spotify; }

        static void Main(string[] args)
        {
            _spotify = new SpotifyWebAPI();
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

            auth.ClientId = "4a170da109934b9ea73d1c1d891d9ac8";
            auth.RedirectUri = "http://localhost:4002";
            auth.Scope = Scope.PlaylistReadCollaborative;

            auth.OnResponseReceivedEvent += Auth_OnResponseReceivedEvent;
            auth.StartHttpServer(4002); // Starts an internal HTTP Server
            auth.DoAuth();
        }

        private static void Auth_OnResponseReceivedEvent(Token token, string state)
        {
            int ExpiresIn = token.ExpiresIn;
            _spotify.AccessToken = token.AccessToken;
            _spotify.TokenType = token.TokenType;
            SignalEvent.Set();
        }
    }
}
