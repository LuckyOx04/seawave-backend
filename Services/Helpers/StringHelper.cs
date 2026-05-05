using System.Text.RegularExpressions;

namespace Services.Helpers;

public static partial class StringHelper
{
    public static string Slugify(this string phrase)
    {
        if (string.IsNullOrEmpty(phrase))
        {
            return string.Empty;
        }

        var str = phrase.ToLower();

        str = NotAllowedCharactersRegex().Replace(str, "");
        
        str = WhiteSpaceOrHyphenRegex().Replace(str, "-").Trim();
        
        return str;
    }

    [GeneratedRegex(@"[^a-z0-9\s-]")]
    private static partial Regex NotAllowedCharactersRegex();
    [GeneratedRegex(@"[\s-]+")]
    private static partial Regex WhiteSpaceOrHyphenRegex();
}