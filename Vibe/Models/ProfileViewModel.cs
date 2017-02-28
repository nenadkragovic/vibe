using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.IO;
using System.ComponentModel.DataAnnotations;

namespace Vibe.Models
{
    public class ProfileViewModel
    {
        [Display(Name = "Birth date:")]
        public DateTime BirthDate { get; set; }

        [Required]
        [Display(Name = "Password:")]
        public string Password { get; set; }

        [Required]
        [Display(Name = "Repeat password:")]
        public string RepeatPassword { get; set; }

        [Required]
        [Display(Name = "Name:")]
        public string FullName { get; set; }

        [Display(Name = "Bio")]
        public string Bio { get; set; }

        [Required]
        public HttpPostedFileBase Image { get; set; }

        [Display(Name = "Email")]
        public string Email { get; set; }

        public List<VibeUser> Followers { get; set; }

        public List<VibeUser> Following { get; set; }

        public string ImageUrl { get; set; }

    }
}