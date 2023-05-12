using System.ComponentModel.DataAnnotations;

namespace ExamManagementSystem.Data
{
    public class EntityBase
    {
        [Key]
        public int Id { get; set; }
    }
}
