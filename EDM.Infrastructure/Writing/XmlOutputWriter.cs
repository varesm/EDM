using System.Xml.Linq;
using Microsoft.Extensions.Logging;

namespace EDM.Infrastructure.Writing;

public class XmlOutputWriter(ILogger<XmlOutputWriter> logger) : IOutputWriter
{
    public void WriteOutput(
            string outputPath,
            IEnumerable<(string GeneratorName, double TotalValue)> totals,
            IEnumerable<(string GeneratorName, double Emission, DateTime Date)> maxEmissionsPerDay,
            IEnumerable<(string GeneratorName, double HeatRate)> heatRates)
        {
            // Example structure:
            // <GenerationOutput>
            //   <Totals>
            //     <Generator>
            //       <Name>Coal[1]</Name>
            //       <Total>5341.716526632</Total>
            //     </Generator>
            //     ...
            //   </Totals>
            //   <MaxEmissionGenerators>
            //     <Day>
            //       <Name>Coal[1]</Name>
            //       <Date>2017-01-01T00:00:00+00:00</Date>
            //       <Emission>137.175004008</Emission>
            //     </Day>
            //     ...
            //   </MaxEmissionGenerators>
            //   <ActualHeatRates>
            //     <CoalGenerator>
            //       <Name>Coal[1]</Name>
            //       <HeatRate>12.849293200</HeatRate>
            //     </CoalGenerator>
            //   </ActualHeatRates>
            // </GenerationOutput>

            var doc = new XDocument(
                new XElement("GenerationOutput",
                    new XElement("Totals",
                        from t in totals
                        select new XElement("Generator",
                            new XElement("Name", t.GeneratorName),
                            new XElement("Total", t.TotalValue.ToString("F9")) // https://stackoverflow.com/questions/9570796/does-tostringf-lose-precision-relative-to-tostringg
                        )
                    ),
                    new XElement("MaxEmissionGenerators",
                        from m in maxEmissionsPerDay
                        select new XElement("Day",
                            new XElement("Name", m.GeneratorName),
                            new XElement("Date", m.Date.ToString("o")),
                            new XElement("Emission", m.Emission.ToString("F9"))
                        )
                    ),
                    new XElement("ActualHeatRates",
                        from h in heatRates
                        select new XElement("CoalGenerator",
                            new XElement("Name", h.GeneratorName),
                            new XElement("HeatRate", h.HeatRate.ToString("F9"))
                        )
                    )
                )
            );

            Directory.CreateDirectory(Path.GetDirectoryName(outputPath) ?? ".");

            doc.Save(outputPath);
            logger.LogInformation("Successfully wrote generation output to {OutputPath}", outputPath);
        }
    }