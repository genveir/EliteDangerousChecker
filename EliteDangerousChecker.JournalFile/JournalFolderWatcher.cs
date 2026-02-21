using EliteDangerousChecker.JournalFile.PublicAbstractions;

namespace EliteDangerousChecker.JournalFile;

internal sealed class JournalFolderWatcher : IDisposable, IJournalFolderWatcher
{
    const string JournalFolderPath = @"c:\Users\genve\Saved Games\Frontier Developments\Elite Dangerous";

    private readonly FileSystemWatcher watcher;
    private readonly FilesChangedHandler filesChangedHandler;

    private readonly List<string>[] ChangedFiles = [[], []];
    private int WriteIndex = 0;

    private const int Delay = 100;

    public JournalFolderWatcher(FilesChangedHandler filesChangedHandler)
    {
        Console.WriteLine($"created journal folder watcher");

        watcher = new FileSystemWatcher(JournalFolderPath);
        this.filesChangedHandler = filesChangedHandler;
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
                if (ChangedFiles[WriteIndex].Count > 0)
                {
                    WriteIndex = 1 - WriteIndex;
                    await PerformCallback(ChangedFiles[1 - WriteIndex].ToArray());
                    ChangedFiles[1 - WriteIndex].Clear();
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

    private async Task PerformCallback(string[] changedFiles)
    {
        await filesChangedHandler.HandleUpdate(changedFiles);
    }

    private void OnError(object sender, ErrorEventArgs e)
    {
        Console.WriteLine($"error in {JournalFolderPath}");
        Console.WriteLine(e.GetException());
    }

    private void OnChange(object sender, FileSystemEventArgs e)
    {
        if (e.FullPath.EndsWith("Status.json") || e.FullPath.EndsWith("lastupdatetime.txt"))
            return;

        Console.WriteLine($"change in {e.FullPath}");

        if (ChangedFiles[WriteIndex].Contains(e.FullPath))
        {
            return;
        }

        ChangedFiles[WriteIndex].Add(e.FullPath);
    }

    public void Dispose()
    {
        watcher.Dispose();
    }
}
