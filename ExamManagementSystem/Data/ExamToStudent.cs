using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ExamManagementSystem.Data
{
    public class ExamToStudent
    {
        [Key]
        public int Id { get; set; }

        [ForeignKey("User")]
        public string StudentId { get; set; }
        public User User { get; set; }

        [ForeignKey("Exam")]
        public int ExamId { get; set; }
        public Exam Exam { get; set; }
    }
}
