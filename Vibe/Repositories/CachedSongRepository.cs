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
    public class CachedSongRepository:SongRepository
    {
        RedisClient cache;
        public CachedSongRepository()
        {
            cache = new RedisClient(Config.SingleHost);
        }

        public override Song GetById(ObjectId id)
        {
            cache.As<Song>();
            Song s = cache.Get<Song>("Song" + id.ToString()); //potrazi tu pesmu
            if (s != null)
            {
                cache.Set<Song>("Song" + s.songRef.ToString(), s, new TimeSpan(0, 2, 0, 0)); //updatuje ttl
                return s; //vrati je ili vrati null
            }
            else //kad nije u redis
            {
                s = base.GetById(id);
                cache.Set<Song>("Song" + s.songRef.ToString(), s, new TimeSpan(0, 2, 0, 0)); //stavi u redis
                return s; //vrati je ili vrati null
            }
        }

    }
}