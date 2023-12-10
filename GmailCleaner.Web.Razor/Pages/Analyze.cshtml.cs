using GmailCleaner.Adapters;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace GmailCleaner.Pages
{
    public class AnalyzeModel : PageModel
    {
        private IIdentityAdapter _identityAdapter;

        public AnalyzeModel(IIdentityAdapter identityAdapter)
        {
            _identityAdapter = identityAdapter;
        }
        public void OnGet()
        {
        }
    }
}
