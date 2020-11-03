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
    public class AddModel : PageModel
    {
        private readonly IHttpRestClient _http;

        public AddModel(IHttpRestClient http)
        {
            _http = http ?? throw new ArgumentNullException(nameof(http));
        }

        [BindProperty]
        public ToDoItem Item { get; set; }

        public void OnGet()
        {
            Item = new ToDoItem();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            await _http.SendAsync(HttpMethod.Post, "/todo", Item);

            return RedirectToPage("./Index");
        }
    }
}
