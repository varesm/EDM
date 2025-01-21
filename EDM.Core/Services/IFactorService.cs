using EDM.Core.Models;

namespace EDM.Core.Services;

/// <summary>
/// Provides an abstraction for retrieving the correct factors (Value, Emissions)
/// based on generator type
/// </summary>
public interface IFactorService
{
    double GetValueFactor(GeneratorType generatorType);
    double GetEmissionFactor(GeneratorType generatorType);
}