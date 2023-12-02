using GmailCleaner.Adapters;
using GmailCleaner.Models.ExternalModels;
using GmailCleaner.Repositories;
using GmailCleaner.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace GmailCleaner.Pages
{
    public class LoginSuccessModel : PageModel
    {
        private IUserContextService _contextService;
        private ILoginAdapter _loginAdapter;

        public LoginSuccessModel(IUserContextService contextService, ILoginAdapter loginAdapter)
        {
            _contextService = contextService;
            _loginAdapter = loginAdapter;
        }
        public async Task OnGet()
        {
            GmailToken gmailToken = _contextService.GetToken(Request);
            GmailUserModel gmailUser = _contextService.GetUser(Request);
            int userId = await _loginAdapter.AddUser(gmailUser, gmailToken);
            ////string useriId = _contextService.GetUserId(Request);
            //Response.Cookies.Append("access_token", accessToken);
            //Response.Cookies.Append("user_id", userId);
            RedirectToPage("Emails", new { userId = userId});
        }
    }
}
