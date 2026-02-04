namespace EliteDangerousChecker.JournalFile;

public sealed class JournalFolderWatcher : IDisposable
{
    const string JournalFolderPath = @"c:\Users\genve\Saved Games\Frontier Developments\Elite Dangerous";

    private readonly FileSystemWatcher watcher;

    private DateTime? LastChange;
    private List<string> ChangedFiles = [];

    private static readonly TimeSpan WaitAfterLastChange = TimeSpan.FromMilliseconds(1000);

    private static int Delay = 1000;

    public bool HasChanges { get; private set; } = false;

    public JournalFolderWatcher()
    {
        Console.WriteLine($"created journal folder watcher");

        watcher = new FileSystemWatcher(JournalFolderPath);

        Delay = new Random().Next(500) + 750;
    }

    public async Task StartWatching(CancellationToken cancellationToken)
    {
        Console.WriteLine($"start watching {JournalFolderPath}");

        watcher.Changed += OnChange;
        watcher.Created += OnChange;
        watcher.Deleted += OnChange;
        watcher.Renamed += OnChange;
        watcher.Error += OnError;
        watcher.EnableRaisingEvents = true;

        try
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                if (LastChange != null && DateTime.Now - LastChange > WaitAfterLastChange)
                {
                    await PerformCallback();
                }

                await Task.Delay(Delay, cancellationToken);
            }
        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
            Console.WriteLine(e.StackTrace ?? "no stack trace");

            throw;
        }
        finally
        {
            Console.WriteLine($"stop watching {JournalFolderPath}");
        }
    }

    private async Task PerformCallback()
    {
        await FilesChangedHandler.HandleUpdate(ChangedFiles);

        LastChange = null;
        ChangedFiles.Clear();
    }

    private void OnError(object sender, ErrorEventArgs e)
    {
        Console.WriteLine($"error in {JournalFolderPath}");
        Console.WriteLine(e.GetException());
    }

    private void OnChange(object sender, FileSystemEventArgs e)
    {
        LastChange = DateTime.Now;
        ChangedFiles.Add(e.FullPath);
    }

    public void Dispose()
    {
        watcher.Dispose();
    }
}
