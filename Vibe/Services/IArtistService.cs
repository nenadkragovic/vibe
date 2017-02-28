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
    public interface IArtistService
    {
        List<Artist> GetAllArtists();
        bool AddArtist(Artist a);
        Artist GetArtistById(string id);
        bool DeleteArtist(Artist artist);
        bool UpdateArtist(Artist artist);
        List<Album> getAllAlbumsByArtist(string id);
    }
}
