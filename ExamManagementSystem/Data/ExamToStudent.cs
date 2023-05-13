using ExamManagementSystem.Enums;
using System.ComponentModel.DataAnnotations.Schema;

namespace ExamManagementSystem.Data
{
    public class ExamToStudent : EntityBase
    {
        [ForeignKey("Student")]
        public string StudentId { get; set; }
        public User Student { get; set; }

        [ForeignKey("Exam")]
        public int ExamId { get; set; }
        public Exam Exam { get; set; }

        public EnumExamToStudentStatus ExamToStudentStatus { get; set; }
    }
}
