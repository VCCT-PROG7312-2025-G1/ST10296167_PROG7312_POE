using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ST10296167_PROG7312_POE.Models
{
    public class UploadedFile
    {
        [Key]
        public int ID { get; set; }

        [Required]
        public string FileName { get; set; }

        [Required]
        public string MimeType { get; set; }

        public long Size { get; set; }

        [Required]
        public string FilePath { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        // FK
        [ForeignKey("Issue")]
        public int IssueID { get; set; }
        public virtual Issue Issue { get; set; }
    }
}
//--------------------------------------------------------X END OF FILE X-------------------------------------------------------------------//