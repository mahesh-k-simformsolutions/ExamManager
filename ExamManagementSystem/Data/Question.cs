using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ExamManagementSystem.Data
{
    public class Question
    {
        [Key]
        public int QuestionId { get; set; }
        public string QuestionText { get; set; }
        public int Marks { get; set; }

        [ForeignKey("Exam")]
        public int ExamId { get;set; }
        public Exam Exam { get; set; }

        public ICollection<Option> Options { get; set; }

        /// <summary>
        /// Subjective / Objective
        /// </summary>
        public string QueType { get; set; }
    }
}
