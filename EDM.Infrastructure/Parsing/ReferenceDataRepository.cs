using System.Xml.Linq;
using EDM.Core.Models;
using Microsoft.Extensions.Logging;

namespace EDM.Infrastructure.Parsing;

public class ReferenceDataRepository(ILogger<ReferenceDataRepository> logger) : IReferenceDataRepository
{
    public ReferenceFactors LoadReferenceFactors(string xmlFilePath)
        {
            if (string.IsNullOrWhiteSpace(xmlFilePath) || !File.Exists(xmlFilePath))
            {
                logger.LogError("Reference data XML not found at {Path}", xmlFilePath);
                throw new FileNotFoundException("Reference data XML not found", xmlFilePath);
            }

            logger.LogInformation("Loading reference factors from {Path}", xmlFilePath);

            var doc = XDocument.Load(xmlFilePath);

            var valueFactorNode = doc.Descendants("ValueFactor").FirstOrDefault();
            var emissionFactorNode = doc.Descendants("EmissionsFactor").FirstOrDefault();

            if (valueFactorNode == null || emissionFactorNode == null)
            {
                throw new InvalidDataException("Missing required nodes in ReferenceData.xml");
            }

            var referenceFactors = new ReferenceFactors
            {
                ValueFactor = new FactorSettings
                {
                    High = double.Parse(valueFactorNode.Element("High")?.Value ?? "0"),
                    Medium = double.Parse(valueFactorNode.Element("Medium")?.Value ?? "0"),
                    Low = double.Parse(valueFactorNode.Element("Low")?.Value ?? "0"),
                },
                EmissionsFactor = new FactorSettings
                {
                    High = double.Parse(emissionFactorNode.Element("High")?.Value ?? "0"),
                    Medium = double.Parse(emissionFactorNode.Element("Medium")?.Value ?? "0"),
                    Low = double.Parse(emissionFactorNode.Element("Low")?.Value ?? "0"),
                }
            };

            logger.LogDebug("Loaded reference factors: ValueFactor(High={High}, Medium={Medium}, Low={Low}), " +
                             "EmissionsFactor(High={EHigh}, Medium={EMedium}, Low={ELow})",
                referenceFactors.ValueFactor.High,
                referenceFactors.ValueFactor.Medium,
                referenceFactors.ValueFactor.Low,
                referenceFactors.EmissionsFactor.High,
                referenceFactors.EmissionsFactor.Medium,
                referenceFactors.EmissionsFactor.Low);

            return referenceFactors;
        }
    }