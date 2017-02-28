using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Vibe.Models
{
    public class AdminViewModel
    {
        public RegisterViewModel RegUser { get; set; }

        public AddSong AddSong { get; set; }

        public AddAlbum AddAlbum { get; set; }

        public AddArtist AddArtist { get; set; }
    }
}