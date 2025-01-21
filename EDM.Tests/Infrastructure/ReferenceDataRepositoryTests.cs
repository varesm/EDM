using System.IO;
using EDM.Infrastructure.Parsing;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace EDM.Tests.Infrastructure;

public class ReferenceDataRepositoryTests
{
    private const string ExampleXml = @"
<ReferenceData>
  <Factors>
    <ValueFactor>
      <High>0.946</High>
      <Medium>0.696</Medium>
      <Low>0.265</Low>
    </ValueFactor>
    <EmissionsFactor>
      <High>0.812</High>
      <Medium>0.562</Medium>
      <Low>0.312</Low>
    </EmissionsFactor>
  </Factors>
</ReferenceData>";

    [Fact]
    public void LoadReferenceFactors_ParsesValuesCorrectly()
    {
        // Arrange
        var mockLogger = new Mock<ILogger<ReferenceDataRepository>>();
        var repo = new ReferenceDataRepository(mockLogger.Object);

        var tempPath = Path.GetTempFileName();
        File.WriteAllText(tempPath, ExampleXml);

        // Act
        var result = repo.LoadReferenceFactors(tempPath);

        // Assert
        Assert.Equal(0.946, result.ValueFactor.High, 3);
        Assert.Equal(0.696, result.ValueFactor.Medium, 3);
        Assert.Equal(0.265, result.ValueFactor.Low, 3);
        Assert.Equal(0.812, result.EmissionsFactor.High, 3);
        Assert.Equal(0.562, result.EmissionsFactor.Medium, 3);
        Assert.Equal(0.312, result.EmissionsFactor.Low, 3);
    }
}