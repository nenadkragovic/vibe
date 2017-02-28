using MongoDB.Bson;
using System.Collections.Generic;
using System.IO;
using Vibe.Models;

namespace Vibe.Services
{
    public interface IUsersService
    {
        List<VibeUser> GetAllUsers();
        VibeUser GetUserByUsername(string username);

        VibeUser Login(string username, string password);

        bool Register(VibeUser user);

        bool Update(VibeUser user);

        List<VibeUser> getFollowers(VibeUser user);

        List<VibeUser> getFollowing(VibeUser user);

        void followUser(VibeUser user);

        void unfollowUser(VibeUser user);

        VibeUser GetByID(ObjectId id);

        List<VibeUser> Search(string query);

    }
}
