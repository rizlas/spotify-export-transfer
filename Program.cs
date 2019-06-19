using System;
using System.Collections.Generic;
using System.Text;
using Spotify.API.NetCore;
using Spotify.API.NetCore.Auth;
using Spotify.API.NetCore.Enums;
using Spotify.API.NetCore.Models;

namespace SpotifyClone
{
    class Program
    {
        private static SpotifyWebAPI _spotify;
        static void Main(string[] args)
        {
            ImplicitGrantAuth auth = new ImplicitGrantAuth();

            auth.ClientId = "4a170da109934b9ea73d1c1d891d9ac8";
            auth.RedirectUri = "http://localhost:4002";
            auth.Scope = Scope.PlaylistReadCollaborative;

            auth.OnResponseReceivedEvent += Auth_OnResponseReceivedEvent;
            //auth.OnResponseReceivedEvent += async (sender, payload) =>
            //{
            //    auth.StopHttpServer(); // `sender` is also the auth instance

            //    // Do requests with API client
            //};
            auth.StartHttpServer(4002); // Starts an internal HTTP Server
            auth.DoAuth();

            Console.ReadLine();
        }

        private static void Auth_OnResponseReceivedEvent(Token token, string state)
        {
            int c = token.ExpiresIn;
            _spotify = new SpotifyWebAPI();
            _spotify.AccessToken = token.AccessToken;
            _spotify.TokenType = token.TokenType;

            PrivateProfile pp = _spotify.GetPrivateProfile();

            Paging<SimplePlaylist> list = _spotify.GetUserPlaylists(pp.Id);

            int Offset = 0;
            Paging<PlaylistTrack> PlaylistTracks = _spotify.GetPlaylistTracks(pp.Id, list.Items[2].Id, offset: Offset);
            int Total = PlaylistTracks.Total;
            Offset += PlaylistTracks.Items.Count;

            while (Offset < Total)
            {
                Paging<PlaylistTrack> TmpTracks = _spotify.GetPlaylistTracks(pp.Id, list.Items[2].Id, offset: Offset);
                PlaylistTracks.Items.AddRange(TmpTracks.Items);
                Offset += TmpTracks.Items.Count;
            }

            // Export CSV TrackId,Artist,TrackName,AddedAt

            StringBuilder Sb = new StringBuilder();

            for (int i = 0; i < PlaylistTracks.Items.Count; i++)
            {
                FullTrack Track = PlaylistTracks.Items[i].Track;
                Sb.AppendLine($"{(i + 1).ToString("000")};{Track.Id};{Track.Artists[0].Name};{Track.Name};{PlaylistTracks.Items[i].AddedAt}");
            }
            
            System.IO.File.WriteAllText("Ciccio.txt", Sb.ToString());
        }
    }
}
