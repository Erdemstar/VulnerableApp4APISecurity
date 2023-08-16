using System;
namespace VulnerableApp4APISecurity.Core.Interfaces.Utility.Database
{
	public interface IDatabaseSettings
	{
        string ConnectionString { get; set; }
        string DatabaseName { get; set; }
    }
}

