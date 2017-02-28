using MongoDB.Bson;
using System.Collections.Generic;
using Vibe.Models;

namespace Vibe.Repositories
{
    interface ISongRepository
    {
        Song GetById(ObjectId id);
        bool Add(Song s);
        bool Update(Song s);

        bool Delete(Song s);

        List<Song> GetByUser(VibeUser u);

        List<Song> GetByArtist(Artist a);

        List<Song> GetByAlbum(Album a);
        bool Delete(ObjectId id);

        List<Song> GetByUser(ObjectId id);

        List<Song> GetByArtist(string id);

        List<Song> GetByAlbum(string id);

        byte[] GetAudioBlob(string SongId);

        void LikeSong(string songId);

        void LeaveAComment(string songId, string userId, string comment);

        IList<Song> Search(string query);
    }
}
