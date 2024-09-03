using System.Text.RegularExpressions;

namespace TaskManagement.Application.Methods
{
    public class PasswordValidator
    {
        private readonly int _minLength;
        private readonly int _minUpperCase;
        private readonly int _minLowerCase;
        private readonly int _minDigits;
        private readonly int _minSpecialChars;

        public PasswordValidator(int minLength = 8, int minUpperCase = 1, int minLowerCase = 1, int minDigits = 1, int minSpecialChars = 1)
        {
            _minLength = minLength;
            _minUpperCase = minUpperCase;
            _minLowerCase = minLowerCase;
            _minDigits = minDigits;
            _minSpecialChars = minSpecialChars;
        }

        public bool ValidatePassword(string password, out string errorMessage)
        {
            if (password.Length < _minLength)
            {
                errorMessage = $"Password must be at least {_minLength} characters long.";
                return false;
            }

            if (Regex.Matches(password, "[A-Z]").Count < _minUpperCase)
            {
                errorMessage = $"Password must contain at least {_minUpperCase} uppercase letter(s).";
                return false;
            }

            if (Regex.Matches(password, "[a-z]").Count < _minLowerCase)
            {
                errorMessage = $"Password must contain at least {_minLowerCase} lowercase letter(s).";
                return false;
            }

            if (Regex.Matches(password, @"\d").Count < _minDigits)
            {
                errorMessage = $"Password must contain at least {_minDigits} digit(s).";
                return false;
            }

            if (Regex.Matches(password, @"[\W_]").Count < _minSpecialChars)
            {
                errorMessage = $"Password must contain at least {_minSpecialChars} special character(s).";
                return false;
            }

            errorMessage = string.Empty;
            return true;
        }
    }
}
