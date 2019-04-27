using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace UploadIt.Pages
{
    public class LoginModel : PageModel
    {
        [Required]
        public string Username { get; set; }

        [Required]
        public string Password { get; set; }

        public void OnGet()
        {

        }

        public IActionResult OnPost()
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }

            return Page();
        }
    }
}