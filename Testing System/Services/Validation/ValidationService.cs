using System.Text.RegularExpressions;

namespace Testing_System.Services.Validation
{

    public enum ValidationTerms
    {
        None = 0,
        NotEmpty,
        Login,
        Email,
        RealName,
        Password
    }

    public class ValidationService : IValidationService
    {
        public bool Validate(string source, params ValidationTerms[] terms)
        {
            if (terms.Length == 0) throw new ArgumentException("No term(s) for validation");
            if (terms.Length == 1 && terms[0] == ValidationTerms.None)
            {
                return true;
            }
            bool result = true;

            if (terms.Contains(ValidationTerms.NotEmpty))
            {
                result &= ValidateNotEmpty(source);
            }
            if (terms.Contains(ValidationTerms.Login))
            {
                result &= ValidateLogin(source);
            }
            if (terms.Contains(ValidationTerms.Email))
            {
                result &= ValidateEmail(source);
            }
            if (terms.Contains(ValidationTerms.RealName))
            {
                result &= ValidateRealName(source);
            }
            if (terms.Contains(ValidationTerms.Password))
            {
                result &= ValidatePassword(source);
            }
            return result;
        }
        private static bool ValidateRegex(string source, String pattern)
        {
            return Regex.IsMatch(source, pattern);
        }
        private static bool ValidateEmail(string source)
        {
            return ValidateRegex(source, @"^[\w.%+-]+@[\w.-]+\.[a-zA-Z]{2,}$");
        }
        private static bool ValidateLogin(string source)
        {
            return ValidateRegex(source, @"^\w{3,}$");
        }
        private static bool ValidateRealName(string source)
        {
            return ValidateRegex(source, @"^.+$");
        }
        private static bool ValidatePassword(string source)
        {
            return source.Length >= 3;
        }

        private bool ValidateNotEmpty(String source)
        {
            return !string.IsNullOrEmpty(source);
        }
    }
}
