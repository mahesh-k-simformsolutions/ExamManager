using System.ComponentModel.DataAnnotations.Schema;

namespace ExamManagementSystem.Data
{
    public class ExamToQuestion : EntityBase
    {
        public ExamToQuestion()
        {

        }
        public ExamToQuestion(int questionId)
        {
            QuestionId = questionId;
        }
        public ExamToQuestion(int questionId, int examId)
        {
            QuestionId = questionId;
            ExamId = examId;
        }
        [ForeignKey("Question")]
        public int QuestionId { get; set; }
        public Question Question { get; set; }

        [ForeignKey("Exam")]
        public int ExamId { get; set; }
        public Exam Exam { get; set; }
    }
}
