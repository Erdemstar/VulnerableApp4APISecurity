using System;
using VulnerableApp4APISecurity.Core.Entities.Base;

namespace VulnerableApp4APISecurity.Core.Entities.Card
{
	public class CardEntity: BaseEntity
	{
        public string? UserId { get; set; }
        public string? Nickname { get; set; }
        public string? Number { get; set; }
        public string? ExpireDate { get; set; }
        public string? Cve { get; set; }
        public string? Password { get; set; }
    }
}

