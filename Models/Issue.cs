using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;

namespace ST10296167_PROG7312_POE.Models
{
    public class Issue
    {
        public int ID { get; set; }

        [Required(ErrorMessage = "Please enter the address of the issue")]
        [MaxLength(50)]
        public string Address { get; set; }

        [Required(ErrorMessage = "Please enter the suburb of the issue")]
        [MaxLength(20)]
        public string Suburb { get; set; }

        public string? Location { get; set; }

        [Required(ErrorMessage = "Please select a category for the issue")]
        public string Category { get; set; }

        [Required(ErrorMessage = "Please enter a description for the issue")]
        [MaxLength(500)]
        public string Description { get; set; }

        // Navigation property
        public ICollection<UploadedFile>? Files{ get; set; }
    }
}
