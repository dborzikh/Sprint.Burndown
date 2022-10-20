using System.ComponentModel.DataAnnotations;

namespace Sprint.Burndown.WebApp.ViewModels
{
    public class UserViewModel
    {
        [Required]
        public string Username { get; set; }

        [Required]
        public string Password { get; set; }
    }
}
