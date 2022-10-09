using System.ComponentModel.DataAnnotations.Schema;

namespace ExamManagementSystem.Data
{
    public class Answer
    {
        public int Id { get; set; }

        [ForeignKey("Question")]
        public int QuestionId { get; set; }
        public Question Question { get; set; }

        [ForeignKey("Option")]
        public int AnswerId { get; set; }
        public Option Option { get; set; }
    }
}
