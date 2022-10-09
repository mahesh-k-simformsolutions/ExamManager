using System.ComponentModel.DataAnnotations.Schema;

namespace ExamManagementSystem.Data
{
    public class ExamResult
    {
        public int Id { get; set; }

        [ForeignKey("User")]
        public string StudentId { get; set; }
        public User User { get; set; }  

        public int ExamId { get; set; }
        public float Score { get; set; }

        /// <summary>
        /// Pass / Fail
        /// </summary>
        public string Status { get; set; }

    }
}
