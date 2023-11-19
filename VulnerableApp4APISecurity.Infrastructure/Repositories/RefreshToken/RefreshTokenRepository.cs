using MongoDB.Driver;
using VulnerableApp4APISecurity.Core.Entities.RefreshToken;
using VulnerableApp4APISecurity.Core.Interfaces.Repositories.RefreshToken;
using VulnerableApp4APISecurity.Core.Interfaces.Utility.Database;
using VulnerableApp4APISecurity.Infrastructure.Repositories.Base;

namespace VulnerableApp4APISecurity.Infrastructure.Repositories.RefreshToken;

public class RefreshTokenRepository : BaseRepository<RefreshTokenEntity>, IRefreshTokenRepository
{
    private readonly IMongoCollection<RefreshTokenEntity> _refreshToken;

    public RefreshTokenRepository(IDatabaseSettings settings) : base(settings)
    {
        var client = new MongoClient(settings.ConnectionString);
        var database = client.GetDatabase(settings.DatabaseName);

        _refreshToken = database.GetCollection<RefreshTokenEntity>("RefreshToken");
    }
}