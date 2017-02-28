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

namespace Vibe.Repositories
{
    public class PlaylistRepository : IPlaylistRepository
    {
        private MongoDatabase dataBase;
        private UsersService userService = new UsersService();
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

        public virtual bool Add(PlaylistBase playlist)
        {
            
            if(playlist.GetType() == typeof(Playlist))
            {
                try
                {
                    MongoCollection<Playlist> playlists = DB.GetCollection<Playlist>("Playlist");
                    var realPlaylist = playlist as Playlist;
                    realPlaylist.NumberOfLikes = 0;
                    playlists.Insert<Playlist>((Playlist)realPlaylist);
                    var user = userService.User;
                    user.Playlists.Add(playlist.PlaylistId);
                    return userService.Update(user); // treba da se proveri dal je uspesno updajtovao usera // proverava :D 
                    
                }
                catch(Exception)
                {
                    return false;
                }
                
            }
            if (playlist.GetType() == typeof(PrivatePlaylist))
            {
                try
                { 

                    MongoCollection<PrivatePlaylist> privatePlaylists = DB.GetCollection<PrivatePlaylist>("PrivatePlaylist");
                    privatePlaylists.Insert<PrivatePlaylist>((PrivatePlaylist)playlist);
                    var user = userService.User;
                    user.PrivatePlaylists.Add(playlist.PlaylistId);
                    return userService.Update(user);

                }
                catch (Exception)
                {
                    return false;
                }
            }

            throw new NotImplementedException();
        }

        public virtual bool AddSongToPlaylist(string SongId, string playlistId)
        {
            var playlist = GetById(new ObjectId(playlistId));
            playlist.Songs.Add(new ObjectId(SongId));
            return Update(playlist);
        }

        public virtual bool Delete(ObjectId PlaylistId)
        {
            var removalQuery = Query.EQ("_id", PlaylistId);
                if (userService.User.Playlists.Contains(PlaylistId))
                    userService.User.Playlists.Remove(PlaylistId);
                if (userService.User.PrivatePlaylists.Contains(PlaylistId))
                    userService.User.PrivatePlaylists.Remove(PlaylistId);
                if (userService.User.LikedPlaylists.Contains(PlaylistId))
                    userService.User.LikedPlaylists.Remove(PlaylistId);

                userService.Update(userService.User);
                var playlist = GetById(PlaylistId);
                if (playlist.GetType() == typeof(Playlist))
                    return DB.GetCollection<Playlist>("Playlist").Remove(removalQuery, SafeMode.True).Ok;
                else
                    return DB.GetCollection<PrivatePlaylist>("PrivatePlaylist").Remove(removalQuery, SafeMode.True).Ok;
        }

        public virtual bool Delete(PlaylistBase playlist)
        {
            var removalQuery = Query.EQ("_id", playlist.PlaylistId);
            if(userService.User.Playlists.Contains(playlist.PlaylistId))
                userService.User.Playlists.Remove(playlist.PlaylistId);
            if (userService.User.PrivatePlaylists.Contains(playlist.PlaylistId))
                userService.User.PrivatePlaylists.Remove(playlist.PlaylistId);
            if (userService.User.LikedPlaylists.Contains(playlist.PlaylistId))
                userService.User.LikedPlaylists.Remove(playlist.PlaylistId);

            userService.Update(userService.User);
            if (playlist.GetType() == typeof(Playlist))
                return DB.GetCollection<Playlist>("Playlist").Remove(removalQuery, SafeMode.True).Ok;
            else
                return DB.GetCollection<PrivatePlaylist>("PrivatePlaylist").Remove(removalQuery, SafeMode.True).Ok;
        }

        public virtual List<PlaylistBase> GetAll()
        {
            List<PlaylistBase> pbList = new List<PlaylistBase>();
            MongoCollection<VibeUser> users = DB.GetCollection<VibeUser>("User");
            foreach (VibeUser u in users.AsQueryable<VibeUser>())
            {
                pbList.AddRange(GetPrivateByUser(u.Id));
                pbList.AddRange(GetPublicByUser(u.Id));
            }
            return pbList;
        }

        public virtual PlaylistBase GetById(ObjectId id)
        {
            //PlaylistBase p = new PlaylistBase();
            MongoCollection<Playlist> Playlists = DB.GetCollection<Playlist>("Playlist");
            var playlistId  = Playlists.FindOneByIdAs<Playlist>(id);
            if (playlistId != null) return playlistId;

            else
            {
            MongoCollection<PrivatePlaylist> privatePlaylists = DB.GetCollection<PrivatePlaylist>("PrivatePlaylist");
            var privatePlaylistsId = privatePlaylists.FindOneByIdAs<PrivatePlaylist>(id);

            return privatePlaylistsId;
            }

        }

