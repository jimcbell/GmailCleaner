using GmailCleaner.Adapters;
using GmailCleaner.Models;
using GmailCleaner.Models.Data;
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

    public List<Email> Emails { get; set; } = new List<Email>();
    public ErrorModel EmailErrors { get; set; } = new();
    private string accessToken { get; set; } = string.Empty;
    private string userId { get; set; } = string.Empty;



    public EmailsModel(IEmailAdapter emailAdapter, IUserContextService contextService)
    {
        _emailAdapter = emailAdapter;
        _contextService = contextService;

    }
    public PageResult OnGet(string userId = "")
    {

        if (string.IsNullOrEmpty(userId))
        {
            throw new Exception("User Id is not valid");
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
        Emails = await _emailAdapter.GetEmails(accessToken, filter);
        //Response.Cookies.Append(key: "access_token", accessToken);
        return Page();
    }
}
