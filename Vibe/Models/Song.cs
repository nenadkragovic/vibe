using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson.Serialization.IdGenerators;
using MongoDB.Bson;
using MongoDB.Driver.GridFS;
using Vibe.Services;

namespace Vibe.Models
{
    public class Song
    {
        public Song()
        {
            this.Genres = new List<string>();
            this.Tag = new List<string>();
        }
        [BsonId]
        public ObjectId SongId { get; set; }

        [BsonElement("Name")]
        public string Name { get; set; }

        [BsonElement("Artist")]
        public string Artist { get; set; }

        [BsonElement("Album")]
        public string Album { get; set; }

        [BsonElement("Length")]
        public string Length { get; set; }

        [BsonElement("YearOfRelease")]
        public int YearOfRelease { get; set; }

        [BsonElement("Genres")]
        public IList<string> Genres { get; set; }

        [BsonElement("Tag")]
        public IList<string> Tag { get; set; }
        public string songRef { get; set; }
        [BsonElement("FilePath")]
        public Stream File { get; set; }

        public ObjectId Owner { get; set; }

        public int Likes { get; set; }

    }
}