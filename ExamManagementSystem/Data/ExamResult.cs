using ExamManagementSystem.Enums;
using System.ComponentModel.DataAnnotations.Schema;

namespace ExamManagementSystem.Data
{
    public class ExamResult : EntityBase
    {
        [ForeignKey("Student")]
        public string StudentId { get; set; }
        public User Student { get; set; }

        [ForeignKey("Exam")]
        public int ExamId { get; set; }
        public Exam Exam { get; set; }

        public float Score { get; set; }

        /// <summary>
        /// Pass / Fail
        /// </summary>
        public ExamResultStatus Status { get; set; }

    }
}


