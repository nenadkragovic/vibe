using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson.Serialization.IdGenerators;
using MongoDB.Bson;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace Vibe.Models
{
    public class Artist
    {
        public Artist()
        {
            this.Genres = new List<string>();
            this.Tags = new List<string>();
            this.Albums = new List<Album>();
        }

        [BsonId]
        public ObjectId Id { get; set; }

        [BsonElement("ArtistName")]
        public string ArtistName { get; set; }

        [BsonElement("Albums")]
        public IList<Album> Albums { get; set; }

        [BsonElement("Biography")]
        public string Biography { get; set; }

        [BsonElement("ArtistSongs")]
        public IList<ObjectId> ArtistSongs { get; set; }

        [BsonElement("Tags")]
        public IList<string> Tags { get; set; }

        [BsonElement("Genres")]
        public IList<string> Genres { get; set; }

        [BsonElement("ArtistComments")]
        public IList<ArtistComment> ArtistComments { get; set; }

        [BsonElement("Picture")]
        public string Picture { get; set; }

    }
    public class AddArtist
    {
        [Required]
        [Display(Name = "Name:")]
        public string Name { get; set; }
        [Required]
        [Display(Name = "Bio:")]
        public string Biography { get; set; }

        [Display(Name = "Tags:")]
        public string Tags { get; set; }

        [Display(Name = "Genres:")]
        public string Genres { get; set; }
        [Required]
        [Display(Name = "Picture")]
        public HttpPostedFileBase Picture { get; set; }

    }
    public class ArtistViewModel
    {
        public string ArtistName { get; set; }

        public List<AlbumLink> Albums { get; set; }

        public string Biography { get; set; }

        public List<SongViewModel> Songs { get; set; }

        public string ImageUrl { get; set; }
        public List<SelectListItem> Playlists { get; internal set; }
    }
    public class AlbumLink
    {
        public string Name { get; set; }

        public string Link { get; set; }
    }
}