using EliteDangerousChecker.Database.FromJournal;

namespace EliteDangerousChecker.JournalFile.JournalUpdate;

internal class SystemChangeTracker
{
    private static bool HasGeneralChanges = false;
    private static List<int> BodyChanges = [];
    private static long CurrentSystemAddress = 0;

    private static bool Running = true;

    public static void MarkSystemChange(long newSystemAddress)
    {
        CurrentSystemAddress = newSystemAddress;
        HasGeneralChanges = true;
    }

    public static void MarkGeneralChange()
    {
        HasGeneralChanges = true;
    }

    public static void MarkBodyChange(int bodyId)
    {
        BodyChanges.Add(bodyId);
    }

    public static async Task StartOutputLoop()
    {
        while (Running)
        {
            var hasChanges = HasGeneralChanges || (BodyChanges.Count > 0);

            if (hasChanges)
            {
                bool general = HasGeneralChanges;
                int[] bodiesToUpdate = general ? [] : BodyChanges.ToArray();

                HasGeneralChanges = false;
                BodyChanges.Clear();

                var solarSystemName = await GetSolarSystemName.Execute(CurrentSystemAddress);

                if (general)
                {
                    var systemData = await GetBodyData.Execute(CurrentSystemAddress);

                    SystemWriter.WriteSystem(solarSystemName, systemData);
                }
                if (bodiesToUpdate.Length > 0)
                {
                    foreach (var bodyToUpdate in bodiesToUpdate)
                    {
                        var bodyData = await GetBodyData.Execute(CurrentSystemAddress, bodyToUpdate);

                        if (bodyData == null)
                        {
                            Console.WriteLine($"Unable to find body data for {bodyToUpdate}");
                            continue;
                        }

                        SystemWriter.UpdateBody(bodyData);
                    }
                }

            }
            await Task.Delay(1000);
        }
    }

    public static void Stop()
    {
        Running = false;
    }
}
