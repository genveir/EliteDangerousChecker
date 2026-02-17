namespace EliteDangerousChecker.Output;

public interface ITermController
{
    void SetDataHandlers(Func<string, Task> onDataAvailable, Func<Task> onTickComplete);

    public bool IsInitialized { get; }

    string[] GetCommands();

    Task RegisterInput(string input);

    Task SendOutputLine(string output);

    Task UpdateView();
}
