using System.ComponentModel.DataAnnotations;

namespace ST10296167_PROG7312_POE.Models
{
    public class Announcement
    {
        [Key]
        public int ID { get; set; }

        [Required(ErrorMessage = "Please enter a title")]
        [MaxLength(30)]
        public string Title { get; set; }

        [Required(ErrorMessage = "Please enter content for the this announcement")]
        [MaxLength(100)]
        public string Content { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }
}
