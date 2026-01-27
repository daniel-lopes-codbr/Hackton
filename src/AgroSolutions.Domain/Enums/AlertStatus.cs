namespace AgroSolutions.Domain.Enums;

/// <summary>
/// Alert status enumeration
/// </summary>
public enum AlertStatus
{
    /// <summary>
    /// Normal conditions - no alert needed
    /// </summary>
    Normal = 0,

    /// <summary>
    /// Drought alert - soil moisture below threshold for extended period
    /// </summary>
    DroughtAlert = 1,

    /// <summary>
    /// Pest risk alert - conditions favorable for pest development
    /// </summary>
    PestRisk = 2
}
