using GmailCleaner.Common;
using GmailCleaner.Common.Models;
using Microsoft.EntityFrameworkCore;

namespace GmailCleaner.Repositories
{
    public interface IUserRepository
    {
        public Task<GCUser> UpsertUserAsync(GCUser user);
        public Task<GCUser?> GetUserAsync(int userId);
        public Task<bool> DeleteUserAsync(int userId);
        public Task<int> GetUserIdAsync(string gmailId);
    }
    public class UserRepository : IUserRepository
    {
        private GmailCleanerContext _context;

        public UserRepository(GmailCleanerContext context)
        {
            _context = context;
        }
        /// <summary>
        /// Used to replace an expired token on a user with an updated token
        /// </summary>
        /// <param name="user"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        public async Task<GCUser> UpsertTokenAsync(GCUser user, GCUserToken token)
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
        /// <summary>
        /// Get a user by their ID
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public async Task<GCUser?> GetUserAsync(int userId)
        {

            GCUser? user = await _context.GCUsers.Where(u => u.UserId == userId).Include(x => x.GCUserTokens).Include(x=> x.GCMessages).FirstOrDefaultAsync();
            return user;
        }
        /// <summary>
        /// Add a new user to the database
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public async Task<GCUser> UpsertUserAsync(GCUser user)
        {

            GCUser? existingUser = await _context.GCUsers.Include(u => u.GCUserTokens).Where(u => u.GmailId == user.GmailId).FirstOrDefaultAsync();
            if (existingUser == null)
            {
                existingUser = user;
                _context.GCUsers.Add(existingUser);
            }
            else
            {
                existingUser.Name = user.Name;
                existingUser.Email = user.Email;
            }
            await _context.SaveChangesAsync();
            return existingUser;
        }
        public Task<bool> DeleteUserAsync(int userId)
        {
            throw new NotImplementedException();
        }

        public async Task<int> GetUserIdAsync(string gmailId)
        {

            return await _context.GCUsers.Where(u => u.GmailId == gmailId).Select(u => u.UserId).FirstOrDefaultAsync();
        }
    }
}
