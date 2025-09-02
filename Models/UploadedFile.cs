using System.ComponentModel.DataAnnotations;

namespace ST10296167_PROG7312_POE.Models
{
    public class UploadedFile
    {
        public int ID { get; set; }

        public string FileName { get; set; }
        public string MimeType { get; set; }
        public long Size { get; set; }
        public string FilePath { get; set; }

        // FK
        public int IssueID { get; set; }
        public Issue Issue { get; set; }
    }
}
