namespace EDM.Infrastructure;

/// <summary>
/// Monitors a specified folder for new files matching certain criteria
/// and triggers an event when a file arrives.
/// </summary>
public interface IFileWatcherService
{
    /// <summary>
    /// Start monitoring the folder.
    /// </summary>
    void StartWatching();

    /// <summary>
    /// Event triggered when a new file that matches the criteria is created.
    /// </summary>
    event EventHandler<string>? OnFileCreated;
}