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
    public class IndexModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;
        private readonly IHttpRestClient _http;

        public IndexModel(IHttpRestClient http, ILogger<IndexModel> logger)
        {
            _logger = logger;
            _http = http ?? throw new ArgumentNullException(nameof(http));
        }

        public IList<ToDoItem> Items { get; set; }

        public async Task OnGetAsync()
        {
            _logger.LogInformation("Fetching all items");

            Items = await _http.SendAsync<IList<ToDoItem>>(HttpMethod.Get, "/todo");
        }

        public async Task<IActionResult> OnPostMarkAsDone(Guid id)
        {
            _logger.LogInformation("Marking item {ID} as done", id);

            var item = await _http.SendAsync<ToDoItem>(HttpMethod.Get, $"/todo/{id}");

            item.IsDone = true;

            await _http.SendAsync(HttpMethod.Post, $"/todo/{id}", item);

            return RedirectToPage("./Index");
        }

        public async Task<IActionResult> OnPostDelete(Guid id)
        {
            _logger.LogInformation("Deleting item {ID}", id);

            await _http.SendAsync(HttpMethod.Delete, $"/todo/{id}");

            return RedirectToPage("./Index");
        }
    }
}
