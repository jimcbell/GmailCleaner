using GmailCleaner.Adapters;
using GmailCleaner.Common.Models;
using GmailCleaner.Managers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace GmailCleaner.Pages
{
    public class MessageDetailModel : PageModel
    {
        private IMessageManager _messageManager;
        private IIdentityAdapter _identityAdapter;

        public List<GCMessage> Messages { get; set; } = new List<GCMessage>();
        public MessageDetailModel(IIdentityAdapter identityAdapter, IMessageManager messageManager)
        {
            _messageManager = messageManager;
            _identityAdapter = identityAdapter;
        }
        public async Task<IActionResult> OnGetAsync()
        {
            var user = await _identityAdapter.GetUser(Request);
            if (user == null)
            {
                return RedirectToPage("Index");
            }
            else
            {
                Messages = await _messageManager.GetMessagesAsync(user);
            }
            return Page();
        }
    }
}
