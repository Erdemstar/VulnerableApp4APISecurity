using System;
using VulnerableApp4APISecurity.Core.Entities.Base;

namespace VulnerableApp4APISecurity.Core.Entities.Account
{
	public class AccountEntity: BaseEntity
	{
        public string? Name { get; set; }
        public string? Surname { get; set; }
        public string? Email { get; set; }
        public string? Password { get; set; }
        public string? Role { get; set; }
    }
}

