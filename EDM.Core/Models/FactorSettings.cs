namespace EDM.Core.Models;

/// <summary>
/// Stores numeric values for High, Medium, Low categories.
/// Example: ValueFactor or EmissionsFactor.
/// </summary>
public class FactorSettings
{
    public double High { get; set; }
    public double Medium { get; set; }
    public double Low { get; set; }
}