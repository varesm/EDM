using System.Xml.Linq;
using EDM.Core.Models;
using Microsoft.Extensions.Logging;

namespace EDM.Infrastructure.Parsing;

public class GenerationReportRepository(ILogger<GenerationReportRepository> logger) : IGenerationReportRepository
{
    public List<GeneratorData> LoadGenerationReport(string xmlFilePath)
        {
            if (string.IsNullOrWhiteSpace(xmlFilePath) || !File.Exists(xmlFilePath))
            {
                logger.LogError("Generation report XML not found at {Path}", xmlFilePath);
                throw new FileNotFoundException("Generation report XML not found", xmlFilePath);
            }

            logger.LogInformation("Loading generation report from {Path}", xmlFilePath);

            var doc = XDocument.Load(xmlFilePath);

            var windGenerators = doc.Descendants("WindGenerator");
            var result = windGenerators.Select(ParseWindGenerator).OfType<GeneratorData>().ToList();

            var gasGenerators = doc.Descendants("GasGenerator");
            result.AddRange(gasGenerators.Select(ParseGasGenerator).OfType<GeneratorData>());
            
            var coalGenerators = doc.Descendants("CoalGenerator");
            result.AddRange(coalGenerators.Select(ParseCoalGenerator).OfType<GeneratorData>());

            return result;
        }

        private static GeneratorData? ParseWindGenerator(XElement windElem)
        {
            // <Name>Wind[Offshore]</Name> or <Name>Wind[Onshore]</Name>
            var name = windElem.Element("Name")?.Value ?? "";
            var location = windElem.Element("Location")?.Value ?? "";

            var generatorType = location.Equals("Offshore", StringComparison.OrdinalIgnoreCase)
                ? GeneratorType.WindOffshore
                : GeneratorType.WindOnshore;

            var generator = new GeneratorData
            {
                Name = name,
                GeneratorType = generatorType,
                EmissionsRating = 0, // wind does not emit
                TotalHeatInput = 0,
                ActualNetGeneration = 0,
                DailyData = ParseDailyData(windElem.Element("Generation"))
            };

            return generator;
        }

        private static GeneratorData? ParseGasGenerator(XElement gasElem)
        {
            // <Name>Gas[1]</Name>
            var name = gasElem.Element("Name")?.Value ?? "";

            double.TryParse(gasElem.Element("EmissionsRating")?.Value, out var emissionsRating);

            var generator = new GeneratorData
            {
                Name = name,
                GeneratorType = GeneratorType.Gas,
                EmissionsRating = emissionsRating,
                DailyData = ParseDailyData(gasElem.Element("Generation")),
                // Gas does not use heat input or net generation
                TotalHeatInput = 0,
                ActualNetGeneration = 0
            };

            return generator;
        }

        private GeneratorData? ParseCoalGenerator(XElement coalElem)
        {
            // <Name>Coal[1]</Name>
            var name = coalElem.Element("Name")?.Value ?? "";

            double.TryParse(coalElem.Element("EmissionsRating")?.Value, out var emissionsRating);
            double.TryParse(coalElem.Element("TotalHeatInput")?.Value, out var totalHeatInput);
            double.TryParse(coalElem.Element("ActualNetGeneration")?.Value, out var actualNetGeneration);

            var generator = new GeneratorData
            {
                Name = name,
                GeneratorType = GeneratorType.Coal,
                EmissionsRating = emissionsRating,
                TotalHeatInput = totalHeatInput,
                ActualNetGeneration = actualNetGeneration,
                DailyData = ParseDailyData(coalElem.Element("Generation"))
            };

            return generator;
        }

        private static List<DayInfo> ParseDailyData(XElement? generationElem)
        {
            var days = new List<DayInfo>();
            if (generationElem == null) return days;

            // <Day> elements
            var dayElems = generationElem.Descendants("Day");
            foreach (var dayElem in dayElems)
            {
                var dateStr = dayElem.Element("Date")?.Value;
                var energyStr = dayElem.Element("Energy")?.Value;
                var priceStr = dayElem.Element("Price")?.Value;

                if (DateTime.TryParse(dateStr, out DateTime date) &&
                    double.TryParse(energyStr, out var energy) &&
                    double.TryParse(priceStr, out var price))
                {
                    days.Add(new DayInfo
                    {
                        Date = date,
                        Energy = energy,
                        Price = price
                    });
                }
            }

            return days;
        }
    }