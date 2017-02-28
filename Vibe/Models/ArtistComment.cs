using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson.Serialization.IdGenerators;
using MongoDB.Bson;

namespace Vibe.Models
{
    public class ArtistComment : Comment
    {
        [BsonElement("ArtistName")]
        public string ArtistName { get; set; }
    }
}