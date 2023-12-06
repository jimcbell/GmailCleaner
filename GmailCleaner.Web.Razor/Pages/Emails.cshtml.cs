using GmailCleaner.Adapters;
using GmailCleaner.Models;
using GmailCleaner.Models.Data;
using GmailCleaner.Models.ExternalModels;
using GmailCleaner.Models.Settings;
using GmailCleaner.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages; // Page
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Nodes; // JsonNode

namespace GmailCleaner.Pages;

[ResponseCache(Duration = 200 /* seconds */, Location = ResponseCacheLocation.Any)]
public class EmailsModel : PageModel
{
    private IEmailAdapter _emailAdapter;
    private IUserContextService _contextService;

    [BindProperty]
    [Range(0, 25)]
    public int NumberEmails { get; set; } = 5;

    public List<Email> Emails { get; set; } = new List<Email>();
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
        if (ModelState.IsValid)
        {
            Console.Write("Model is valid");
        }
        else {
            NumberEmails = 25;
        }
        //string accessToken = _contextService.GetToken(Request);
        //AccessToken = accessToken;
        Emails = await _emailAdapter.GetEmails(Request, NumberEmails);
        //Response.Cookies.Append(key: "access_token", accessToken);
        return Page();
    }
}
