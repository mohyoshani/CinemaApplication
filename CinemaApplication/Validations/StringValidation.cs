namespace CinemaApplication.Validations
{
    public class StringValidation: ValidationAttribute
    {

        public override bool IsValid(object? value)
        {
         
            if (value == null)
            {
                return true;
            }

         
            if (value is string stringValue)
            {
             
                return true;
            }

            return false;
        }
        public override string FormatErrorMessage(string name)
        {
            return ErrorMessage ?? $"The field {name} must be a string.";
        }
    }
}
