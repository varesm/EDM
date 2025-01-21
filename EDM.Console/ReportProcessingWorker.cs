using EDM.Application;
using EDM.Core.Models;
using EDM.Core.Services;
using EDM.Infrastructure;
using EDM.Infrastructure.Parsing;
using EDM.Infrastructure.Writing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace EDM.Console;

public class ReportProcessingWorker(
    IFileWatcherService fileWatcher,
    IReportProcessingService reportProcessingService,
    ILogger<ReportProcessingWorker> logger)
    : BackgroundService
{
    private readonly IFileWatcherService _fileWatcher = fileWatcher ?? throw new ArgumentNullException(nameof(fileWatcher));
    private readonly IReportProcessingService _reportProcessingService = reportProcessingService ?? throw new ArgumentNullException(nameof(reportProcessingService));
    private readonly ILogger<ReportProcessingWorker> _logger = logger ?? throw new ArgumentNullException(nameof(logger));

    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        // Start watching
        _fileWatcher.OnFileCreated += async (sender, filePath) =>
        {
            try
            {
                _logger.LogInformation("File created event triggered for {FilePath}", filePath);
                await _reportProcessingService.ProcessReportFileAsync(filePath);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing file {FilePath}", filePath);
            }
        };

        _fileWatcher.StartWatching();

        // Keep running until stop requested
        return Task.CompletedTask;
    }
}