using System;
using VulnerableApp4APISecurity.Core.Interfaces.Utility.JWT;

namespace VulnerableApp4APISecurity.Infrastructure.Utility.JWT
{
	public class JWTSettings: IJWTSettings
    {
        public string? Key { get; set; }
        public string? Issuer { get; set; }
    }
}

