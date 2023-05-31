using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ExamManagementSystem.Data
{
    public class Question : EntityBase
    {
        public Question()
        {
            Exams = new List<Exam>();
            ExamToQuestions = new List<ExamToQuestion>();
            CorrectOptionIndex = GetCorrectOptionIndex();
        }

        [Required(ErrorMessage = "Question is required")]
        public string? QuestionText { get; set; }

        [Required]
        [Range(1,100, ErrorMessage = "Marks should be between 1 and 100")]
        public float Marks { get; set; }

        [NotMapped]
        public int CorrectOptionIndex { get; set; }

        private ICollection<Option> _options;
        [ValidateComplexType]
        public ICollection<Option> Options
        {
            get
            {
                return _options;
            }
            set
            {
                this._options = value;
                this.CorrectOptionIndex = GetCorrectOptionIndex();
            }
        }

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

        public ICollection<ExamToQuestion> ExamToQuestions { get; set; }
    }
}
