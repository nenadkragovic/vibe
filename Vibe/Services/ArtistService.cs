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
using System.Web.Hosting;

namespace Vibe.Services
{
    public class ArtistService : IArtistService
    {
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

        public List<Artist> GetAllArtists()
        {
            List<Artist> allArtists = new List<Artist>();
            MongoCollection<Artist> artistsInDb = DB.GetCollection<Artist>("Artist");

            foreach (Artist a in artistsInDb.AsQueryable<Artist>())
            {
                allArtists.Add(a);
            }

            return allArtists;
        }

        public bool AddArtist(Artist a)
        {
            try
            {
                MongoCollection<Artist> artists = DB.GetCollection<Artist>("Artist");
                return artists.Insert<Artist>(a,SafeMode.True).Ok;   
            }
            catch (MongoException)
            {
                return false;
            }
        }

        public Artist GetArtistById(string id)
        {
            ObjectId obdzektId = new ObjectId(id);
            MongoCollection<Artist> artists = DB.GetCollection<Artist>("Artist"); //ova metoda mora se proveri da li radi ili jok i koji je fazon...
            return artists.FindOneByIdAs<Artist>(obdzektId);
        }

        public bool DeleteArtist(Artist artist)
        {
            MongoCollection artists = DB.GetCollection<Artist>("Artist");
            var removalQuery = Query.EQ("_id", artist.Id);

            return artists.Remove(removalQuery,SafeMode.True).Ok;
        }

        public bool UpdateArtist(Artist artist)
        {
            MongoCollection artists = DB.GetCollection<Artist>("Artist");
            var query = Query.EQ("_id", artist.Id);
            var update = MongoDB.Driver.Builders.Update.Replace<Artist>(artist);
            return artists.Update(query, update, UpdateFlags.None, SafeMode.True).Ok;
        }

        public string SaveImage(HttpPostedFileBase image, string name)
        {
            var path = HostingEnvironment.ApplicationPhysicalPath + "/Resources/artists/" + name + ".png";

            image.SaveAs(path);

            return "/Resources/artists/" + name + ".png";
        }

        public List<Album> getAllAlbumsByArtist(string id)
        {
            var artist = GetArtistById(id);
            return artist.Albums.ToList<Album>();
        }
    }
}
