using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson.Serialization.IdGenerators;

namespace Vibe.Models
{
    public class PrivatePlaylist : PlaylistBase
    {
        [BsonElement("UserName")]
        public string UserName { get; set; }

        //[BsonElement("Songs")]
        //public IList<Song> Songs { get; set; }
    }
}