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
    public class UsersService : IUsersService
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
        public VibeUser User
        {
            get
            {
                return (VibeUser)HttpContext.Current.Session["user"] ?? null;
            }
            set
            {
                HttpContext.Current.Session["user"] = value;
            }
        }
        public List<VibeUser> GetAllUsers()
        {
            List<VibeUser> allUsers = new List<VibeUser>();
            MongoCollection<VibeUser> usersInDb = DB.GetCollection<VibeUser>("User");
            foreach(VibeUser u in usersInDb.AsQueryable<VibeUser>())
            {
                allUsers.Add(u);
            }
            return allUsers;
        }

        public VibeUser GetUserByUsername(string username)
        {
            MongoCollection<VibeUser> users = DB.GetCollection<VibeUser>("User");
            return users.FindOneByIdAs<VibeUser>(username);
        }

        public VibeUser Login(string email, string password)
        {
            //var query = Query.And(Query.EQ("UserName",username),Query.EQ("Password",password));
            MongoCollection<VibeUser> usersCollection = DB.GetCollection<VibeUser>("User");
            foreach (VibeUser u 
                in usersCollection.AsQueryable<VibeUser>())
            {
                if (u.Email == email && u.Password == password)
                {
                    User = u;
                    return u;
                }
                    
            }
            return null;
        }

        public bool Register(VibeUser user)
        {
            MongoCollection<VibeUser> users = DB.GetCollection<VibeUser>("User");
            VibeUser newUser = new VibeUser()
            {
                SignUpDate = DateTime.Now,
                UserName = user.UserName,
                Password = user.Password,
                Email = user.Email,
                FullName = user.FullName,
                imageUrl = "/Content/img/user.png"
            };
            users.Insert<VibeUser>(newUser);
            User = newUser;
            return true;
        }

        public bool Update(VibeUser user)
        {
            try
            {
                MongoCollection<VibeUser> users = DB.GetCollection<VibeUser>("User");
                var query = Query.EQ("_id",user.Id);
                var update = MongoDB.Driver.Builders.Update.Replace<VibeUser>(user);
                return users.Update(query, update,UpdateFlags.None, SafeMode.True).Ok;
            }
            catch(Exception)
            {
                return false;
            }
        }

        public List<VibeUser> getFollowing(VibeUser user)
        {
            var idsOfUsers = user.Following;

            MongoCollection<VibeUser> users = DB.GetCollection<VibeUser>("User");

            List<VibeUser> following = new List<VibeUser>();

            foreach (var u in users.AsQueryable<VibeUser>())
            {
                if (idsOfUsers.Contains(u.Id))
                    following.Add(u);
            }

            return following;
        }

        public List<VibeUser> getFollowers(VibeUser user)
        {
            var idsOfUsers = user.Followers;

            MongoCollection<VibeUser> users = DB.GetCollection<VibeUser>("User");

            List<VibeUser> followers = new List<VibeUser>();

            foreach (var u in users.AsQueryable<VibeUser>())
            {
                if (idsOfUsers.Contains(u.Id))
                    followers.Add(u);
            }

            return followers;
        }

        public void followUser(VibeUser user)
        {
            bool needsUpdate = false;
            if (!user.Followers.Contains(User.Id))
            {
                user.Followers.Add(User.Id);
                needsUpdate = true;
            }
            if (!User.Following.Contains(user.Id))
            {
                User.Following.Add(user.Id);
                needsUpdate = true;
            }
            if (needsUpdate)
            {
                this.Update(User);
                this.Update(user);
            }
        }

        public void unfollowUser(VibeUser user)
        {
            bool needsUpdate = false;
            if (user.Followers.Contains(User.Id))
            {
                user.Followers.Remove(User.Id);
                needsUpdate = true;
            }
            if (User.Following.Contains(user.Id))
            {
                User.Following.Remove(user.Id);
                needsUpdate = true;
            }
            if (needsUpdate)
            {
                this.Update(User);
                this.Update(user);
            }
        }

        public VibeUser GetByID(ObjectId id)
        {
            MongoCollection<VibeUser> users = DB.GetCollection<VibeUser>("User");
            return users.FindOneByIdAs<VibeUser>(id);
        }

        public List<VibeUser> Search(string query)
        {
            if (null == query)
                return null;
            query = query.ToLower();
            MongoCollection<VibeUser> allUsers = DB.GetCollection<VibeUser>("User");
            var users = allUsers.AsQueryable<VibeUser>();
            List<VibeUser> listToReturn = new List<VibeUser>();
            foreach (VibeUser u in users)
            {
                if (u.FullName != null && u.FullName.ToLower().Contains(query))
                {
                    listToReturn.Add(u);
                    continue;
                }
                if (u.UserName != null && u.UserName.ToLower().Contains(query))
                {
                    listToReturn.Add(u);
                    continue;
                }
                if (u.Email != null && u.Email.ToLower().Contains(query))
                {
                    listToReturn.Add(u);
                    continue;
                }
            }
            return listToReturn;
        }
        public string SaveImage(HttpPostedFileBase image)
        {
            var path = HostingEnvironment.ApplicationPhysicalPath + "/Resources/ProfileImages/" + User.UserName + ".png";

            image.SaveAs(path);

            return "/Resources/ProfileImages/" + User.UserName + ".png";
        }
    }
}