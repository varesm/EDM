using EDM.Core.Models;

namespace EDM.Infrastructure.Parsing;

public interface IReferenceDataRepository
{
    /// <summary>
    /// Reads and parses ReferenceData.xml, returning the reference factors.
    /// </summary>
    ReferenceFactors LoadReferenceFactors(string xmlFilePath);
}