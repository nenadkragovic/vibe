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
    public class ProfileController : Controller
    {
        public UsersService userService = new UsersService();
        public PlaylistRepository playlistRepo = new PlaylistRepository();
        public SongRepository songRepo = new SongRepository();

        // GET: Profile
        public ActionResult Index()
        {
            if(userService.User == null)
            {
                return RedirectToAction("Login","Account");
            }
            var model = new ProfileViewModel()
            {
                Followers = userService.User.Followers.Select(f => userService.GetByID(f)).ToList(),
                Following = userService.User.Following.Select(f => userService.GetByID(f)).ToList(),
                FullName = userService.User.FullName,
                Bio = userService.User.Bio,
                BirthDate = userService.User.BirthDate,
                Email = userService.User.Email,
                Password = userService.User.Password,
                RepeatPassword = userService.User.Password,
                ImageUrl = userService.User.imageUrl   
            };
            
            return View(model);
        }

        [HttpPost]
        public ActionResult Follow(string id)
        {
            userService.followUser(userService.GetByID(new MongoDB.Bson.ObjectId(id)));
            return new HttpStatusCodeResult(System.Net.HttpStatusCode.OK);
        }
        [HttpPost]
        public ActionResult Unfollow(string id)
        {
            userService.unfollowUser(userService.GetByID(new MongoDB.Bson.ObjectId(id)));
            return new HttpStatusCodeResult(System.Net.HttpStatusCode.OK);
        }

        [HttpGet]
        public ActionResult Discover()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Update(ProfileViewModel profile)
        {
            var currentUser = userService.User;
            if (!string.IsNullOrWhiteSpace(profile.Bio))
            {
                currentUser.Bio = profile.Bio;
            }
            if (!string.IsNullOrWhiteSpace(profile.FullName))
            {
                currentUser.FullName = profile.FullName;
            }
            if (!string.IsNullOrWhiteSpace(profile.Email))
            {
                currentUser.Email = profile.Email;
            }
            if (!string.IsNullOrWhiteSpace(profile.Password))
            {
                currentUser.Password = profile.Password;
            }
            if (profile.BirthDate != null)
            {
                currentUser.BirthDate = profile.BirthDate;
            }


            currentUser.imageUrl = userService.SaveImage(profile.Image);
            userService.Update(currentUser);
            return RedirectToAction("Index", "Profile");
        }

        [HttpGet]
        public ActionResult SearchUsers(string query)
        {
            var users = userService.Search(query);
            users = users.Where(u => u.Id != userService.User.Id).ToList();
            var ret = users.Select(u => new UserViewModel() { Name = u.FullName, Email = u.Email , Id = u.Id.ToString(), Bio = u.Bio, Picture = u.imageUrl }).ToList();
            return Json(ret, JsonRequestBehavior.AllowGet);
        }
        [HttpGet]
        public ActionResult GetUsersPlaylists(string id)
        {
            var playlists = playlistRepo.GetPublicByUser(new MongoDB.Bson.ObjectId(id));

            var ret = playlists.Select(
                    p =>
                        new UserPlaylist()
                        {
                            PlaylistName = p.PlaylistName,
                            NumberOfSongs = p.Songs.Count,
                            URL = Url.Action("Playlist", "Playlists", new { id = p.PlaylistId })
                        }
                );
            return Json(ret, JsonRequestBehavior.AllowGet);
        }
        [HttpGet]
        public ActionResult GetFollowers()
        {
            var ret = new List<UserViewModel>();
            var followers = userService.User.Followers.Select(f => userService.GetByID(f)).ToList();
            foreach (var u in followers)
            {
                var f = new UserViewModel() { Name = u.FullName, Email = u.Email, Id = u.Id.ToString(), Bio = u.Bio, Picture = u.imageUrl };
                ret.Add(f);
            }
            return Json(ret, JsonRequestBehavior.AllowGet);
        }
        [HttpGet]
        public ActionResult GetFollowing()
        {
            var ret = new List<UserViewModel>();
            var following = userService.User.Following.Select(f => userService.GetByID(f)).ToList();
            foreach (var u in following)
            {
                var f = new UserViewModel() { Name = u.FullName, Email = u.Email, Id = u.Id.ToString(), Bio = u.Bio, Picture = u.imageUrl };
                ret.Add(f);
            }
            return Json(ret, JsonRequestBehavior.AllowGet);
        }
    }
}