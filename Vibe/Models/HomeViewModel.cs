using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Vibe.Models
{
    public class HomeViewModel
    {
        [Required]
        [Display(Name = "Select playlist:")]
        public List<SelectListItem> Playlists { get; set; }
        public HomeViewModel()
        {

        }
    }
}