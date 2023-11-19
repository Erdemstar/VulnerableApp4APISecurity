using VulnerableApp4APISecurity.Core.Entities.Card;
using VulnerableApp4APISecurity.Core.Interfaces.Repositories.Base;

namespace VulnerableApp4APISecurity.Core.Interfaces.Repositories.Card;

public interface ICardRepository : IBaseRepository<CardEntity>
{
    Task<List<CardEntity>> GetCard(string UserId);
    Task<bool> DeleteCard(string CardId);
}