using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Vibe.Repositories;
using Vibe.Services;
using Vibe.Models;
using System.Reflection;

namespace Vibe.Controllers
{
    public class AdminController : Controller
    {
        // GET: Admin
        UsersService userService = new UsersService();
        PlaylistRepository playlistRepo = new PlaylistRepository();
        SongRepository songRepo = new SongRepository();
        ArtistService artistRepo = new ArtistService();
        AlbumService albumRepo = new AlbumService();

        public ActionResult Index()
        {
            AddAlbum addA = new AddAlbum();
            AddArtist AddAr = new AddArtist();
            AddSong addS = new AddSong();

            addA.ArtistIds = artistRepo.GetAllArtists().Select(artist => new SelectListItem() { Text = artist.ArtistName, Value = artist.Id.ToString() }).ToList();
            addA.ArtistId = addA.ArtistIds.First().Value.ToString();
            //addA.ArtistId = "1";
            //addS.SelectedArtist = "1";
            //addS.SelectedAlbum = "1";
            addS.Artists = addA.ArtistIds;
            addS.SelectedArtist = addA.ArtistId;
            var alfa = albumRepo.GetAllAlbums();
            addS.Albums = alfa.Select(album => new SelectListItem() { Text = album.AlbumName, Value = album.Id.ToString() }).ToList();
            addS.SelectedAlbum = addS.Albums.First().Value.ToString();

            var model = new AdminViewModel();
            //{

            //    AddAlbum = new AddAlbum()
            //    {
            //        ArtistIds = artistRepo.GetAllArtists().Select(artist => new SelectListItem() { Text = artist.ArtistName, Value = artist.Id.ToString() }).ToList(),
            //        ArtistId = artistRepo.GetAllArtists().First<Artist>().Id.ToString()
            //    },
            //    AddArtist = new Models.AddArtist(),
            //    AddSong = new AddSong()
            //    {
            //        Albums = albumRepo.GetAllAlbums().Select(al => new SelectListItem() { Text = al.AlbumName, Value = al.Id.ToString() }).ToList(),
            //        Artists = artistRepo.GetAllArtists().Select(a => new SelectListItem() { Text = a.ArtistName, Value = a.Id.ToString() }).ToList(),
            //        SelectedArtist = artistRepo.GetAllArtists().First<Artist>().Id.ToString(),
            //        SelectedAlbum = albumRepo.GetAllAlbumsByOneArtist(artistRepo.GetAllArtists().First<Artist>().Id.ToString()).First().ToString()
            //    }
            //};
            model.AddArtist = AddAr;
            model.AddAlbum = addA;
            model.AddSong = addS;
            return View(model);
        }

        [HttpPost]
        public ActionResult AddAlbum(AdminViewModel vm)
        {
            AddAlbum album = vm.AddAlbum;
            string[] tmp = album.Genres.Split(' ');
            Album a = new Album();
            foreach (string s in tmp)
            {
                a.Genres.Add(s);
            }
            tmp = album.Tags.Split(' ');
            foreach (string s in tmp)
            {
                a.Tags.Add(s);
            }

            a.Picture = albumRepo.SaveImage(album.Picture, album.Name);
            a.Year = album.Year;
            a.AlbumName = album.Name;
            string artistref = album.ArtistId;
            var artist = new Artist();
            artist = artistRepo.GetArtistById(artistref);
            a.ArtistName = artist.Id.ToString();
            albumRepo.AddAlbum(a);
            artist.Albums.Add(a);
            artistRepo.UpdateArtist(artist);
            return RedirectToAction("Index", "Admin");
        }

        [HttpPost]
        public ActionResult AddArtist(AdminViewModel vm)
        {
            bool isEmpty = false;
            AddArtist artist = vm.AddArtist;
            Type getInfoRequestType = artist.GetType();
            PropertyInfo[] myProps = getInfoRequestType.GetProperties();
            foreach (var item in myProps)
            {
                if (item == null)
                    isEmpty = true;
            }
            if (!isEmpty)
            {
                Artist a = new Artist();
                string[] tmp = artist.Genres.Split(' ');
                foreach (string s in tmp)
                {
                    a.Genres.Add(s);
                }
                tmp = artist.Tags.Split(' ');
                foreach (string s in tmp)
                {
                    a.Tags.Add(s);
                }
                a.Picture = artistRepo.SaveImage(artist.Picture, artist.Name);
                a.ArtistName = artist.Name;
                a.Biography = artist.Biography;
                artistRepo.AddArtist(a);
                return RedirectToAction("Index", "Admin");
            }
            return RedirectToAction("Index", "Admin");
        }

        [HttpPost]
        public ActionResult AddSong(AdminViewModel vm)
        {
            var addsong = vm.AddSong;
            Song song = new Song()
            {
                Name=addsong.Name,
                YearOfRelease = addsong.Year,
                Album = addsong.SelectedAlbum,
                Artist = addsong.SelectedArtist,
                File = addsong.File.InputStream,
                Owner = new MongoDB.Bson.ObjectId("58a86904bd1f138ddc58847b")
            };
            string[] tmp = addsong.Genres.Split(' ');
            foreach (string s in tmp)
            {
                song.Genres.Add(s);
            }
            tmp = addsong.Tags.Split(' ');
            foreach (string s in tmp)
            {
                song.Tag.Add(s);
            }
            if (userService.User.Id == new MongoDB.Bson.ObjectId("58a86904bd1f138ddc58847b"))
            {
                songRepo.Add(song);
                var album = albumRepo.GetAlbumById(song.Album);
                album.Songs.Add(song.SongId);
                albumRepo.UpdateAlbum(album);
            }
            return RedirectToAction("Index", "Admin");
        }
    }
}