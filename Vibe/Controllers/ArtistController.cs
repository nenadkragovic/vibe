using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Vibe.Models;
using Vibe.Services;
using Vibe.Repositories;

namespace Vibe.Controllers
{
    public class ArtistController : Controller
    {

        ArtistService artistService = new ArtistService();
        SongRepository songRepo = new SongRepository();
        AlbumService albumService = new AlbumService();
        PlaylistRepository plRepo = new PlaylistRepository();
        UsersService userService = new UsersService();
        // GET: Artist
        public ActionResult Index(string name)
        {
            var artist = artistService.GetArtistById(name);
            var model = new ArtistViewModel()
            {
                ArtistName = artist.ArtistName,
                Biography = artist.Biography,
                Songs = new List<SongViewModel>(),
                Albums = artistService.getAllAlbumsByArtist(artist.Id.ToString()).Select(a => new AlbumLink() { Name = a.AlbumName, Link = Url.Action("Album","Artist", new { @id = a.Id })}).ToList(),
                ImageUrl = artist.Picture,
                Playlists = plRepo.GetByUser(userService.User).Select(pl => new SelectListItem() { Text = pl.PlaylistName, Value = pl.PlaylistId.ToString() }).ToList()
            };
            return View(model);
        }

        public ActionResult Album(string id)
        {
            var album = albumService.GetAlbumById(id);
            var artist = artistService.GetArtistById(album.ArtistName);
            var model = new AlbumViewModel()
            {
                Name = album.AlbumName,
                Picture = album.Picture,
                Year = album.Year,
                Artist = new ArtistLink() { Name = artist.ArtistName, Link = Url.Action("Index", "Artist", new { name = album.ArtistName }) },
                SongIDs = new List<Song>(),
                Id = album.Id
            };
            foreach( var val in album.Songs)
            {
                model.SongIDs.Add(songRepo.GetById(val));
            }
            return View(model);
        }
        [HttpGet]
        public ActionResult GetSongs(string id)
        {
            var songIds = albumService.GetAlbumById(id).Songs;
            var songs = new List<SongViewModel>();
            foreach (var s in songIds)
            {
                var dbSong = songRepo.GetById(s);
                var album = albumService.GetAlbumById(dbSong.Album);
                var artist = artistService.GetArtistById(album.ArtistName);
                var song = new SongViewModel()
                {
                    Name = dbSong.Name,
                    Album = new AlbumLink() { Name = album.AlbumName, Link = "javascript: void(0)" },
                    Artist = new ArtistLink() { Name = artist.ArtistName, Link = "javascript: void(0)" },
                    Likes = dbSong.Likes.ToString(),
                    SongId = dbSong.SongId.ToString(),
                    songRef = dbSong.songRef
                };
                songs.Add(song);
            }
            return Json(songs, JsonRequestBehavior.AllowGet);
        }
    }
}