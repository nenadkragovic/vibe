using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MongoDB.Bson;
using Vibe.Models;
using MongoDB.Driver;
using MongoDB.Driver.Linq;
using Vibe.Services;
using MongoDB.Driver.Builders;
using System.IO;
using System.Threading.Tasks;
using System.Reflection;

namespace Vibe.Repositories
{
    public class SongRepository : ISongRepository
    {
        UsersService _userService = new UsersService();
        IPlaylistRepository playlistRepo = new PlaylistRepository();
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

        public virtual Song GetById(ObjectId id)
        {
            MongoCollection songs = DB.GetCollection<Song>("Song");
            return songs.FindOneByIdAs<Song>(id);
        }

        public bool Add(Song song)
        {
                MongoCollection songs = DB.GetCollection<Song>("Song");

                //var testFileName = "C:\\Users\\Matija\\Downloads\\System Of A Down - Chop Suey!.mp3";  //sta ce radimo s ovim?
                var gridFs = new MongoGridFs(dataBase);

                var id = ObjectId.Empty;
                var file = song.File;
                id = gridFs.AddFile(file,song.Name);
                song.songRef = id.ToString(); // ovo mora se postavi ovako manuelno jer se tu gore generise
                song.File = null;
                song.Likes = 0;

                return songs.Insert<Song>(song, SafeMode.True).Ok;
        }

        public bool Update(Song song)
        {
            try
            {
                MongoCollection songs = DB.GetCollection<Song>("Song");
                var query = Query.EQ("_id", song.SongId);
                var update = MongoDB.Driver.Builders.Update.Replace<Song>(song);
                return songs.Update(query, update, UpdateFlags.None, SafeMode.True).Ok;
            }

            catch (Exception)
            {
                return false;
            }
        }

        public bool Delete(Song song)
        {
            var allPlaylists = new List<PlaylistBase>();
            var removalQuery = Query.EQ("_id", song.SongId);
            if (_userService.User != null)
            {
                if (playlistRepo.GetPublicByUser(_userService.User.Id) != null)
                    allPlaylists = playlistRepo.GetPublicByUser(_userService.User.Id);
                if (playlistRepo.GetPrivateByUser(_userService.User.Id) != null)
                    allPlaylists.AddRange(playlistRepo.GetPrivateByUser(_userService.User.Id));

                foreach (var p in allPlaylists)
                {
                    if (p.Songs.Contains(song.SongId))
                    {
                        p.Songs.Remove(song.SongId);
                        playlistRepo.Update(p);
                    }
                }
                var usersLikedSongs = _userService.User.LikedSongs;

                if (usersLikedSongs.Contains(song.SongId))
                {
                    usersLikedSongs.Remove(song.SongId);
                }
                _userService.Update(_userService.User);

                return DB.GetCollection<Song>("Song").Remove(removalQuery, SafeMode.True).Ok;
            }
            return false;
        }

        public List<Song> GetByUser(VibeUser u)
        {
            return DB.GetCollection<Song>("Song").AsQueryable<Song>().Where(s => s.Owner == u.Id).ToList<Song>();
        }

        public List<Song> GetByUser(ObjectId id)
        {
            var kur = DB.GetCollection<Song>("Song").AsQueryable<Song>().Where(s => s.Owner == id).ToList<Song>();
            return kur;

        }

        public List<Song> GetByArtist(Artist a)
        {
            return DB.GetCollection<Song>("Song").AsQueryable<Song>().Where(x => x.Artist == a.ArtistName).Select(x => x).ToList<Song>();
        }

        public List<Song> GetByArtist(string id)
        {
            return DB.GetCollection<Song>("Song").AsQueryable<Song>().Where(x => x.Artist == id).Select(x => x).ToList<Song>();
        }

        public List<Song> GetByAlbum(Album a)
        {
            return DB.GetCollection<Song>("Song").AsQueryable<Song>().Where(x => x.Album == a.AlbumName).Select(x => x).ToList<Song>();
        }

        public List<Song> GetByAlbum(string id)
        {
            return DB.GetCollection<Song>("Song").AsQueryable<Song>().Where(x => x.Album == id).Select(x => x).ToList<Song>();
        }

        public bool Delete(ObjectId id)
        {
            var removalQuery = Query.EQ("SongId", id);
            return DB.GetCollection<Song>("Song").Remove(removalQuery, SafeMode.True).Ok;
        }

        public IList<Song> All()
        {
            return DB.GetCollection<Song>("Song").FindAllAs<Song>().ToList<Song>();

        }

        public IList<Song> Search(string query)
        {
            query = query.ToLower();
            MongoCollection<Song> allSongs = DB.GetCollection<Song>("Song");
            var songList = allSongs.AsQueryable<Song>();
            List<Song> listToReturn = new List<Song>();
            foreach (Song s in songList)
            {
                if (s.Name != null && s.Name.ToLower().Contains(query))
                {
                    listToReturn.Add(s);
                    continue;
                }
                if (s.Album != null && s.Album.ToLower().Contains(query))
                {
                    listToReturn.Add(s);
                    continue;
                }
                if (s.Artist != null && s.Artist.ToLower().Contains(query))
                {
                    listToReturn.Add(s);
                    continue;
                }
                if(s.Genres != null)
                foreach (var g in s.Genres)
                {
                    if (g.ToLower().Contains(query))
                    {
                        listToReturn.Add(s);
                        break;
                    }
                }
                if (s.Tag != null && s.Tag.Select(x=>x.ToLower()).Contains(query))
                {
                    listToReturn.Add(s);
                    continue;
                }


            }
            return listToReturn;
            
        }

        public IList<Song> SearchByArtist(string artist)
        {
            throw new NotImplementedException();
        }

        public IList<Song> SearchByCategory(string category)
        {
            throw new NotImplementedException();
        }

        public Song GetSongByRefId(ObjectId id)
        {
            MongoCollection<Song> songs = DB.GetCollection<Song>("Song");
            return songs.AsQueryable().Where(x => x.songRef == id.ToString()).First<Song>();
        }

        public Stream RetrieveSongFromDb(Song s)
        {
            MongoCollection songs = DB.GetCollection<Song>("Song");
            var gridFs = new MongoGridFs(dataBase);
            var file = gridFs.GetFile(new ObjectId(s.songRef));
            return file;
        }
        public byte[] GetAudioBlob(string SongId)
        {

            var song = GetSongByRefId(new ObjectId(SongId));

            song.File = RetrieveSongFromDb(song);

            byte[] blob = new byte[song.File.Length];
            int res = song.File.Read(blob, 0, (int)song.File.Length);

            return blob;
        }

        public void LikeSong(string songId)
        {
            MongoCollection songs = DB.GetCollection<Song>("Song");
            var selectedSong = songs.FindOneByIdAs<Song>(new ObjectId(songId));

            if (!_userService.User.LikedSongs.Contains(selectedSong.SongId))
            {
                selectedSong.Likes++;
                _userService.User.LikedSongs.Add(selectedSong.SongId);
            }
            else
            {
                selectedSong.Likes--;
                _userService.User.LikedSongs.Remove(selectedSong.SongId);
            }
            
            _userService.Update(_userService.User);
            Update(selectedSong);

        }

        public void LeaveAComment(string songId, string userId, string comment)
        {
            throw new NotImplementedException();
        ////    MongoCollection comments = DB.GetCollection<PlaylistComment>("PlaylistComment");
        ////    comments.Insert<PlaylistComment>(new PlaylistComment { Text = text, NumberOfLikes = 0, TimeOfSubmit = DateTime.Now, UserId = user.Id, PlaylistId = p.PlaylistId });
        }
    }
}