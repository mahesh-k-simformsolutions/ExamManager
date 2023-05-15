using System.ComponentModel.DataAnnotations.Schema;

namespace ExamManagementSystem.Data
{
    public class Question : EntityBase
    {
        public Question()
        {
            Exams = new List<Exam>();
            CorrectOptionIndex = GetCorrectOptionIndex();
        }
        public string? QuestionText { get; set; }
        public float Marks { get; set; }

        [NotMapped]
        public int CorrectOptionIndex { get; set; }

        public ICollection<Option> Options { get; set; }

        /// <summary>
        /// Exams in which this question is included
        /// </summary>
        [NotMapped]
        public List<Exam> Exams { get; set; }

        private int GetCorrectOptionIndex()
        {
            if (Options != null && Options.Count > 0)
            {
                Option? correct = Options.FirstOrDefault(x => x.IsCorrect);
                return correct != null ? Options.ToList().IndexOf(correct) : -1;
            }
            return -1;
        }
    }
}
