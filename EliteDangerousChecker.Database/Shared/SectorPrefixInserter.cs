using Dapper;
using EliteDangerousChecker.Database.Shared.ImmediateWrite;

namespace EliteDangerousChecker.Database.Shared;
internal static class SectorPrefixInserter
{
    public static async Task InsertSectorPrefix(long solarSystemId, string[] prefixes)
    {
        int sequence = 0;

        for (int n = 0; n < prefixes.Length; n++)
        {
            var prefix = prefixes[n];

            if (prefix.Contains('-'))
            {
                var split = prefix.Split('-');

                await InsertSectorPrefix(solarSystemId, split[0], sequence++, startWithDash: false);
                for (int i = 1; i < split.Length; i++)
                {
                    await InsertSectorPrefix(solarSystemId, split[i], sequence++, startWithDash: true);
                }
            }
            else
            {
                await InsertSectorPrefix(solarSystemId, prefix, sequence++, startWithDash: false);
            }
        }
    }

    private static async Task InsertSectorPrefix(long solarSystemId, string prefix, int sequence, bool startWithDash)
    {
        bool startWithJ = false;
        bool isNumber = false;
        int prefixNumeric = 0;

        if (prefix.StartsWith('J') && prefix.Length > 1)
        {
            isNumber = int.TryParse(prefix[1..], out prefixNumeric);
            startWithJ = isNumber;
        }

        if (!startWithJ)
        {
            isNumber = int.TryParse(prefix, out prefixNumeric);
        }

        var sectorPrefixWordId = isNumber ? null : await SectorPrefixWordAccess.GetId(prefix);

        using var connection = DbAccess.GetOpenConnection();

        var insertSql = @"
insert into SectorPrefix (SolarSystemId, Sequence, SectorPrefixWordId, SectorPrefixNumber, StartWithDash, StartWithJ)
values (@SolarSystemId, @Sequence, @SectorPrefixWordId, @SectorPrefixNumber, @StartWithDash, @StartWithJ)";
        await connection.ExecuteAsync(insertSql, new
        {
            SolarSystemId = solarSystemId,
            Sequence = sequence,
            SectorPrefixWordId = sectorPrefixWordId,
            SectorPrefixNumber = isNumber ? prefixNumeric : (int?)null,
            StartWithDash = startWithDash,
            StartWithJ = startWithJ
        });
    }
}
