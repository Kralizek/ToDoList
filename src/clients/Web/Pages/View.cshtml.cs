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
    public class ViewModel : PageModel
    {
        private readonly IHttpRestClient _http;
        private readonly ILogger<ViewModel> _logger;

        public ViewModel(IHttpRestClient http, ILogger<ViewModel> logger)
        {
            _http = http ?? throw new ArgumentNullException(nameof(http));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        [BindProperty]
        public ToDoItem Item { get; set; }
        
        public async Task OnGetAsync(Guid id)
        {
            _logger.LogInformation("Fetching item {ID}", id);

            Item = await _http.SendAsync<ToDoItem>(HttpMethod.Get, $"/todo/{id}");

            ViewData["Title"] = $"{Item.Title} - ToDo";
        }

        public async Task<IActionResult> OnPostAsync(Guid id)
        {
            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Model state is not valid");

                return Page();
            }

            _logger.LogInformation("Updating item {ID}", id);

            Item.Id = id;

            await _http.SendAsync(HttpMethod.Post, $"/todo/{id}", Item);

            return RedirectToPage("./Index");
        }
    }
}
