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
    public string AccessToken { get; set; } = string.Empty;



    public EmailsModel(IEmailAdapter emailAdapter, IUserContextService contextService)
    {
        _emailAdapter = emailAdapter;
        _contextService = contextService;

    }
    public async Task<PageResult> OnGet(string filter = "")
    {
        string accessToken = _contextService.GetToken(Request);
        AccessToken = accessToken;
        Emails = await _emailAdapter.GetEmails(accessToken, filter);
        //Response.Cookies.Append(key: "access_token", accessToken);
        return Page();
    }
}
