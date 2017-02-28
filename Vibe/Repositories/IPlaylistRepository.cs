using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Vibe.Models;
using MongoDB.Driver;
using MongoDB.Bson;
using MongoDB.Driver.Builders;
using MongoDB.Driver.Linq;

namespace Vibe.Repositories
{
    interface IPlaylistRepository
    {
        List<PlaylistBase> GetAll();
        bool Add(PlaylistBase playlist);
        PlaylistBase GetById(ObjectId id);
        bool Delete(PlaylistBase playlist);

        bool Delete(ObjectId id);
        bool Update(PlaylistBase playlist);

        List<PlaylistBase> GetByUser(ObjectId id);

        List<PlaylistBase> GetByUser(VibeUser u);

        List<PlaylistBase> GetPrivateByUser(ObjectId id);

        List<PlaylistBase> GetPublicByUser(ObjectId id);

        List<PlaylistBase> GetVibePlaylists();

        List<PlaylistBase> Search(string query);

        bool AddSongToPlaylist(string SongId, string playlistId);

        void RemoveSongFromPlaylist(string SongId, string playlistId);

        void LikePlaylist(string playlistId);
    }
}
