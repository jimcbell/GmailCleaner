using GmailCleaner.Adapters;
using GmailCleaner.Common.Models;
using GmailCleaner.Managers;
using GmailCleaner.Models.ExternalModels;
using GmailCleaner.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Net;
using System.Security.Claims;

namespace GmailCleaner.Pages
{
    public class IndexModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;
        private IIdentityAdapter _homeAdapter;
        private IConfiguration _config;
        private IEmailAdapter _emailAdapter;

        public bool IsAuthenticated { get; set; } = false;
        public int UserId { get; set; } = 0;
        public GCUser? GCUser { get; set; } = new GCUser();

        public IndexModel(ILogger<IndexModel> logger, IIdentityAdapter homeAdapter, IConfiguration config, IEmailAdapter emailAdapter)
        {
            _logger = logger;
            _homeAdapter = homeAdapter;
            _config = config;
            _emailAdapter = emailAdapter;
        }

        public async Task<IActionResult> OnGet()
        {
            ClaimsPrincipal claimsPrincipal = base.User;
            if (claimsPrincipal.Identity != null && claimsPrincipal.Identity.IsAuthenticated)
            {
                var user = await _homeAdapter.GetUser(Request);
                if (user != null)
                {
                    IsAuthenticated = true;
                    GCUser = user;
                    UserId = user.UserId;
                    return Page();
                }
                else
                {
                    return RedirectToPage("Error", new { error = "User not found" });
                }
            }
            else
            {
                return Page();
            }
            //if (!string.IsNullOrEmpty(accessToken))
            //{
            //    return RedirectToPage("Emails", accessToken, userId);
            //}
            //return Page();
        }
        public async Task<IActionResult> OnPost()
        {

        }
    }
}
