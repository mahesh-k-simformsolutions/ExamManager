using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ExamManagementSystem.Data
{
    public class Question : EntityBase
    {
        public Question()
        {
            Exams = new List<Exam>();
        }
        public string? QuestionText { get; set; }
        public float Marks { get; set; }

        public int CorrectOptionId { get; set; }

        public ICollection<Option> Options { get; set; }

        /// <summary>
        /// Exams in which this question is included
        /// </summary>
        [NotMapped]
        public List<Exam> Exams { get; set; }
    }
}
