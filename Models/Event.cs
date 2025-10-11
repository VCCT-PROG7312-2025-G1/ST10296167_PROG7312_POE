using System.ComponentModel.DataAnnotations;

namespace ST10296167_PROG7312_POE.Models
{
    public class Event
    {
        [Key]
        public int ID { get; set; }

        [Required(ErrorMessage = "Please enter a title")]
        [MaxLength(30)]
        public string Title { get; set; }

        [Required(ErrorMessage = "Please enter a description")]
        [MaxLength(500)]
        public string Description { get; set; }

        [Required(ErrorMessage = "Please select a category")]
        public string Category { get; set; }

        [Required(ErrorMessage = "Please select a date")]
        public DateOnly Date { get; set; }

        [Required(ErrorMessage = "Please select a time")]
        public TimeOnly Time { get; set; }

        [Required(ErrorMessage = "Please enter a location")]
        [MaxLength(50)]
        public string Location { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }
}
