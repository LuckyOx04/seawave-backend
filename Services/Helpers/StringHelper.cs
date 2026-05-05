using System.Text.RegularExpressions;

namespace Services.Helpers;

public static class StringHelper
{
    public static string Slugify(this string phrase)
    {
        if (string.IsNullOrEmpty(phrase))
        {
            return string.Empty;
        }

        string str = phrase.ToLower();

        str = Regex.Replace(str, @"[^a-z0-9\s-]", "");
        
        str = Regex.Replace(str, @"[\s-]+", "-").Trim();
        
        return str;
    }
}