namespace EliteDangerousChecker.Database;
internal class DbHelper
{
    public static long? DateTimeToUnix(DateTime? dateTime)
    {
        if (dateTime == null)
            return null;

        if (dateTime == DateTime.MinValue)
            return null;

        var dto = new DateTimeOffset(dateTime.Value, TimeSpan.Zero);
        return dto.ToUnixTimeSeconds();
    }

    public static DateTime UnixToDateTime(long unixTimestamp)
    {
        var dto = DateTimeOffset.FromUnixTimeSeconds(unixTimestamp);
        return dto.DateTime;
    }

    public static long? OffsetToUnix(string? offset)
    {
        if (offset == null)
            return null;

        if (DateTimeOffset.TryParse(offset, out var dto))
        {
            return dto.ToUnixTimeSeconds();
        }
        return null;
    }

    public static dynamic GetDictValueOrDbNull<T>(Dictionary<string, T>? dict, string key)
    {
        if (dict != null && dict.TryGetValue(key, out var value))
        {
            return ValueOrDbNull(value);
        }
        return DBNull.Value;
    }

    public static dynamic ValueOrDbNull(object? value)
    {
        return value ?? DBNull.Value;
    }

}
