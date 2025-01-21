namespace EDM.Core.Services;

/// <summary>
/// Provides calculation formulas for generation values, emissions, heat rates, etc.
/// </summary>
public interface ICalculationService
{
    /// <summary>
    /// Calculate the daily generation value for a single day.
    /// DailyGenerationValue = Energy x Price x ValueFactor
    /// </summary>
    double CalculateDailyGenerationValue(double energy, double price, double valueFactor);

    /// <summary>
    /// Calculate daily emissions for a single day (only applies to fossil fuels).
    /// DailyEmissions = Energy x EmissionsRating x EmissionsFactor
    /// </summary>
    double CalculateDailyEmissions(double energy, double emissionsRating, double emissionsFactor);

    /// <summary>
    /// Calculate the actual heat rate (coal only).
    /// ActualHeatRate = TotalHeatInput / ActualNetGeneration
    /// </summary>
    double CalculateActualHeatRate(double totalHeatInput, double actualNetGeneration);
}