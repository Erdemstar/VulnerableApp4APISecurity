using System;
namespace VulnerableApp4APISecurity.Core.DTO.Account
{
	public class RegisterRequest
	{
        public string? Name { get; set; }
        public string? Surname { get; set; }
        public string? Email { get; set; }
        public string? Password { get; set; }
    }
}

