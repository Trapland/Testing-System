namespace Testing_System.Services.Validation
{
    public interface IValidationService
    {
        bool Validate(String source, params ValidationTerms[] terms);
    }
}
