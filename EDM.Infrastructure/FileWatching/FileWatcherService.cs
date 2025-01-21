using Microsoft.Extensions.Logging;

namespace EDM.Infrastructure.FileWatching;

public class FileWatcherService(string folderPath, string filter, ILogger<FileWatcherService> logger)
    : IFileWatcherService
{
    private readonly string _folderPath = folderPath ?? throw new ArgumentNullException(nameof(folderPath));
    private readonly string _filter = filter ?? "*.*"; // "GenerationReport.xml"

    public event EventHandler<string>? OnFileCreated;

    public void StartWatching()
    {
        logger.LogInformation("Starting file watcher on path {Path} with filter {Filter}", _folderPath, _filter);

        var watcher = new FileSystemWatcher(_folderPath, _filter)
        {
            NotifyFilter = NotifyFilters.FileName | NotifyFilters.LastWrite | NotifyFilters.CreationTime
        };

        watcher.Created += OnCreated;
        watcher.EnableRaisingEvents = true;
    }

    private void OnCreated(object sender, FileSystemEventArgs e)
    {
        logger.LogInformation("New file detected: {FileName}", e.FullPath);
        OnFileCreated?.Invoke(this, e.FullPath);
    }
    
    //https://learn.microsoft.com/en-us/dotnet/api/system.io.filesystemwatcher?view=net-8.0
}