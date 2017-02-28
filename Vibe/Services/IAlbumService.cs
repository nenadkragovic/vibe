using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Vibe.Models;
using MongoDB.Driver;
using MongoDB.Bson;
using MongoDB.Driver.Builders;
using MongoDB.Driver.Linq;

namespace Vibe.Services
{
    public interface IAlbumService
    {
        List<Album> GetAllAlbums();
        List<Album> GetAllAlbumsByOneArtist(string artistName); // recimo ako zatreba ovo nzm,za svaki slucaj
        bool AddAlbum(Album album);
        Album GetAlbumById(string albumName);
        bool DeleteAlbum(Album album);
        bool UpdateAlbum(Album album);
        Album GetBySongId(string songId);
        List<Song> getSongsInAlbum(string albumName);
    }
}
