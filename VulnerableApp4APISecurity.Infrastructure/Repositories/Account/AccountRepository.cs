using MongoDB.Driver;
using VulnerableApp4APISecurity.Core.Entities.Account;
using VulnerableApp4APISecurity.Core.Interfaces.Repositories.Account;
using VulnerableApp4APISecurity.Core.Interfaces.Utility.Database;
using VulnerableApp4APISecurity.Infrastructure.Repositories.Base;

namespace VulnerableApp4APISecurity.Infrastructure.Repositories.Account;

public class AccountRepository : BaseRepository<AccountEntity>, IAccountRepository
{
    private readonly IMongoCollection<AccountEntity> _account;

    public AccountRepository(IDatabaseSettings settings) : base(settings)
    {
        var client = new MongoClient(settings.ConnectionString);
        var database = client.GetDatabase(settings.DatabaseName);

        _account = database.GetCollection<AccountEntity>("Account");
    }

    public async Task<AccountEntity> GetAccountByEmailPassword(string email, string password)
    {
        return await _account.Find(ac => ac.Email == email && ac.Password == password).FirstOrDefaultAsync();
    }

    public async Task<AccountEntity> GetAccountByEmail(string email)
    {
        return await _account.Find(ac => ac.Email == email).FirstOrDefaultAsync();
    }

    public async Task<bool> DeleteAccountByEmail(string email)
    {
        var status = await _account.DeleteOneAsync(ac => ac.Email == email);
        return status.DeletedCount > 0;
    }
}