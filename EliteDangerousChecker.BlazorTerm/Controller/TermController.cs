using EliteDangerousChecker.JournalFile.JournalUpdate;

namespace EliteDangerousChecker.BlazorTerm.Controller;

public class TermController : ITermController
{
    public List<string> Commands { get; } = [];

    public void SetDataHandlers(Func<string, Task> onDataAvailable, Func<Task> onTickComplete, Func<Task> onClear)
    {
        OnDataAvailable = onDataAvailable;
        OnTickComplete = onTickComplete;
        OnClear = onClear;
    }

    public bool IsInitialized => OnDataAvailable != null && OnTickComplete != null && OnClear != null;

    public string[] GetCommands()
    {
        var copy = Commands.ToArray();
        Commands.Clear();

        return copy;
    }

    public async Task RegisterInput(string input)
    {
        Commands.Add(input);

        await Task.CompletedTask;
    }

    public async Task SendOutputLine(string output)
    {
        if (OnDataAvailable == null) return;

        await OnDataAvailable.Invoke(output);
    }

    public async Task DoTick()
    {
        if (OnTickComplete == null) return;

        await OnTickComplete.Invoke();
    }

    public async Task Clear()
    {
        if (OnClear == null) return;

        await OnClear.Invoke();
    }

    public Func<string, Task>? OnDataAvailable;
    public Func<Task>? OnTickComplete;
    public Func<Task>? OnClear;
}
