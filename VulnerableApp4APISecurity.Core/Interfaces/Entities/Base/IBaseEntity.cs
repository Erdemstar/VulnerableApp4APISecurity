namespace VulnerableApp4APISecurity.Core.Interfaces.Entities.Base;

public interface IBaseEntity
{
    string? Id { get; }
    DateTime? CreatedAt { get; set; }
}