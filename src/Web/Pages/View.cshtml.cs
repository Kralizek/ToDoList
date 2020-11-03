using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Kralizek.Extensions.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Web.Pages
{
    public class ViewModel : PageModel
    {
        private readonly IHttpRestClient _http;

        public ViewModel(IHttpRestClient http)
        {
            _http = http ?? throw new ArgumentNullException(nameof(http));
        }

        [BindProperty]
        public ToDoItem Item { get; set; }
        
        public async Task OnGetAsync(Guid id)
        {
            Item = await _http.SendAsync<ToDoItem>(HttpMethod.Get, $"/todo/{id}");

            ViewData["Title"] = $"{Item.Title} - ToDo";
        }

        public async Task<IActionResult> OnPostAsync(Guid id)
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            Item.Id = id;

            await _http.SendAsync(HttpMethod.Post, $"/todo/{id}", Item);

            return RedirectToPage("./Index");
        }
    }
}
