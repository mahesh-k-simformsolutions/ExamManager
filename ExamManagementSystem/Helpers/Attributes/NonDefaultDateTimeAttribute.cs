using System.ComponentModel.DataAnnotations;

namespace ExamManagementSystem.Helpers.Attributes
{
    public class NonDefaultDateTimeAttribute : ValidationAttribute
    {
        private readonly string _fieldDisplayName;

        public NonDefaultDateTimeAttribute(string fieldDisplayName)
        {
            _fieldDisplayName = fieldDisplayName;
        }

        public override bool IsValid(object? value)
        {
            if (value is null)
            {
                return true;
            }

            if (value is DateTime dateTime)
            {
                return dateTime != default;
            }

            return true;
        }

        public override string FormatErrorMessage(string name)
        {
            return $"{_fieldDisplayName ?? name} is required.";
        }
    }
}
