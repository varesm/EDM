using EDM.Infrastructure.Parsing;
using Microsoft.Extensions.Logging;
using Moq;

namespace EDM.Tests.Infrastructure
{
    public class GenerationReportRepositoryTests
    {
        private const string ExampleXml = @"
<GenerationReport>
    <Wind>
        <WindGenerator>
            <Name>Wind[Offshore]</Name>
            <Generation>
                <Day>
                    <Date>2017-01-01T00:00:00+00:00</Date>
                    <Energy>100.368</Energy>
                    <Price>20.148</Price>
                </Day>
            </Generation>
            <Location>Offshore</Location>
        </WindGenerator>
    </Wind>
</GenerationReport>";

        [Fact]
        public void LoadGenerationReport_ParsesWindGenerator()
        {
            // Arrange
            var mockLogger = new Mock<ILogger<GenerationReportRepository>>();
            var repo = new GenerationReportRepository(mockLogger.Object);

            var tempPath = Path.GetTempFileName();
            File.WriteAllText(tempPath, ExampleXml);

            // Act
            var generators = repo.LoadGenerationReport(tempPath);

            // Assert
            Assert.Single(generators);
            var gen = generators[0];
            Assert.Equal("Wind[Offshore]", gen.Name);
            Assert.NotEmpty(gen.DailyData);
            Assert.Equal(100.368, gen.DailyData[0].Energy, 3);
        }
    }
}