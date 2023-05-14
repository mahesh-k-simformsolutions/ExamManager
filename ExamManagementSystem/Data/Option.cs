using System.ComponentModel.DataAnnotations.Schema;

namespace ExamManagementSystem.Data
{
    public class Option : EntityBase
    {
        public string OptText { get; set; }
        public bool IsCorrect { get; set; }

        [ForeignKey("Question")]
        public int QuestionId { get; set; }
        public Question Question { get; set; }
    }
}
