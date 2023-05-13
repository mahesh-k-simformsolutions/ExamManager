using System.ComponentModel.DataAnnotations.Schema;

namespace ExamManagementSystem.Data
{
    public class Answer : EntityBase
    {
        [ForeignKey("Question")]
        public int QuestionId { get; set; }
        public Question Question { get; set; }

        [ForeignKey("Option")]
        public int AnswerId { get; set; }
        public Option Option { get; set; }

        [ForeignKey("Student")]
        public string StudentId { get; set; }
        public User Student { get; set; }
    }
}
