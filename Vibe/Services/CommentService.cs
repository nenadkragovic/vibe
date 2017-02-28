﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Vibe.Models;
using MongoDB.Driver;
using MongoDB.Bson;
using MongoDB.Driver.Builders;
using MongoDB.Driver.Linq;
using System.IO;

namespace Vibe.Services
{
    public class CommentService : ICommentService
    {
        UsersService _userService;
        private MongoDatabase dataBase;
        public MongoDatabase DB
        {
            get
            {
                if (dataBase == null)
                {
                    MongoServer server = MongoServer.Create("mongodb://localhost/");
                    dataBase = server.GetDatabase("vibe");
                }
                return dataBase;
            }
        }

        public void placeComment(VibeUser user, string text, Playlist p)
        {
            MongoCollection comments = DB.GetCollection<PlaylistComment>("PlaylistComment");
            comments.Insert<PlaylistComment>(new PlaylistComment { Text = text, NumberOfLikes = 0, TimeOfSubmit = DateTime.Now, UserId = user.Id, PlaylistId = p.PlaylistId });
        }

        public List<PlaylistComment> getAllPlaylistsComments(Playlist p)
        {
            return p.PlaylistComments.ToList();
        }
    }
}