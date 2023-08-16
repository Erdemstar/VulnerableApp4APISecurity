using System;
using VulnerableApp4APISecurity.Core.Entities.Account;
using VulnerableApp4APISecurity.Core.Interfaces.Repositories.Base;

namespace VulnerableApp4APISecurity.Core.Interfaces.Repositories.Account
{
    public interface IAccountRepository : IBaseRepository<AccountEntity>
    {
        Task<AccountEntity> GetAccountByEmailPassword(string email, string password);
        Task<AccountEntity> GetAccountByEmail(string email);
        Task<bool> DeleteAccountByEmail(string email);
    }
}

