using System.ComponentModel.DataAnnotations;

namespace ExamManagementSystem.Helpers.Attributes
{
    public class EndTimeGreaterThanStartTimeAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var startTimeProperty = validationContext.ObjectType.GetProperty("StartTime");
            var startTimeValue = (DateTime)startTimeProperty.GetValue(validationContext.ObjectInstance);

            var endTimeValue = (DateTime)value;

            if (endTimeValue <= startTimeValue)
            {
                return new ValidationResult("End Time must be greater than Start Time.", new string[] { "EndTime" });
            }

            return ValidationResult.Success;
        }
    }

}
