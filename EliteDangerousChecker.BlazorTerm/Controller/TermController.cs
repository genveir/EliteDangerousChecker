using EliteDangerousChecker.Output;

namespace EliteDangerousChecker.BlazorTerm.Controller;

public class TermController : ITermController
{
    public List<string> Commands { get; } = [];

    public void SetDataHandlers(Func<string, Task> onDataAvailable, Func<Task> onTickComplete)
    {
        OnDataAvailable = onDataAvailable;
        OnTickComplete = onTickComplete;
    }

    public bool IsInitialized => OnDataAvailable != null && OnTickComplete != null;

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

    private string TimeOnly => DateTime.Now.ToString("[HH:mm:ss] ");

    public async Task UpdateView()
    {
        if (OnTickComplete == null) return;

        Console.WriteLine($"{TimeOnly}writing data");

        await OnTickComplete.Invoke();
    }

    public Func<string, Task>? OnDataAvailable;
    public Func<Task>? OnTickComplete;
}
