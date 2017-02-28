using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson.Serialization.IdGenerators;

namespace Vibe.Models
{
    public class AlbumComment : Comment
    {
        [BsonElement("AlbumName")]
        public string AlbumName { get; set; }
    }
}