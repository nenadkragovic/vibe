using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Web;
using System.Web.Mvc;

namespace Vibe.Models
{
    public class MySongsViewModel
    {
        [Required]
        public HttpPostedFileBase Song { get; set; }

        [Display(Name = "Artist:")]
        public string Artist { get; set;}
        [Required]
        [Display(Name = "Name:")]
        public string Name { get; set; }
        [Display(Name = "Album:")]
        public string Album { get; set; }
        [Display(Name = "Year:")]
        public string Year { get; set; }
        [Display(Name = "Genres:")]
        public IList<string> Genres { get; set; }
        [Display(Name = "Tags:")]
        public string Tags { get; set; }

        public Song SelectedSong { get; set; }

        public List<SelectListItem> Playlists { get; set; }

    }
}