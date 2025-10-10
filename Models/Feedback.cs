using System.ComponentModel.DataAnnotations;

namespace ST10296167_PROG7312_POE.Models
{
    public class Feedback
    {
        [Key]
        public int ID { get; set; }

        [Required]
        [Range(1, 5)]
        public int Rating { get; set; } // Rating out of 5

        public string? OptionalFeedback { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }
}
//--------------------------------------------------------X END OF FILE X-------------------------------------------------------------------//