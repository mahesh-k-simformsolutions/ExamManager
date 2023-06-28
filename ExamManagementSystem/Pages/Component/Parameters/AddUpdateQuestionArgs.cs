using ExamManagementSystem.Data;
using Microsoft.AspNetCore.Components;

namespace ExamManagementSystem.Pages.Component.Parameters
{
    public class AddUpdateQuestionArgs
    {
        public Question Question { get; set; }
        public Exam Exam { get; set; }
        public EventCallback Callback { get; set; }
    }
}
