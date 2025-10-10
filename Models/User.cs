using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace ST10296167_PROG7312_POE.Models
{
    public class User : IdentityUser
    {
        [Required]
        [StringLength(50)]
        public string FirstName { get; set; } = string.Empty;

        [Required]
        [StringLength(50)]
        public string LastName { get; set; } = string.Empty;

        public string FullName => $"{FirstName} {LastName}";
    }
}
//--------------------------------------------------------X END OF FILE X-------------------------------------------------------------------//