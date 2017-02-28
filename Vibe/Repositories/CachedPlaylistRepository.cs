using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using StackExchange;
using ServiceStack.Redis;
using StackExchange.Redis;
using ServiceStack;
using Vibe.Models;
using Vibe.Services;
using MongoDB.Bson;

namespace Vibe.Repositories
{
    public class CachedPlaylistRepository : PlaylistRepository
    {
        RedisClient cache;
        public CachedPlaylistRepository()
        {
            cache = new RedisClient(Config.SingleHost);
        }

        public override PlaylistBase GetById(ObjectId id)
        {
            cache.As<Playlist>(); //proba najde obicnu
            Playlist pl = cache.Get<Playlist>("Playlist" + id.ToString());
            if (pl != null) //ako naso super
            {
                cache.Set<Playlist>("Playlist" + pl.PlaylistId.ToString(), pl, new TimeSpan(0, 2, 0, 0));
                return pl; //ce obnovi i vrati
            }
            else
            {
                cache.As<PrivatePlaylist>(); //ako nije onda proba privatnu
                PrivatePlaylist pp = cache.Get<PrivatePlaylist>("PrivatePlaylist" + id.ToString());
                if (pp != null) //ako nadje obnovi i vrati
                {
                    cache.Set<PrivatePlaylist>("PrivatePlaylist" + pp.PlaylistId.ToString(), pp, new TimeSpan(0, 2, 0, 0));
                    return pp;
                }
            }//sad ako nijedna nije u kes
            PlaylistBase b=base.GetById(id);
            if(b.GetType() == typeof(Playlist)) //ako mongo naso obicnu
            {
                pl = (Playlist)b;
                cache.Set<Playlist>("Playlist" + pl.PlaylistId.ToString(), pl, new TimeSpan(0, 2, 0, 0));
                return pl; //kesira obicnu i vrati je
            }
            if(b.GetType() == typeof(PrivatePlaylist)) //ili privatnu isto to
            {
                PrivatePlaylist pp = (PrivatePlaylist)b;
                cache.Set<PrivatePlaylist>("PrivatePlaylist" + pp.PlaylistId.ToString(), pp, new TimeSpan(0, 2, 0, 0));
                return pp;
            }
            return null; //ako nije naso base, tj nema takva pl. lista

        }

        public bool CachedUpdate(string playlistId)
        {
            var playlist = base.GetById(new ObjectId(playlistId));
            if (playlist.GetType() == typeof(Playlist))
            {
                try
                {
                    cache.Delete<Playlist>((Playlist)playlist);
                    cache.Set<Playlist>("Playlist" + playlist.PlaylistId.ToString(), (Playlist)playlist, new TimeSpan(0, 2, 0, 0));
                    return true;
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
                    cache.Set<PrivatePlaylist>("PrivatePlaylist" + playlist.PlaylistId.ToString(), (PrivatePlaylist)playlist, new TimeSpan(0, 2, 0, 0));
                    return true;
                }

                catch (Exception)
                {
                    return false;
                }

            }
            return false;
        }
    }
}