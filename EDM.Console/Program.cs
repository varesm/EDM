using EDM.Application;
using EDM.Console;
using EDM.Core.Services;
using EDM.Infrastructure;
using EDM.Infrastructure.FileWatching;
using EDM.Infrastructure.Parsing;
using EDM.Infrastructure.Writing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

// Build and configure the Generic Host
using var host = Host.CreateDefaultBuilder(args)
    .ConfigureAppConfiguration((hostingContext, config) =>
    {
        config.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .AddEnvironmentVariables()
            .AddCommandLine(args);
    })
    .ConfigureLogging((hostingContext, logging) =>
    {
        logging.ClearProviders();
        logging.AddConsole();
    })
    .ConfigureServices((hostContext, services) =>
    {
        var configuration = hostContext.Configuration;

        // 1. Register CORE Services
        services.AddSingleton<ICalculationService, CalculationService>();

        var referenceDataPath = configuration["ReferenceDataFilePath"]
                                ?? "ReferenceData.xml";
        
        var tempServiceProvider = services.BuildServiceProvider();
        var tempLogger = tempServiceProvider.GetRequiredService<ILogger<ReferenceDataRepository>>();
        var refRepo = new ReferenceDataRepository(tempLogger);
        var referenceFactors = refRepo.LoadReferenceFactors(referenceDataPath);

        
        services.AddSingleton<IFactorService>(_ => new FactorService(referenceFactors));

        // 2. Register INFRASTRUCTURE Services

        // Parsing Repositories
        services.AddSingleton<IReferenceDataRepository, ReferenceDataRepository>();
        services.AddSingleton<IGenerationReportRepository, GenerationReportRepository>();

        // Output Writer
        services.AddSingleton<IOutputWriter, XmlOutputWriter>();

        // File watching (optional). You might have "InputFolder" in appsettings.json:
        var inputFolder = configuration["InputFolder"] ?? ".";
        // We watch for "GenerationReport.xml" specifically, or *.xml if you prefer
        services.AddSingleton<IFileWatcherService>(sp =>
            new FileWatcherService(
                folderPath: inputFolder,
                filter: "GenerationReport.xml",
                logger: sp.GetRequiredService<ILogger<FileWatcherService>>()
            )
        );
        
        // 3. Register APPLICATION Services
        services.AddSingleton<IReportProcessingService, ReportProcessingService>();

        // 4. Register a Hosted Service that orchestrates everything
        // This worker will start the file watcher and process new reports
        services.AddHostedService<ReportProcessingWorker>();
    })
    .Build();

await host.RunAsync();