using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Net;

namespace GmailCleaner4._0.Pages
{
    public class IndexModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;

        public IndexModel(ILogger<IndexModel> logger)
        {
            _logger = logger;
        }

        public IActionResult OnGet()
        {
            //string accessToken = Request.Cookies["access_token"] ?? string.Empty;
            //if (!string.IsNullOrEmpty(accessToken) )
            //{
            //    return RedirectToPage("Emails");
            //}
            return Page();
        }
    }
}
