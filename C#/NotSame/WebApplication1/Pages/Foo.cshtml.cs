using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace WebApplication1.Pages
{
    public class FooModel : PageModel
    {
        private readonly ILogger<FooModel> _logger;

        public FooModel(ILogger<FooModel> logger)
        {
            _logger = logger;
        }

        public void OnGet()
        {
        }
    }
}