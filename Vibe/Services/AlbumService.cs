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
using Vibe.Repositories;

namespace Vibe.Services
{
    public class AlbumService : IAlbumService
    {
        SongRepository songRepo = new SongRepository();

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

         public List<Album> GetAllAlbums()
        {
            List<Album> allAlbums = new List<Album>();
            MongoCollection<Album> albumsInDb = DB.GetCollection<Album>("Album");

            foreach (Album a in albumsInDb.AsQueryable<Album>())
            {
                allAlbums.Add(a);
            }

            return allAlbums;
        }

         public List<Album> GetAllAlbumsByOneArtist(string id)
         {
             List<Album> allAlbums = new List<Album>();
             MongoCollection<Album> albumsInDb = DB.GetCollection<Album>("Album");

             foreach (Album a in albumsInDb.AsQueryable<Album>().Where(x=> x.ArtistName==id))
             {
                 allAlbums.Add(a);
             }

             return allAlbums;
         }

         public bool AddAlbum(Album a)
        {
            try
            {
                MongoCollection<Album> albums = DB.GetCollection<Album>("Album");
                return albums.Insert<Album>(a,SafeMode.True).Ok;
            }
            catch (MongoException)
            {
                return false;
            }
        }

         public Album GetAlbumById(string id)
        {
            ObjectId objId = new ObjectId(id);
            MongoCollection<Album> albums = DB.GetCollection<Album>("Album"); //ova metoda mora se proveri da li radi ili jok i koji je fazon...
            return albums.FindOneByIdAs<Album>(objId);
        }

         public bool DeleteAlbum(Album album)
        {
            MongoCollection albums = DB.GetCollection<Album>("Album");
            var removalQuery = Query.EQ("_id", album.Id);

            return albums.Remove(removalQuery,SafeMode.True).Ok;
        }

         public bool UpdateAlbum(Album album)
        {
            MongoCollection albums = DB.GetCollection<Album>("Album");
            var query = Query.EQ("_id", album.Id);
            var update = MongoDB.Driver.Builders.Update.Replace<Album>(album);
            return albums.Update(query, update, UpdateFlags.None, SafeMode.True).Ok;
        }

        public Album GetBySongId(string songId)
        {
            throw new NotImplementedException();
        }

        public string SaveImage(HttpPostedFileBase image, string name)
        {
            var path = HostingEnvironment.ApplicationPhysicalPath + "/Resources/albums/" + name + ".png";

            image.SaveAs(path);

            return "/Resources/albums/" + name + ".png";
        }

        public List<Song> getSongsInAlbum(string id)
        {
            var album = GetAlbumById(id);
            var songIds = album.Songs;
            List<Song> allSongs = new List<Song>();
            foreach (var item in songIds)
            {
                allSongs.Add(songRepo.GetById(new ObjectId(item.ToString())));
            }
            return allSongs;
        }
    }
}