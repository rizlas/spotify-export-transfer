using System;
using System.Collections.Generic;
using System.Text;

namespace SpotifyClone.Model
{
    class CsvPlaylist
    {
        public int Index { get; set; }
        public string Id { get; set; }
        public string Artists { get; set; }
        public string Name { get; set; }
        public DateTime AddedAt { get; set; }
    }
}
