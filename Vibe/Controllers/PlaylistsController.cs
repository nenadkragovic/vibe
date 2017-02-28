using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Vibe.Models;
using Vibe.Services;
using Vibe.Repositories;
using MongoDB.Bson;
using Newtonsoft.Json;

namespace Vibe.Controllers
{
    public class PlaylistsController : Controller
    {
        private ISongRepository songsRepo = new SongRepository();
        private IPlaylistRepository playlistRepo = new PlaylistRepository();
        private UsersService userService = new UsersService();
        private CachedPlaylistRepository cachedPlRepo = new CachedPlaylistRepository();
        private CachedSongRepository cachedSongRepo = new CachedSongRepository();
        private ArtistService artistService = new ArtistService();
        private AlbumService albumService = new AlbumService();
        private string selectedPlaylist;
        // GET: Playlists
        public ActionResult Index()
        {
            if(userService.User == null)
            {
                return RedirectToAction("Login","Account");
            }
            var model = new PlayListsViewModel() {
                Playlists = playlistRepo.GetByUser(userService.User.Id)
            };

            return View(model);
        }
        [HttpGet]
        public ActionResult Playlist(string id)
        {
            var playlist = cachedPlRepo.GetById(new ObjectId(id));
            
            selectedPlaylist = playlist.PlaylistName;
            var others = cachedPlRepo.GetByUser(userService.User).Select(p => new SelectListItem() { Text = p.PlaylistName, Value = p.PlaylistId.ToString()}).ToList();

            if(null != others)
            {
                others = others.Where(p => p.Value != playlist.PlaylistId.ToString()).ToList();
            }
            
            var model = new PlaylistViewModel()
            {
                Description = playlist.Description,
                Name = playlist.PlaylistName,
                Songs = new List<SongViewModel>(),
                Id = playlist.PlaylistId.ToString(),
                OwnerId = playlist.Owner.ToString(),
                OtherPlaylists = others
            };
            return View(model);
        }
        [HttpGet]
        public ActionResult GetSongs(string id)
        {
            var playlist = cachedPlRepo.GetById(new ObjectId(id));

            selectedPlaylist = playlist.PlaylistName;

            var model = new PlaylistViewModel()
            {
                Songs = new List<SongViewModel>()
            };
            foreach (var s in playlist.Songs)
            {
                var album=new Album();
                var dbSong = songsRepo.GetById(s);
                var artist = new Artist();
                string artistLink = "javascript:void(0)";
                string albumlink = artistLink;
                if (playlist.GetType() == typeof(Playlist))
                {
                    album = albumService.GetAlbumById(dbSong.Album);
                    artist = artistService.GetArtistById(album.ArtistName);
                    albumlink = Url.Action("Album", "Artist",new { @id = album.Id.ToString() });
                    artistLink = Url.Action("Index", "Artist", new { name = artist.Id.ToString() });
                }
                else
                {
                    album.AlbumName = dbSong.Album;
                    artist.ArtistName = dbSong.Artist;
                }
                var song = new SongViewModel()
                {
                    Name = dbSong.Name,
                    Album = new AlbumLink() { Name = album.AlbumName, Link = albumlink },
                    Artist = new ArtistLink() { Name = artist.ArtistName, Link = artistLink },
                    Likes = dbSong.Likes.ToString(),
                    SongId = dbSong.SongId.ToString(),
                    songRef = dbSong.songRef
                };
                model.Songs.Add(song);
            }
            return Json(model.Songs, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public ActionResult New(PlayListsViewModel model)
        {
            if(model.AddNewPlaylist.Private)
            {
                PrivatePlaylist p = new PrivatePlaylist()
                {
                    Description = model.AddNewPlaylist.Description,
                    PlaylistName = model.AddNewPlaylist.Name,
                    Owner = userService.User.Id,
                    UserName = userService.User.UserName
                };
                playlistRepo.Add(p);
            }
            if (!model.AddNewPlaylist.Private)
            {
                Playlist p = new Playlist()
                {
                    Description = model.AddNewPlaylist.Description,
                    PlaylistName = model.AddNewPlaylist.Name,
                    Owner = userService.User.Id
                };
                playlistRepo.Add(p);
            }
            return RedirectToAction("Index", "Playlists");
        }
        [HttpPost]
        public ActionResult AddSongToPlaylist(string songId, string playlistId)
        {
            bool res = playlistRepo.AddSongToPlaylist(songId, playlistId);
            bool res2 = cachedPlRepo.CachedUpdate(playlistId);
            if (res && res2)
                return Json("Success", JsonRequestBehavior.AllowGet);
            else throw new Exception("Ne moze da upise ljucki");
        }
        public ActionResult Delete(string id)
        {
            playlistRepo.Delete(new ObjectId(id));
            return RedirectToAction("Index","Playlists");
        }
        public ActionResult DeleteSongFromPlaylist(string songId, string playlistId)
        {
            playlistRepo.RemoveSongFromPlaylist(songId, playlistId);
            return Json("Success"); 
        }
    }
}