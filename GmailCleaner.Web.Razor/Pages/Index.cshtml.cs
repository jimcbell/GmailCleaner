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
        private IUserContextService _contextService;

        public IndexModel(ILogger<IndexModel> logger, IUserContextService contextService)
        {
            _logger = logger;
            _contextService = contextService;
        }

        public IActionResult OnGet()
        {
            ClaimsPrincipal claimsPrincipal = User;
            if (claimsPrincipal.Identity != null && claimsPrincipal.Identity.IsAuthenticated)
            {
                GmailUserModel gmailUser = _contextService.GetUser(Request);
                string userId = _contextService.GetUserId(Request);
                return RedirectToPage("Emails", new { userId = userId.ToString() });
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
    }
}
