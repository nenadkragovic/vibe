using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson.Serialization.IdGenerators;
using MongoDB.Bson;

namespace Vibe.Models
{
    public class PlaylistComment : Comment
    {
        [BsonElement("Playlist")]
        public ObjectId PlaylistId { get; set; }
    }
}