namespace EliteDangerousChecker.Output.Util;

internal static class ANSI_Colors
{
    public const string Black = "\u001b[30m";
    public const string Red = "\u001b[31m";
    public const string Green = "\u001b[32m";
    public const string Yellow = "\u001b[33m";
    public const string Blue = "\u001b[34m";
    public const string Magenta = "\u001b[35m";
    public const string Cyan = "\u001b[36m";
    public const string White = "\u001b[37m";
    public const string BrightBlack = "\u001b[30;1m";
    public const string BrightRed = "\u001b[31;1m";
    public const string BrightGreen = "\u001b[32;1m";
    public const string BrightYellow = "\u001b[33;1m";
    public const string BrightBlue = "\u001b[34;1m";
    public const string BrightMagenta = "\u001b[35;1m";
    public const string BrightCyan = "\u001b[36;1m";
    public const string BrightWhite = "\u001b[37;1m";

    public const string Reset = "\u001b[0m";

    public static string Colorize(string text, string color)
    {
        return $"{color}{text}{Reset}";
    }
}
