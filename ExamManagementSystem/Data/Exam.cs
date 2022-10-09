using System.ComponentModel.DataAnnotations.Schema;

namespace ExamManagementSystem.Data
{
    public class Exam
    {
        public int Id { get; set; }

        public string Duration { get { return (EndTime.TimeOfDay - StartTime.TimeOfDay).TotalMinutes.ToString(); } }

        /// <summary>
        /// 00:00:00 to 23:59:59
        /// </summary>
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }

        public DateTime Date { get; set; }

        [ForeignKey("User")]
        public string TeacherId { get; set; }
        public User User { get; set; }

        public ICollection<Question> Questions { get; set; }
    }
}
