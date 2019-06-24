using EasyConsoleCore;
using Spotify.API.NetCore.Models;
using SpotifyClone.Model;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading;

namespace SpotifyClone.Pages
{
    class Import : MenuPage
    {
        private static ManualResetEvent _SignalEvent = new ManualResetEvent(false);
        public Import(Program program) : base("Import", program)
        {
        }

        public override void Display()
        {
            Console.WriteLine($"Main Page > Import{Environment.NewLine}---");

            Output.WriteLine(ConsoleColor.Yellow, $"Current account in use {Auth.Profile.DisplayName}");
            Console.Write("Would you like to use the same account? (Y/n) [Default: Y]");
            string Confirm = Console.ReadLine();

            if (Confirm.Trim() == "n")
            {
                Output.WriteLine(ConsoleColor.Red, $"Before continue clean your cookie and browser session, otherwise the same account will be used..{Environment.NewLine}Hit [Enter] to continue...");
                Console.ReadLine();

                Console.WriteLine("Authentication...");
                Auth.Authentication();
                while (!Auth.IsAuthenticated) { }
                Output.WriteLine(ConsoleColor.Yellow, $"Current account in use {Auth.Profile.DisplayName}");
            }

            Console.Write("Enter path of playlist csv file: ");
            string PathPlaylist = Console.ReadLine();

            if(PathPlaylist.StartsWith('"') && PathPlaylist.EndsWith('"'))
            {
                PathPlaylist = PathPlaylist.Trim('"');
            }

            if (File.Exists(PathPlaylist))
            {
                string PlaylistName = Path.GetFileNameWithoutExtension(PathPlaylist);

                List<CsvPlaylist> Tracks = CsvLoad(PathPlaylist);
                Tracks.OrderBy(t => t.Index);

                Console.WriteLine("Creation of a new playlist...");
                Console.Write($"Would you like to use the same name ({PlaylistName})? (Y/n)");
                Confirm = Console.ReadLine();

                string NewName = string.Empty;

                if (Confirm.Trim() == "n")
                {
                    Console.Write($"Please enter the new name: ");
                    NewName = Console.ReadLine();
                }

                Console.Write($"Would you like to keep the add order of the oldest playlist? [Y (slower) / n (faster)]");
                Confirm = Console.ReadLine();
                bool Order = true;

                if (Confirm.Trim() == "n")
                {
                    Order = false;
                }

                FullPlaylist NewPlaylist = Auth.Spotify.CreatePlaylist(Auth.Profile.Id, NewName == string.Empty ? PlaylistName : NewName, false);
                Console.WriteLine("Import started");
                Console.WriteLine();

                string Format = "0000";

                if (Tracks.Count < 1000)
                    Format = "000";
                else if (Tracks.Count < 100)
                    Format = "00";

                foreach (var Track in Tracks)
                {
                    Console.Write($"\rProcessing: {Track.Index.ToString(Format)}/{Tracks.Count.ToString(Format)}");
                    Auth.Spotify.AddPlaylistTrack(Auth.Profile.Id, NewPlaylist.Id, $"spotify:track:{Track.Id}", Track.Index - 1);

                    if (Order)
                    {
                        Thread.Sleep(1000);
                    }
                }
            }

            Console.WriteLine();
            Output.WriteLine(ConsoleColor.Green, "Import Finished");
            Input.ReadString("Press [Enter] to navigate home");
            Program.NavigateHome();
        }

        private static List<CsvPlaylist> CsvLoad(string Path)
        {
            List<CsvPlaylist> tmp = new List<CsvPlaylist>();

            using (var reader = new StreamReader(Path))
            {
                // Skip header
                reader.ReadLine();

                while (!reader.EndOfStream)
                {
                    string Row = reader.ReadLine();
                    string[] Values = Row.Split(';');

                    CsvPlaylist Track = new CsvPlaylist
                    {
                        Index = int.Parse(Values[0]),
                        Id = Values[1],
                        Artists = Values[2],
                        Name = Values[3],
                        AddedAt = DateTime.ParseExact(Values[4], "dd/MM/yyyy HH:mm:ss", CultureInfo.InvariantCulture)
                    };

                    tmp.Add(Track);
                }
            }

            return tmp;
        }
    }
}
