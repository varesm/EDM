namespace EDM.Core.Models;

/// <summary>
/// Contains factor settings for both value and emissions data.
/// will load from ReferenceData.xml.
/// </summary>
public class ReferenceFactors
{
    public FactorSettings ValueFactor { get; set; } = new();
    public FactorSettings EmissionsFactor { get; set; } = new();
}