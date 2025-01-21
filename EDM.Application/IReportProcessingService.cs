namespace EDM.Application;

/// <summary>
/// Orchestrates loading the generation report, computing calculations, and producing the output.
/// </summary>
public interface IReportProcessingService
{
    /// <summary>
    /// Processes a single GenerationReport file end-to-end (parse, calculate, write output).
    /// </summary>
    Task ProcessReportFileAsync(string filePath);
}