using System;
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
    interface ICommentService
    {
        void placeComment(VibeUser user, string text, Playlist p);
        List<PlaylistComment> getAllPlaylistsComments(Playlist p);
    }
}
