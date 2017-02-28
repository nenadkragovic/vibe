using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson.Serialization.IdGenerators;
using MongoDB.Bson;

namespace Vibe.Models
{
    public abstract class Comment
    {
        [BsonId]
        public ObjectId CommentId { get; set; }

        [BsonElement("Username")]
        public ObjectId UserId { get; set; }

        [BsonElement("Text")]
        public string Text { get; set; }

        [BsonDateTimeOptions]
        public DateTime TimeOfSubmit { get; set; }

        [BsonElement("NumberOfLikes")]
        public int NumberOfLikes { get; set; }

    }
}