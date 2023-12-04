using GmailCleaner.Adapters;
using GmailCleaner.Common.Models;
using GmailCleaner.Models.ExternalModels;
using GmailCleaner.Repositories;
using GmailCleaner.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace GmailCleaner.Pages;

public class LoginSuccessModel : PageModel
{
    private IUserContextService _contextService;
    private ILoginAdapter _loginAdapter;

    public LoginSuccessModel(IUserContextService contextService, ILoginAdapter loginAdapter)
    {
        _contextService = contextService;
        _loginAdapter = loginAdapter;
    }
    public async Task<IActionResult> OnGet()
    {
        GCUser user = await _loginAdapter.LogInUser(Request);
        ////string useriId = _contextService.GetUserId(Request);
        //Response.Cookies.Append("access_token", accessToken);
        //Response.Cookies.Append("user_id", userId);
        return RedirectToPage("Index");
    }
}
