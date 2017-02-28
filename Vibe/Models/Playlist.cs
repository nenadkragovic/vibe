using System;
using System.Collections.Generic;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson.Serialization.IdGenerators;
using MongoDB.Bson;

namespace Vibe.Models
{
    public class Playlist : PlaylistBase
    {
        [BsonElement("Tag")]
        public List<string> Tags { get; set; }

        [BsonElement("NumberOfLikes")]
        public int NumberOfLikes { get; set; }

        [BsonElement("PlaylistComments")]
        public IList<PlaylistComment> PlaylistComments { get; set; }
    }
}