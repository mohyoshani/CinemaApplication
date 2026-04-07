namespace CinemaApplication.Validations
{
    public class StringValidationAttribute : ValidationAttribute
    {
       
        public override bool IsValid(object? value)
        {
            if(value == null) return false;
            if(value is not string)
            {
               return false;
            }
            
            return base.IsValid(value);
        }

        public override string FormatErrorMessage(string name)
        {
            return ErrorMessage ?? $"The field {name} must be a string.";
        }
    }
}
