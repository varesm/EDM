using EDM.Core.Models;
using EDM.Core.Services;
using EDM.Infrastructure.Parsing;
using EDM.Infrastructure.Writing;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;

namespace EDM.Application;

/// <summary>
/// Concrete implementation that handles end-to-end report processing logic.
/// </summary>
public class ReportProcessingService(
    IGenerationReportRepository reportRepository,
    ICalculationService calculationService,
    IFactorService factorService,
    IOutputWriter outputWriter,
    IConfiguration configuration,
    ILogger<ReportProcessingService> logger)
    : IReportProcessingService
{
    private readonly IGenerationReportRepository _reportRepository =
        reportRepository ?? throw new ArgumentNullException(nameof(reportRepository));

    private readonly ICalculationService _calculationService =
        calculationService ?? throw new ArgumentNullException(nameof(calculationService));

    private readonly IFactorService _factorService =
        factorService ?? throw new ArgumentNullException(nameof(factorService));

    private readonly IOutputWriter
        _outputWriter = outputWriter ?? throw new ArgumentNullException(nameof(outputWriter));

    private readonly IConfiguration _configuration =
        configuration ?? throw new ArgumentNullException(nameof(configuration));

    private readonly ILogger<ReportProcessingService> _logger =
        logger ?? throw new ArgumentNullException(nameof(logger));

    public async Task ProcessReportFileAsync(string filePath)
    {
        if (string.IsNullOrWhiteSpace(filePath) || !File.Exists(filePath))
        {
            _logger.LogError("File path {FilePath} is invalid or does not exist.", filePath);
            return;
        }

        _logger.LogInformation("Processing report file: {FilePath}", filePath);

        // 1. Load generator data
        var generatorDataList = _reportRepository.LoadGenerationReport(filePath);
        if (generatorDataList.Count == 0)
        {
            _logger.LogWarning("No generator data found in {FilePath}", filePath);
            return;
        }

        // 2. Compute results
        var (totals, maxEmissionsPerDay, heatRates) = ComputeResults(generatorDataList);

        // 3. Write Output
        var outputFolder = _configuration["OutputFolder"] ?? ".";
        var outputPath = Path.Combine(outputFolder, "GenerationOutput.xml");

        _logger.LogInformation("Writing output to {OutputPath}", outputPath);
        _outputWriter.WriteOutput(outputPath, totals, maxEmissionsPerDay, heatRates);

        _logger.LogInformation("Completed processing for {FilePath}", filePath);
    }

    /// <summary>
    /// Encapsulates the calculation logic (daily values, max emissions, heat rates) 
    /// into a single method that returns the necessary info for output.
    /// </summary>
    private (
        IEnumerable<(string GeneratorName, double TotalValue)> totals,
        IEnumerable<(string GeneratorName, double Emission, DateTime Date)> maxEmissionsPerDay,
        IEnumerable<(string GeneratorName, double HeatRate)> heatRates
        ) ComputeResults(IEnumerable<GeneratorData> generatorDataList)
    {
        var dailyEmissionsByDate = new Dictionary<DateTime, List<(string generatorName, double emission)>>();
        var totalValues = new List<(string GeneratorName, double TotalValue)>();
        var heatRates = new List<(string GeneratorName, double HeatRate)>();

        foreach (var gen in generatorDataList)
        {
            double generatorTotal = 0;

            // If it's coal, calculate heat rate once
            if (gen is { GeneratorType: GeneratorType.Coal, ActualNetGeneration: 0 })
            {
                var hr = _calculationService.CalculateActualHeatRate(gen.TotalHeatInput, gen.ActualNetGeneration);
                heatRates.Add((gen.Name, hr));
            }

            // Determine factors
            var valFactor = _factorService.GetValueFactor(gen.GeneratorType);
            var emissionFactor = _factorService.GetEmissionFactor(gen.GeneratorType);

            foreach (var day in gen.DailyData)
            {
                // Daily Generation Value
                var dailyValue = _calculationService.CalculateDailyGenerationValue(day.Energy, day.Price, valFactor);
                generatorTotal += dailyValue;

                // For fossil fuels only
                if (gen.GeneratorType is not (GeneratorType.Gas or GeneratorType.Coal)) continue;

                var dailyEmission =
                    _calculationService.CalculateDailyEmissions(day.Energy, gen.EmissionsRating, emissionFactor);

                if (!dailyEmissionsByDate.TryGetValue(day.Date, out var value))
                {
                    value = new List<(string, double)>();
                    dailyEmissionsByDate[day.Date] = value;
                }

                value.Add((gen.Name, dailyEmission));
            }

            totalValues.Add((gen.Name, generatorTotal));
        }

        var maxEmissionsPerDay = new List<(string GeneratorName, double Emission, DateTime Date)>();
        foreach (var (date, emissionList) in dailyEmissionsByDate)
        {
            var maxEm = emissionList.OrderByDescending(e => e.emission).First();
            maxEmissionsPerDay.Add((maxEm.generatorName, maxEm.emission, date));
        }

        return (totalValues, maxEmissionsPerDay, heatRates);
    }
}