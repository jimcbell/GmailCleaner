using GmailCleaner.Common;
using GmailCleaner.Common.Models;
using Microsoft.EntityFrameworkCore;

namespace GmailCleaner.Repositories
{
    public interface IUserRepository
    {
        public Task<GCUser> UpsertUserAsync(GCUser user);
        public Task<GCUser> GetUserAsync(int userId);
        public Task<GCUser> UpsertTokenAsync(GCUser user, GCUserToken token);

    }
    public class UserRepository : IUserRepository
    {
        private GmailCleanerContext _context;

        public UserRepository(GmailCleanerContext context)
        {
            _context = context;
        }
        /// <summary>
        /// Add a new user to the database
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public async Task<GCUser> UpsertUserAsync(GCUser user)
        {
            if (user.GCUserTokens.Count < 1)
            {
                throw new Exception("User must have at least one token");
            }
            using (_context)
            {
                GCUser? existingUser = await _context.GCUsers.Where(u => u.GmailId == user.GmailId).FirstOrDefaultAsync();
                if (existingUser != null)
                {
                    existingUser.Name = user.Name;
                    existingUser.Email = user.Email;
                    await UpsertTokenAsync(existingUser, user.GCUserTokens.First());
                }
                else
                {
                    existingUser = user;
                    _context.GCUsers.Add(existingUser);
                }
                await _context.SaveChangesAsync();
                return existingUser;
            }
        }
        /// <summary>
        /// Get a user by their ID
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public async Task<GCUser> GetUserAsync(int userId)
        {
            using (_context)
            {
                GCUser? user = await _context.GCUsers.Where(u => u.UserId == userId).Include(x => x.GCUserTokens).FirstOrDefaultAsync();
                if (user == null)
                {
                    throw new Exception("User not found");
                }
                else
                {
                    return user;
                }
            }
        }
        /// <summary>
        /// Used to replace an expired token on a user with an updated token
        /// </summary>
        /// <param name="user"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        public async Task<GCUser> UpsertTokenAsync(GCUser user, GCUserToken token)
        {
            using (_context)
            {
                GCUserToken? existingToken = await _context.GCUserTokens.Where(t => t.UserId == user.UserId).FirstOrDefaultAsync();
                if (existingToken == null)
                {
                    user.GCUserTokens.Add(token);
                }
                else
                {
                    existingToken.AccessToken = token.AccessToken;
                    existingToken.ExpiresOn = token.ExpiresOn;
                    existingToken.IdToken = token.IdToken;
                    existingToken.RefreshToken = token.RefreshToken;
                }
                await _context.SaveChangesAsync();
                return user;
            }
        }
    }
}
