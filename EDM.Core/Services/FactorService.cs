using EDM.Core.Models;

namespace EDM.Core.Services;

/// <summary>
/// Concrete implementation of IFactorService to map generator types
/// to the correct factor values.
/// </summary>
public class FactorService(ReferenceFactors referenceFactors) : IFactorService
{
    private readonly ReferenceFactors _referenceFactors = referenceFactors ?? throw new ArgumentNullException(nameof(referenceFactors));

    public double GetValueFactor(GeneratorType generatorType)
    {
        return generatorType switch
        {
            GeneratorType.WindOffshore => _referenceFactors.ValueFactor.Low,
            GeneratorType.WindOnshore => _referenceFactors.ValueFactor.High,
            GeneratorType.Gas => _referenceFactors.ValueFactor.Medium,
            GeneratorType.Coal => _referenceFactors.ValueFactor.Medium,
            _ => throw new ArgumentOutOfRangeException(nameof(generatorType), generatorType, null)
        };
    }

    public double GetEmissionFactor(GeneratorType generatorType)
    {
        return generatorType switch
        {
            GeneratorType.Gas => _referenceFactors.EmissionsFactor.Medium,
            GeneratorType.Coal => _referenceFactors.EmissionsFactor.High,
            // Wind doesn't use an EmissionsFactor, return 0
            GeneratorType.WindOffshore => 0,
            GeneratorType.WindOnshore => 0,
            _ => throw new ArgumentOutOfRangeException(nameof(generatorType), generatorType, null)
        };
    }
}