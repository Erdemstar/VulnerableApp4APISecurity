using System;
namespace VulnerableApp4APISecurity.Core.Interfaces.Utility.JWT
{
	public interface IJWTSettings
	{
        string Key { get; set; }
        string Issuer { get; set; }
    }
}

