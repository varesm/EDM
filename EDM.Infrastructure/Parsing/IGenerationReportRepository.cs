using EDM.Core.Models;

namespace EDM.Infrastructure.Parsing;

/// <summary>
/// Responsible for reading GenerationReport.xml
/// and converting it to domain models (GeneratorData).
/// </summary>
public interface IGenerationReportRepository
{
    /// <summary>
    /// Parses the input GenerationReport.xml into domain objects.
    /// </summary>
    /// <param name="xmlFilePath">Path to the GenerationReport.xml file</param>
    /// <returns>A list of GeneratorData objects</returns>
    List<GeneratorData> LoadGenerationReport(string xmlFilePath);
}