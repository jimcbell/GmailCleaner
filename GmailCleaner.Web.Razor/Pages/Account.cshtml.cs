using GmailCleaner.Adapters;
using GmailCleaner.Entities;
using GmailCleaner.Managers;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace GmailCleaner.Pages
{
    
    public class AccountModel : PageModel
    {
        public GCUser GcUser { get; set; } = new GCUser();
        public List<GCMessage> GcMessages { get; set; } = new List<GCMessage>();

        private IIdentityAdapter _identityAdapter;
        private IMessageManager _messageManager;

        public AccountModel(IIdentityAdapter identityAdapter, IMessageManager messageManager)
        {
            _identityAdapter = identityAdapter;
            _messageManager = messageManager;
        }
        public async Task OnGetAsync()
        {
            var user = await _identityAdapter.GetUser(Request);
            if (user != null)
            {
                GcUser = user;
                GcMessages = await _messageManager.GetMessagesAsync(user);
            }
        }
    }
}
