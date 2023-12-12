using GmailCleaner.Entities;

namespace GmailCleaner.Data.Utilities
{
    public static class UserUtil
    {
        public static bool HasValidToken(GCUser user)
        {
            if (user.GCUserTokens.Count < 1)
            {
                return false;
            }
            else
            {
                GCUserToken? token = user.GCUserTokens.OrderByDescending(x => x.ExpiresOn).FirstOrDefault();
                if (token == null)
                {
                    return false;
                }
                else
                {
                    return token.ExpiresOn > DateTime.Now;
                }
            }
        }
    }
}
