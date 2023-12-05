using GmailCleaner.Adapters;
using GmailCleaner.Models;
using GmailCleaner.Models.Data;
using GmailCleaner.Models.ExternalModels;
using GmailCleaner.Models.Settings;
using GmailCleaner.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc.RazorPages; // Page
using System.Text.Json.Nodes; // JsonNode

namespace GmailCleaner.Pages;

public class EmailsModel : PageModel
{
    private IEmailAdapter _emailAdapter;
    private IUserContextService _contextService;

    public List<GmailCleanerEmail> Emails { get; set; } = new List<GmailCleanerEmail>();
    public ErrorModel EmailErrors { get; set; } = new();
    private int userId { get; set; }



    public EmailsModel(IEmailAdapter emailAdapter, IUserContextService contextService)
    {
        _emailAdapter = emailAdapter;
        _contextService = contextService;

    }
    public PageResult OnGet(int userId)
    {

        if (userId < 1)
        {
            RedirectToPage("Error", new { error = "User Id is required" });
        }
        else
        {
            this.userId = userId;
        }
        //accessToken = _contextService.GetToken(Request);
        //Emails = await _emailAdapter.GetEmails(accessToken, filter);
        //Response.Cookies.Append(key: "access_token", accessToken);
        return Page();
    }
    public async Task<PageResult> OnPost(string filter = "")
    {
        //string accessToken = _contextService.GetToken(Request);
        //AccessToken = accessToken;
        Emails = await _emailAdapter.GetEmails(Request, filter);
        //Response.Cookies.Append(key: "access_token", accessToken);
        return Page();
    }
}