        public virtual List<PlaylistBase> GetByUser(VibeUser u)
        {
            var p = GetPrivateByUser(u.Id);
            p.AddRange(GetPublicByUser(u.Id));
            return p;
        }

        public virtual List<PlaylistBase> GetByUser(ObjectId userId)
        {
            var p = GetPrivateByUser(userId);
            p.AddRange(GetPublicByUser(userId));
            return p;
        }

        public virtual List<PlaylistBase> GetPrivateByUser(ObjectId id)
        {
            MongoCollection<VibeUser> usersInDb = DB.GetCollection<VibeUser>("User");
            List<ObjectId> pp = usersInDb.AsQueryable<VibeUser>().Where(u => u.Id == id).First<VibeUser>().PrivatePlaylists;

            List<PlaylistBase> pl = new List<PlaylistBase>();

            if (pp != null && pp.Count > 0)
            {
                foreach (var plId in pp)
                {
                    var pb = GetById(plId);
                    if(pb != null)
                    {
                        pl.Add(pb);
                    }
                    
                }
            }
            return pl;
        }

        public virtual List<PlaylistBase> GetPublicByUser(ObjectId id)
        {
            MongoCollection<VibeUser> usersInDb = DB.GetCollection<VibeUser>("User");
            List<ObjectId> pp = usersInDb.AsQueryable<VibeUser>().Where(u => u.Id == id).First<VibeUser>().Playlists;

            List<PlaylistBase> pl = new List<PlaylistBase>();

            if(pp != null && pp.Count > 0)
            {
                foreach (var plId in pp)
                {
                    var pb = GetById(plId);
                    if (pb != null)
                    {
                        pl.Add(pb);
                    }

                }
            }

            return pl;
        }

        public virtual List<PlaylistBase> GetVibePlaylists()
        {
            VibeUser admin = userService.GetUserByUsername("Admin");
            return GetPublicByUser(admin.Id);
        }

        public virtual void LikePlaylist(string playlistId)
        {
            MongoCollection playlists = DB.GetCollection<Playlist>("Playlist");
            var selectedPlaylist = playlists.FindOneByIdAs<Playlist>(new ObjectId(playlistId));
            if (!userService.User.LikedPlaylists.Contains(selectedPlaylist.PlaylistId))
            {
                selectedPlaylist.NumberOfLikes++;
                userService.User.LikedPlaylists.Add(selectedPlaylist.PlaylistId);
            }
            else
            {
                selectedPlaylist.NumberOfLikes--;
                userService.User.LikedPlaylists.Remove(selectedPlaylist.PlaylistId);
            }
        }

        public void RemoveSongFromPlaylist(string SongId, string playlistId)
        {
            var pl = GetById(new ObjectId(playlistId));
            if (pl != null && pl.Songs.Contains(new ObjectId(SongId)))
            {
                pl.Songs.Remove(new ObjectId(SongId));
            }
            Update(pl);
        }

        public virtual List<PlaylistBase> Search(string query)
        {
            throw new NotImplementedException();
        }

        public virtual bool Update(PlaylistBase playlist)
        {
            if (playlist.GetType() == typeof(Playlist))
            {
                try
                {
                    MongoCollection playlists = DB.GetCollection<Playlist>("Playlist");
                    var query = Query.EQ("_id", playlist.PlaylistId);
                    var update = MongoDB.Driver.Builders.Update.Replace<Playlist>(playlist as Playlist);
                    return playlists.Update(query, update, UpdateFlags.None, SafeMode.True).Ok;
                }

                catch (Exception)
                {
                    return false;
                }
            }

            if (playlist.GetType() == typeof(PrivatePlaylist))
            {
                try
                {
                    MongoCollection playlists = DB.GetCollection<PrivatePlaylist>("PrivatePlaylist");
                    var query = Query.EQ("_id", playlist.PlaylistId);
                    var update = MongoDB.Driver.Builders.Update.Replace<PrivatePlaylist>(playlist as PrivatePlaylist);
                    return playlists.Update(query, update, UpdateFlags.None, SafeMode.True).Ok;
                }

                catch (Exception)
                {
                    return false;
                }

            }

            throw new NotImplementedException();
        }

    }
}