namespace EDM.Core.Services;

/// <summary>
/// Implementation of core calculation formulas.
/// </summary>
public class CalculationService : ICalculationService
{
    public double CalculateDailyGenerationValue(double energy, double price, double valueFactor)
    {
        return energy * price * valueFactor;
    }

    public double CalculateDailyEmissions(double energy, double emissionsRating, double emissionsFactor)
    {
        // For wind or zero-based inputs, this might return 0
        return energy * emissionsRating * emissionsFactor;
    }

    public double CalculateActualHeatRate(double totalHeatInput, double actualNetGeneration)
    {
        // Prevent divide-by-zero if net generation is zero
        if (actualNetGeneration is 0)
        {
            return 0;
        }
        return totalHeatInput / actualNetGeneration;
    }
}