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
    public class Album
    {
        public Album()
        {
            Songs = new List<ObjectId>();
            Tags = new List<string>();
            Genres = new List<string>();
        }
        [BsonId]
        public ObjectId Id { get; set; }

        [BsonElement("AlbumName")]
        public string AlbumName { get; set; }

        [BsonElement("Artist")]
        public string ArtistName { get; set; }

        [BsonElement("Songs")]
        public IList<ObjectId> Songs { get; set; }
        // public IList<Song> Songs { get; set; } original je to bio ali mislim da bude bolje lista id-jeva

        [BsonElement("Year")]
        public int Year { get; set; }

        [BsonElement("Tags")]
        public IList<string> Tags { get; set; }

        [BsonElement("Genres")]
        public IList<string> Genres { get; set; }

        [BsonElement("NumberOfHits")]
        public int NumberOfHits { get; set; }

        [BsonElement("ArtistComments")]
        public IList<AlbumComment> AlbumComments { get; set; }

        [BsonElement("Picture")]
        public string Picture { get; set; }
    }
    public class AddAlbum
    {
        public AddAlbum()
        {
            ArtistIds = new List<SelectListItem>();
            Year = DateTime.Now.Year;
        }
        [Required]
        [Display(Name = "Name:")]
        public string Name { get; set; }

        [Display(Name = "Artist:")]
        public List<SelectListItem> ArtistIds { get; set; }

        [Required]
        [Display(Name = "ArtistId")]
        public string ArtistId { get; set; }

        [Required]
        [Display(Name = "Year:")]
        public int Year { get; set; }

        [Required]
        [Display(Name = "Tags:")]
        public string Tags { get; set; }

        [Required]
        [Display(Name = "Genres:")]
        public string Genres { get; set; }

        [Required]
        [Display(Name = "Picture")]
        public HttpPostedFileBase Picture { get; set; }

    }
    public class AlbumViewModel
    {
        public ObjectId Id { get; set; }

        public string Name { get; set; }

        public int Year { get; set; }

        public ArtistLink Artist { get; set; }

        public string Picture { get; set; }

        public List<Song> SongIDs { get; set; }
    }
    public class ArtistLink
    {
        public string Name { get; set; }

        public string Link { get; set; }
    }
}