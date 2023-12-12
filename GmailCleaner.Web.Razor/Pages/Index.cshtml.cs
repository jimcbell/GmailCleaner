using Azure.Core;
using GmailCleaner.Adapters;
using GmailCleaner.Entities;
using GmailCleaner.Managers;
using GmailCleaner.Data.Gmail.Models;
using GmailCleaner.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Net;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;

namespace GmailCleaner.Pages
{
    public class IndexModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;
        private IIdentityAdapter _homeAdapter;
        private IConfiguration _config;
        private IMessageManager _messageManager;

        public bool IsAuthenticated { get; set; } = false;
        public string? Name { get; set; }
        public int Usages { get; set; } = 0;
        public string ErrorMessage { get; set; } = string.Empty;
        private string _gmailId = string.Empty;

        public IndexModel(ILogger<IndexModel> logger, 
            IIdentityAdapter homeAdapter, 
            IConfiguration config, 
            IMessageManager messageManager)
        {
            _logger = logger;
            _homeAdapter = homeAdapter;
            _config = config;
            _messageManager = messageManager;
        }

        public async Task<IActionResult> OnGet()
        {
            if (isUserAuthenticated())
            {
                GCUser? user = await _homeAdapter.GetUser(Request);

                if (user != null)
                {
                    IsAuthenticated = true;
                    Name = user.Name;
                    Usages = user.Usages;
                    if(user.GCMessages.Count >0)
                    {
                        return RedirectToPage("Messages");
                    }
                }
                else
                {
                    await Request.HttpContext.SignOutAsync();
                    // The user was removed from the database but they still have the cookie, need to re-add them.
                    return RedirectToPage("Error");
                }
            }
            return Page();
        }
        public async Task<IActionResult> OnPost()
        {
            GCUser? user = await _homeAdapter.GetUser(Request);
            if (!isUserAuthenticated() || user ==null)
            {
                return RedirectToPage("Error", new { error = "User not found" });
            }
            if (!(user.GCMessages.Count > 0)) // Only load messages if they haven't been loaded yet
            {
                try
                {

                    List<GCMessage>? messages = await _messageManager.LoadMessagesAsync(user!);
                    if (messages == null) // Transient error getting messages
                    {
                        ErrorMessage = "Too many requests to Gmail. Please try again in a few minutes.";
                        return Page();
                    }
                }
                catch
                {
                    return RedirectToPage("Error", new { error = "Error loading messages. Please contact jimcampbell355@gmail.com to remediate account issues." });
                }
            }
            return RedirectToPage("Messages");
        }
        private bool isUserAuthenticated()
        {
            ClaimsPrincipal claimsPrincipal = base.User;
            if (claimsPrincipal.Identity != null && claimsPrincipal.Identity.IsAuthenticated)
            {
                _gmailId = Request.HttpContext.User.Claims.Where(c => c.Type == "id").Select(c => c.Value).FirstOrDefault() ?? string.Empty;
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
