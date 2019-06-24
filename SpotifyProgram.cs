using System;
using System.Collections.Generic;
using System.Text;
using EasyConsoleCore;
using SpotifyClone.Pages;

namespace SpotifyClone
{
    class SpotifyProgram : Program
    {
        public SpotifyProgram() : base("EasyConsole Spotify", breadcrumbHeader: true)
        {
            AddPage(new Main(this));
            AddPage(new Export(this));
            AddPage(new Import(this));

            SetPage<Main>();
        }
    }
}
