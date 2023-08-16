using System;
namespace VulnerableApp4APISecurity.Core.DTO.Account
{
	public class LoginRequest
	{
        public string? Email { get; set; }
        public string? Password { get; set; }
    }
}

