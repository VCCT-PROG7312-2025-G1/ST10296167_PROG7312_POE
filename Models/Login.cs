using System.ComponentModel.DataAnnotations;

namespace ST10296167_PROG7312_POE.Models
{
    public class Login
    {
        [Required(ErrorMessage = "Please enter a email")]
        [EmailAddress(ErrorMessage = "Please enter a valid email address")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Please enter a password")]
        public string Password { get; set; } = string.Empty;
    }
}
//--------------------------------------------------------X END OF FILE X-------------------------------------------------------------------//