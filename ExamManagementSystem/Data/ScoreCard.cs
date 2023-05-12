using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ExamManagementSystem.Data
{
    public class ScoreCard : EntityBase
    {
        [ForeignKey("Question")]
        public int QuestionId { get; set; }
        public Question Question { get; set; }

        [ForeignKey("CorrectAnswer")]
        public int CorrectAnswerId { get; set; }
        public Answer CorrectAnswer { get; set; }

        [ForeignKey("SelectedAnswer")]
        public int SelectedAnswerId { get; set; }
        public Answer SelectedAnswer { get; set; }

        public float ObtainedMarks { get; set; }
    }
}


