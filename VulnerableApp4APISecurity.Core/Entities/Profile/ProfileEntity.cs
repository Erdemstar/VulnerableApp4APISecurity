using System;
using VulnerableApp4APISecurity.Core.Entities.Base;

namespace VulnerableApp4APISecurity.Core.Entities.Profile
{
	public class ProfileEntity: BaseEntity
	{
        public string? Email { get; set; }
        public string? Hobby { get; set; }
        public string? Address { get; set; }
        public DateTime? Birthday { get; set; }
    }
}

