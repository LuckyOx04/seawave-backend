using System.Text.RegularExpressions;

namespace Services.Helpers;

public static partial class Validator
{
    public static bool IsValidEmail(string email) 
        => AllowedEmailsRegex().IsMatch(email);

    public static bool IsValidPassword(string password) 
        => AllowedPasswordsRegex().IsMatch(password);
    
    [GeneratedRegex(@"^[^@\s]+@[^@\s]+\.[^@\s]+$", RegexOptions.IgnoreCase, "en-US")]
    private static partial Regex AllowedEmailsRegex();
    [GeneratedRegex(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d).{8,}$")]
    private static partial Regex AllowedPasswordsRegex();
}