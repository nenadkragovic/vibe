using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Vibe.Models;
using Vibe.Repositories;
using Vibe.Services;

namespace Vibe.Controllers
{
    public class MySongsController : Controller
    {
        private UsersService userService = new UsersService();
        private SongRepository songRepo = new SongRepository();
        private PlaylistRepository playlistRepo = new PlaylistRepository();
        private CachedSongRepository cachedSongRepo = new CachedSongRepository();
        private AlbumService albumService = new AlbumService();
        private ArtistService artistService = new ArtistService();
        public ActionResult Index()
        {
            if (userService.User.Id != new ObjectId("58a86904bd1f138ddc58847b"))
            {
                var model = new MySongsViewModel();
                if (userService.User == null)
                {
                    return RedirectToAction("Login", "Account");
                }
                ViewData["user"] = userService.User;
                model.Playlists = new List<SelectListItem>();
                var playlists = playlistRepo.GetPrivateByUser(userService.User.Id);
                foreach (var pl in playlists)
                {
                    model.Playlists.Add(new SelectListItem() { Text = pl.PlaylistName, Value = pl.PlaylistId.ToString(), Selected = true });
                }
                return View(model);
            }
            else
            {
                return RedirectToAction("Index", "Admin");
            }
        }

        [HttpPost]
        public ActionResult AddSong(MySongsViewModel model)
        {
            
                var song = new Song()
                {
                    Name = model.Name,
                    Artist = model.Artist,
                    Album = model.Album,
                    File = model.Song.InputStream,
                    Owner = userService.User.Id

                };
                songRepo.Add(song);
                return RedirectToAction("Index", "MySongs");
        }
        [HttpGet]
        public ActionResult Search(string query)
        {
            List<Song> dbSongs = new List<Song>();

            if (!string.IsNullOrEmpty(query))
            {
                dbSongs = songRepo.Search(query).ToList<Song>();
            }
            var songs = new List<SongViewModel>();

            foreach (var s in dbSongs)
            {
                if (s.Owner == userService.User.Id)
                {
                    songs.Add(new SongViewModel()
                    {
                        Name = s.Name,
                        Album = new AlbumLink() { Name = s.Album, Link = "javascript: void(0)" },
                        Artist = new ArtistLink() { Name = s.Artist, Link = "javascript: void(0)" },
                        Likes = s.Likes.ToString(),
                        SongId = s.SongId.ToString(),
                        songRef = s.songRef
                    });
                }
            }

            return Json(songs, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public ActionResult Like(string songId)
        {
            songRepo.LikeSong(songId);
            return new HttpStatusCodeResult(System.Net.HttpStatusCode.OK);
        }
        [HttpPost]
        public ActionResult Delete(string SongId)
        {
            var song = cachedSongRepo.GetById(new ObjectId(SongId));
            songRepo.Delete(song);

            return null;
        }
    }
}