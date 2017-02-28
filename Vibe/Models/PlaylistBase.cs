using System;
using System.Collections.Generic;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Bson.Serialization.Attributes;

namespace Vibe.Models
{
    public class PlaylistBase
    {
        public PlaylistBase()
        {
            this.Songs = new List<ObjectId>();
        }

        [BsonId]
        public ObjectId PlaylistId { get; set; }

        [BsonElement("PlaylistName")]
        public string PlaylistName { get; set; }

        [BsonElement("Songs")]
        public IList<ObjectId> Songs { get; set; }

        [BsonElement("Description")]
        public string Description { get; set; }

        [BsonElement("Owner")]
        public ObjectId Owner { get; set; }

    }
}
