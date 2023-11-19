using MongoDB.Driver;
using VulnerableApp4APISecurity.Core.Entities.Card;
using VulnerableApp4APISecurity.Core.Interfaces.Repositories.Card;
using VulnerableApp4APISecurity.Core.Interfaces.Utility.Database;
using VulnerableApp4APISecurity.Infrastructure.Repositories.Base;

namespace VulnerableApp4APISecurity.Infrastructure.Repositories.Card;

public class CardRepository : BaseRepository<CardEntity>, ICardRepository
{
    private readonly IMongoCollection<CardEntity> _card;

    public CardRepository(IDatabaseSettings settings) : base(settings)
    {
        var client = new MongoClient(settings.ConnectionString);
        var database = client.GetDatabase(settings.DatabaseName);

        _card = database.GetCollection<CardEntity>("Card");
    }

    public async Task<List<CardEntity>> GetCard(string UserId)
    {
        return await _card.Find(card => card.UserId == UserId).ToListAsync();
    }

    public async Task<bool> DeleteCard(string cardId)
    {
        try
        {
            await _card.DeleteOneAsync(card => card.Id == cardId);
            return true;
        }
        catch
        {
            return false;
        }
    }
}