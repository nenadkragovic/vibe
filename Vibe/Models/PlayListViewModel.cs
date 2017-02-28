using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Vibe.Models
{
    public class PlayListsViewModel
    {

        public List<PlaylistBase> Playlists { get; set; }

        public NewPlaylistViewModel AddNewPlaylist { get; set; }
    }
    public class NewPlaylistViewModel
    {
        [Required]
        [Display(Name = "Name your playlist:")]
        public string Name { get; set; }
        [Display(Name = "Discribe your playlist:")]
        public string Description { get; set; }

        [Required]
        [Display(Name = "Private")]
        public bool Private { get; set; }
    }
    public class PlaylistViewModel
    {
        public string Name { get; set; }
        public string Description { get; set; }

        public string Id { get; set; }

        public string OwnerId { get; set; }
        public List<SongViewModel> Songs { get; set; }

        [Required]
        [Display(Name = "Select playlist:")]
        public List<SelectListItem> OtherPlaylists { get; set; }
    }
    public class UserPlaylist
    {
        public string PlaylistName { get; set; }

        public string URL { get; set; }

        public int NumberOfSongs { get; set; }
    }
}