using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.IO;
using MongoDB.Bson;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace Vibe.Models
{
    public class SongViewModel
    {
        public string Name { get; set; }

        public ArtistLink Artist { get; set; }

        public AlbumLink Album { get; set; }

        public string Likes { get; set; }

        public string SongId { get; set; }

        public string songRef { get; set; }
    }
    public class AddSong
    {
        public AddSong()
        {
            Artists = new List<SelectListItem>();
            Albums = new List<SelectListItem>();
            Year = DateTime.Now.Year;
        }
        [Required]
        [Display(Name ="Name:")]
        public string Name { get; set; }

        [Required]
        [Display(Name = "Select file:")]
        public HttpPostedFileBase File { get; set; }

        [Required]
        public string SelectedArtist { get; set; }

        [Required]
        public string SelectedAlbum { get; set; }

        [Display(Name = "Artist:")]
        public List<SelectListItem> Artists { get; set; }

        [Display(Name = "Album:")]
        public List<SelectListItem> Albums { get; set; }

        [Required]
        [Display(Name = "Genres:")]
        public string Genres { get; set; }

        [Required]
        [Display(Name = "Tags:")]
        public string Tags { get; set; }

        [Required]
        [Display(Name = "Year:")]
        public int Year { get; set; }
    }
}