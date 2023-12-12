using GmailCleaner.Adapters;
using GmailCleaner.Entities;
using GmailCleaner.Managers;
using GmailCleaner.Models;
using GmailCleaner.Data.Gmail.Models;
using GmailCleaner.Data.Settings;
using GmailCleaner.Data.ViewModels;
using GmailCleaner.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages; // Page
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Nodes;
using System.Web; // JsonNode

namespace GmailCleaner.Pages;

public class MessagesModel : PageModel
{
    private IMessageAdapter _messageAdapter;
    private IIdentityAdapter _identityAdapter;

    public List<GCMessage> Messages { get; set; } = new List<GCMessage>();
    public ErrorModel EmailErrors { get; set; } = new();
    private GCUser? user { get; set; } = null;
    public List<Tuple<string, bool>> DeleteList { get; set; } = new();
    public Dictionary<string, int> FromCounts { get; set; } = new();
    public List<MessageViewModel> MessageViewModels { get; set; } = new();
    public int DeleteCount { get; set; } = 0;   
    public string ErrorMessage { get; set; } = string.Empty;    



    public MessagesModel(IMessageAdapter messageAdapter, IUserContextService contextService, IIdentityAdapter identityAdapter)
    {
        _messageAdapter = messageAdapter;
        _identityAdapter = identityAdapter;

    }
    public async Task<IActionResult> OnGet()
    {
        user = await _identityAdapter.GetUser(Request);
        if (user == null)
        {
            return RedirectToPage("Error", new { error = "User Id is required" });
        }
        else
        {
            await initializePage(user);
            
            return Page();
        }
    }

    public async Task<IActionResult> OnPost([FromForm] Dictionary<string, int> deleteList)
    {
        user = await _identityAdapter.GetUser(Request);
        if (user != null)
        {
            await initializePage(user);
            List<GCMessage> messagesToDelete =  _messageAdapter.GetMessagesToDeleteFromList(deleteList, Messages);
            DeleteCount = await _messageAdapter.TrashMessagesAsync(messagesToDelete, user);
            // Reinitialize the page so the deleted messages are gone.
            await initializePage(user);
        }
        return Page();
    }
    public async Task<IActionResult> OnPostDeleteAll()
    {
        user = await _identityAdapter.GetUser(Request);
        if (user != null)
        {
            await initializePage(user);
            List<GCMessage> messagesToDelete = Messages;
            DeleteCount = await _messageAdapter.DeleteMessagesAsync(user);
            // Reinitialize the page so the deleted messages are gone.
            await initializePage(user);
            if(user.GCMessages.Count == 0)
            {
                return RedirectToPage("Index");
            }
            else
            {
                ErrorMessage = "Error deleting messages from GmailCleaner. Please try again, if the issue persists, please reach out to jimcampbell355@gmail.com";
            }
            return Page();
        }
        else
        {
            return RedirectToPage("Error", new { error = "User Id is required" });
        }
    }
    private async Task initializePage(GCUser user)
    {
        Messages = await _messageAdapter.GetMessagesAsync(user);
        MessageViewModels = _messageAdapter.GetMessageViewModels(Messages);        
    }
    
}
