﻿using EasyConsoleCore;
using Spotify.API.NetCore.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace SpotifyClone.Pages
{
    class Export : MenuPage
    {
        Paging<SimplePlaylist> Playlists;

        public Export(Program program) : base("Export", program)
        {
            GetPlaylists();
        }

        private void GetPlaylists()
        {
            Playlists = Auth.Spotify.GetUserPlaylists(Auth.Profile.Id);
        }

        public override void Display()
        {
            Console.WriteLine($"Main Page > Export{Environment.NewLine}---");
            
            if(Playlists.Items.Count == 0)
            {
                Output.WriteLine(ConsoleColor.Red, "0 playlists found!");
            }

            Console.WriteLine("0. Go back");

            for (int i = 0; i < Playlists.Items.Count; i++)
            {
                Console.WriteLine($"{i + 1}. {Playlists.Items[i].Name} Owner: {Playlists.Items[i].Owner.DisplayName}");
            }

            int Choice = Input.ReadInt("Choose an option: ", min: 0, max: Playlists.Items.Count);

            if (Choice != 0)
            {
                Output.WriteLine(ConsoleColor.Green, "You selected {0}", Playlists.Items[Choice - 1].Name);
                Output.WriteLine(ConsoleColor.Red, "Export started");

                ExportPlaylist(Choice - 1);
                Output.WriteLine(ConsoleColor.Green, $"Export complete, saved in {System.IO.Directory.GetCurrentDirectory()}");
                Input.ReadString("Press [Enter] to navigate home");
                Program.NavigateBack();
            }
            else
            {
                Program.NavigateBack();
            }
        }

        private void ExportPlaylist(int Index)
        {
            int Offset = 0;
            Paging<PlaylistTrack> PlaylistTracks = Auth.Spotify.GetPlaylistTracks(Auth.Profile.Id, Playlists.Items[Index].Id, offset: Offset);
            int Total = PlaylistTracks.Total;
            Offset += PlaylistTracks.Items.Count;

            while (Offset < Total)
            {
                Paging<PlaylistTrack> TmpTracks = Auth.Spotify.GetPlaylistTracks(Auth.Profile.Id, Playlists.Items[Index].Id, offset: Offset);
                PlaylistTracks.Items.AddRange(TmpTracks.Items);
                Offset += TmpTracks.Items.Count;
            }

            // Export CSV TrackId,Artist,TrackName,AddedAt
            StringBuilder Sb = new StringBuilder();
            Sb.AppendLine("Number;Id;Artists;Name;AddedAt");

            for (int i = 0; i < PlaylistTracks.Items.Count; i++)
            {
                FullTrack Track = PlaylistTracks.Items[i].Track;
                Sb.AppendLine($"{(i + 1).ToString("000")};{Track.Id};\"{Track.Artists[0].Name}\";\"{Track.Name}\";{PlaylistTracks.Items[i].AddedAt}");
            }

            System.IO.File.WriteAllText($"{string.Join("", Playlists.Items[Index].Name.Split(System.IO.Path.GetInvalidFileNameChars()))}.csv", Sb.ToString());
        }
    }
}
