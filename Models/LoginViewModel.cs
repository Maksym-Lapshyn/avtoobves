using System.ComponentModel.DataAnnotations;

namespace Avtoobves.Models
{
    public class LoginViewModel
    {
        [Required(ErrorMessage = "Enter username!")]
        public string UserName { get; set; }

        [Required(ErrorMessage = "Enter password!")]
        public string Password { get; set; }
    }
}