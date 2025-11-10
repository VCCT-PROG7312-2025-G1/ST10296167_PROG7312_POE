using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ST10296167_PROG7312_POE.Models
{
    public class Issue
    {
        [Key]
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

        public IssueStatus Status { get; set; } = IssueStatus.Submitted;

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        // EF Core navigation property
        public virtual ICollection<UploadedFile> DbFiles { get; set; } = new List<UploadedFile>();

        // In-memory data structure (not mapped to database)
        [NotMapped]
        public LinkedList<UploadedFile>? Files { get; set; } = new LinkedList<UploadedFile>();
    }

    public enum IssueStatus
    {
        Submitted = 2,
        InProgress = 3,
        Resolved = 1,
    }
}
//--------------------------------------------------------X END OF FILE X-------------------------------------------------------------------//