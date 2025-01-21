namespace EDM.Infrastructure.Writing;

/// <summary>
/// Writes the final output (GenerationOutput.xml) based on
/// aggregated domain calculations
/// </summary>
public interface IOutputWriter
{
    /// <summary>
    /// Writes the totals, max emissions, and heat rates to an XML file.
    /// </summary>
    /// <param name="outputPath">Destination path for the XML file.</param>
    /// <param name="totals">List of generator total generation values.</param>
    /// <param name="maxEmissionsPerDay">List of day-based max emission info.</param>
    /// <param name="heatRates">List of coal generator heat rates.</param>
    void WriteOutput(
        string outputPath,
        IEnumerable<(string GeneratorName, double TotalValue)> totals,
        IEnumerable<(string GeneratorName, double Emission, System.DateTime Date)> maxEmissionsPerDay,
        IEnumerable<(string GeneratorName, double HeatRate)> heatRates);
}