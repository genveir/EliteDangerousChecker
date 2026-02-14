namespace EliteDangerousChecker.Database.Shared;
internal static class SolarSystemNameParser
{
    public static (string[] prefixWords, string? suffix, string? postfix) Parse(string? name)
    {
        if (name == null)
        {
            return (Array.Empty<string>(), null, null);
        }

        var nameParts = name.Split(' ');
        if (nameParts.Length >= 3)
        {
            var suffix = nameParts[^2];
            var postfix = nameParts[^1];
            if (suffix.Length == 4 && suffix[2] == '-' &&
                postfix.Length <= 5 && postfix[1] >= '0' && postfix[1] <= '9')
            {
                return (nameParts[..^2], suffix, postfix);
            }
        }
        return (nameParts, null, null);
    }
}
