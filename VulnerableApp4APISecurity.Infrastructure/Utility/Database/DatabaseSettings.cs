using VulnerableApp4APISecurity.Core.Interfaces.Utility.Database;

namespace VulnerableApp4APISecurity.Infrastructure.Utility.Database;

public class DatabaseSettings : IDatabaseSettings
{
    public string? ConnectionString { get; set; }
    public string? DatabaseName { get; set; }
}