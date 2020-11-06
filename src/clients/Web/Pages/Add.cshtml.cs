using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Kralizek.Extensions.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;

namespace Web.Pages 
{
    public class AddModel : PageModel 
    {
        private readonly IHttpRestClient _http;
        private readonly ILogger<AddModel> _logger;

        public AddModel (IHttpRestClient http, ILogger<AddModel> logger) 
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _http = http ?? throw new ArgumentNullException (nameof (http));
        }

        [BindProperty]
        public ToDoItem Item { get; set; }

        public void OnGet () 
        {
            Item = new ToDoItem ();
        }

        public async Task<IActionResult> OnPostAsync () 
        {
            if (!ModelState.IsValid) 
            {
                _logger.LogWarning("Model state is not valid");

                return Page ();
            }

            _logger.LogInformation("Creating new item");

            await _http.SendAsync (HttpMethod.Post, "/todo", Item);

            return RedirectToPage ("./Index");
        }
    }
}