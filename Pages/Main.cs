using EasyConsoleCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace SpotifyClone.Pages
{
    class Main : MenuPage
    {
        public Main(Program program) : 
            base("Main Page", program,
                  new Option("Export", () => program.NavigateTo<Export>()),
                  new Option("Clone exported to another account", () => program.NavigateTo<Clone>()),
                  new Option("Exit", () => Environment.Exit(0))
                )
        {
        }
    }
}