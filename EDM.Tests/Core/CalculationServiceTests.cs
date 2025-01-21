using EDM.Core.Services;

namespace EDM.Tests.Core;

public class CalculationServiceTests
{
    private readonly CalculationService _calculationService = new();

    [Theory]
    [InlineData(100, 20, 0.696, 1392)]
    [InlineData(50, 10, 0.265, 132.5)]
    [InlineData(0, 99, 0.946, 0)]
    public void CalculateDailyGenerationValue_ReturnsCorrectValue(double energy, double price, double factor, double expected)
    {
        // Act
        var result = _calculationService.CalculateDailyGenerationValue(energy, price, factor);

        // Assert
        Assert.Equal(expected, result, 3);
    }

    [Theory]
    [InlineData(200, 0.05, 0.562, 5.62)]
    [InlineData(100, 0.0, 0.812, 0)]
    [InlineData(0, 0.038, 0.562, 0)]
    public void CalculateDailyEmissions_ReturnsCorrectValue(double energy, double emissionsRating, double emissionFactor, double expected)
    {
        // Act
        var result = _calculationService.CalculateDailyEmissions(energy, emissionsRating, emissionFactor);

        // Assert
        Assert.Equal(expected, result, 3);
    }

    [Theory]
    [InlineData(100, 50, 2)]
    [InlineData(11.815, 11.815, 1)]
    [InlineData(10, 0, 0)] // Should not throw, returns 0
    public void CalculateActualHeatRate_ReturnsCorrectValue(double totalHeatInput, double actualNetGeneration, double expected)
    {
        // Act
        var result = _calculationService.CalculateActualHeatRate(totalHeatInput, actualNetGeneration);

        // Assert
        Assert.Equal(expected, result, 3);
    }
}