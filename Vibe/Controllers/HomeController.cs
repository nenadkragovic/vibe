using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using Vibe.Models;
using Vibe.Repositories;
using Vibe.Services;

namespace Vibe.Controllers
{
    public class HomeController : Controller
    {
        private ISongRepository songsRepo = new SongRepository();
        private IPlaylistRepository playlistRepo = new PlaylistRepository();
        private UsersService _usersService = new UsersService();
        private ArtistService artistService = new ArtistService();
        private AlbumService albumService = new AlbumService();
        public ActionResult Index()
        {
            var model = new HomeViewModel();
            var user = _usersService.User;
            if (user != null)
            {
                model.Playlists = playlistRepo.GetByUser(user.Id).Select(p => new SelectListItem() { Text = p.PlaylistName, Value = p.PlaylistId.ToString() }).ToList();
            }
            else
            {
                return RedirectToAction("Login", "Account");
            }
            return View(model);
        }
        [HttpGet]
        public ActionResult Search(string query)
        {
            List<Song> dbSongs = new List<Song>();

            if (!string.IsNullOrEmpty(query))
            {
                dbSongs = songsRepo.Search(query).ToList<Song>();
            }
            var songs = new List<SongViewModel>();

            foreach(var s in dbSongs)
            {
                if (s.Owner == new ObjectId("58a86904bd1f138ddc58847b"))
                {
                    var album = albumService.GetAlbumById(s.Album);
                    var artist = artistService.GetArtistById(album.ArtistName);
                    songs.Add(new SongViewModel()
                    {
                        Name = s.Name,
                        Album = new AlbumLink() { Name = album.AlbumName , Link = Url.Action("Album", "Artist", new { id = s.Album }) },
                        Artist = new ArtistLink() { Name = artist.ArtistName, Link = Url.Action("Index", "Artist", new { name = s.Artist }) },
                        Likes = s.Likes.ToString(),
                        SongId = s.SongId.ToString(),
                        songRef = s.songRef
                    });
                }
            }

            return Json(songs, JsonRequestBehavior.AllowGet);
        }
        [HttpGet]
        public ActionResult Audio(string id)
        {                                                                                             
            return File(songsRepo.GetAudioBlob(id), "audio/mp3");
        }
    }
}