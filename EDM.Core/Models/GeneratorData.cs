namespace EDM.Core.Models;

public class GeneratorData
{
    /// <summary>
    /// The full name of the generator, for example "Wind[Onshore]" or "Gas[1]".
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Type of the generator (Offshore Wind, Onshore Wind, Gas, Coal).
    /// </summary>
    public GeneratorType GeneratorType { get; set; }

    /// <summary>
    /// Emissions rating if applicable (gas/coal). 0 or unused otherwise.
    /// </summary>
    public double EmissionsRating { get; set; }

    /// <summary>
    /// Total heat input (coal only), 0 for other types.
    /// </summary>
    public double TotalHeatInput { get; set; }

    /// <summary>
    /// Actual net generation (coal only), 0 for other types.
    /// </summary>
    public double ActualNetGeneration { get; set; }

    /// <summary>
    /// Collection of daily data (energy and price) for each day in the report.
    /// </summary>
    public List<DayInfo> DailyData { get; set; } = [];
}