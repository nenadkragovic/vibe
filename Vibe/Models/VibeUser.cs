using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson.Serialization.IdGenerators;
using MongoDB.Bson;
using MongoDB.Driver;
using System.IO;
using Vibe.Services;

namespace Vibe.Models
{
    public class VibeUser
    {
        UsersService userService = new UsersService();

        public VibeUser()
        {
            this.PrivatePlaylists = new List<ObjectId>();
            this.Playlists = new List<ObjectId>();
            this.Following = new List<ObjectId>();
            this.Followers = new List<ObjectId>();
            this.LikedSongs = new List<ObjectId>();
            this.LikedPlaylists = new List<ObjectId>();
        }
        public string UserName { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string FullName { get; set; }
        [BsonId]
        public ObjectId Id { get; set; }

        public DateTime BirthDate { get; set; }

        public DateTime SignUpDate { get; set; }

        public List<ObjectId> Following { get; set; }

        public List<ObjectId> Followers { get; set; }

        public List<ObjectId> Playlists { get; set; }

        public List<ObjectId> PrivatePlaylists { get; set; }

        public List<ObjectId> LikedPlaylists { get; set; }

        public List<ObjectId> LikedSongs { get; set; }

        public string Bio { get; set; }

        public string imageUrl { get; set; }
    }

    public class UserViewModel
    {
        public string Name { get; set; }

        public string Email { get; set; }

        public string Id { get; set; }

        public string Bio { get; set; }

        public string Picture { get; set; }
    }
}