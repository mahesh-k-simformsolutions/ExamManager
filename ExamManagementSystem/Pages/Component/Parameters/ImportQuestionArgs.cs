using ExamManagementSystem.Data;
using Microsoft.AspNetCore.Components;

namespace ExamManagementSystem.Pages.Component.Parameters
{
    public class ImportQuestionArgs
    {
        public Exam exam { get; set; }
        public EventCallback callback { get; set; }
    }
}
