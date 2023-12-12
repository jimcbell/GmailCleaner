using GmailCleaner.Common.Models;
using GmailCleaner.Managers;
using GmailCleaner.Models.Data;
using GmailCleaner.Models.ExternalModels;
using GmailCleaner.Models.Settings;
using GmailCleaner.Models.ViewModels;
using GmailCleaner.Services;
using System.Runtime;

namespace GmailCleaner.Adapters
{
    /// <summary>
    /// Message Adapter is the interface between the front end and the meesage manager to do all the work for the emails page.
    /// </summary>
    public interface IMessageAdapter
    {
        ///// <summary>
        ///// Gets emails from the email manager.
        ///// If there is a null response that means there was a transient error and it will return an empty list.
        ///// </summary>
        ///// <param name="request"></param>
        ///// <param name="numberEmails"></param>
        ///// <param name="filter"></param>
        ///// <returns></returns>
        ///// <exception cref="Exception">Throws an exception on a permanent api error (automatically) - should redirect to error page.</exception>"
        //public Task<List<Email>?> GetEmails(GCUser user, int numberEmails, string filter = "");

        public Task<List<GCMessage>> GetMessagesAsync(GCUser user);
        public List<MessageViewModel> GetMessageViewModels(List<GCMessage> messages);
        public List<GCMessage> GetMessagesToDeleteFromList(Dictionary<string, int> deleteList, List<GCMessage> messages);
        /// <summary>
        /// Calls the message manager to trash the emails in gmail and then deletes them from the database.
        /// </summary>
        /// <param name="messages"></param>
        /// <param name="user"></param>
        /// <returns></returns>
        public Task<int> TrashMessagesAsync(List<GCMessage> messages, GCUser user);
        /// <summary>
        /// Used to clean all user related message data.
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public Task<int> DeleteMessagesAsync(GCUser user);
    }
    public class MessageAdapter : IMessageAdapter
    {
        private IUserContextService _userContextService;
        private IAccessTokenManager _accessManager;
        private IUserManager _userManager;
        private IMessageManager _messageManager;

        public MessageAdapter( 
            IUserContextService contextService, 
            IAccessTokenManager accessManager, 
            IUserManager userManager,
            IMessageManager messageManager)
        {
            _userContextService = contextService;
            _accessManager = accessManager;
            _userManager = userManager;
            _messageManager = messageManager;
        }

        public async Task<int> TrashMessagesAsync(List<GCMessage> messages, GCUser user)
        {
            
            return await _messageManager.TrashMessagesAsync(messages, user);
        }

        public async Task<int> DeleteMessagesAsync(GCUser user)
        {
            return await _messageManager.DeleteAllMessagesAsync(user);
        }

        //public async Task<List<Email>?> GetEmails(GCUser user, int numberEmails, string filter = "")
        //{

        //    List<Email> emails = new List<Email>();

        //    string accessToken = await _accessManager.GetAccessTokenAsync(user.UserId);
        //    _emailManager.LoadAccessToken(accessToken);
        //    // This will throw an exception if there is a permanent error on google api, letting that be auto rethrown.
        //    EmailMetadatas? emailMetadatas = await _emailManager.GetEmailMetadatas(filter, maxEmails: numberEmails);
        //    // If there is a transient error, it will return null, so we need to check for that.
        //    if(emailMetadatas != null)
        //    {
        //        List<string> emailIds = emailMetadatas.Metadatas.Select(e => e.Id).ToList();
        //        emails = await _emailManager.GetEmails(emailIds, user.GmailId) ?? new();
        //    }
        //    return emails;
        //}

        public async Task<List<GCMessage>> GetMessagesAsync(GCUser user)
        {
            return await _messageManager.GetMessagesAsync(user);
        }

        public List<GCMessage> GetMessagesToDeleteFromList(Dictionary<string, int> deleteList, List<GCMessage> messages)
        {
            List<string> linkingMessageIds = deleteList.Select(m => m.Key).ToList();
            List<string> deleteMessagesFrom = new();
            foreach (string linkingMessageId in linkingMessageIds)
            {
                string? from = messages.Where(m => m.MessageGmailId == linkingMessageId).Select(m => m.From).FirstOrDefault();
                if( !string.IsNullOrEmpty(from))
                {
                    deleteMessagesFrom.Add(from);
                }
            }
            List<GCMessage> deleteMessages = messages.Where(m => deleteMessagesFrom.Contains(m.From)).ToList();
            return deleteMessages;
        }

        public List<MessageViewModel> GetMessageViewModels(List<GCMessage> messages)
        {
            List<string> messageFroms = messages.Select(m => m.From).Distinct().ToList();
            List<MessageViewModel> messageViewModels = new();
            // Probably a better way to do this with linq but i am doing it like this as I have spent too much time on this front end issue.
            foreach (string sender in messageFroms)
            {
                messageViewModels.Add(
                    new MessageViewModel()
                    {
                        From = sender,
                        LinkingMessageId = messages.Where(m => m.From == sender).Select(m => m.MessageGmailId).First(),
                        MessageGmailIds = messages.Where(m => m.From == sender).Select(m => m.MessageGmailId).ToList(),
                        Delete = false
                    });
            }
            return messageViewModels;
            // Old way I was doing it of just taking the froms. This was getting written wrong in the html, so now I need to use a message id.
            //FromCounts = Messages.GroupBy(m => m.From)
            //        .Select(group => new KeyValuePair<string, int>(group.Key, group.Count()))
            //        .ToDictionary();
        }

        //private async Task<GCUser?> getUser(HttpRequest request)
        //{
        //    GmailUserModel model = _userContextService.GetUser(request);
        //    return await _userManager.GetUserAsync(model.UserId);

        //}
    }
}
