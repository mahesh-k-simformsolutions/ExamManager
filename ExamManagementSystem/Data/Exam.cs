using ExamManagementSystem.Enums;
using System.ComponentModel.DataAnnotations.Schema;

namespace ExamManagementSystem.Data
{
    public class Exam : EntityBase
    {
        /// <summary>
        /// Duration in minutes
        /// </summary>
        public string Duration => (EndTime.TimeOfDay - StartTime.TimeOfDay).TotalMinutes.ToString();

        /// <summary>
        /// 00:00:00 to 23:59:59
        /// </summary>
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }

        public DateTime Date { get; set; }

        public EnumExamStatus ExamStatus { get; set; } = EnumExamStatus.NotStarted;

        [ForeignKey("Teacher")]
        public string? TeacherId { get; set; }
        public User? Teacher { get; set; }

        public string ExamCode { get; set; } = Helpers.Helpers.GenerateCode();
        public string ExamName { get; set; }

        [NotMapped]
        public bool IsAppearedByCurrentStudent { get; set; }

        [NotMapped]
        public ICollection<Question> Questions { get; set; }

        public ICollection<ExamResult> Results { get; set; }
    }

}
