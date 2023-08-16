using System;
using VulnerableApp4APISecurity.Core.Entities.Account;
using VulnerableApp4APISecurity.Core.Entities.Profile;
using VulnerableApp4APISecurity.Core.Interfaces.Repositories.Base;

namespace VulnerableApp4APISecurity.Core.Interfaces.Repositories.Profile
{
	public interface IProfileRepository: IBaseRepository<ProfileEntity>
	{
        Task<ProfileEntity> GetProfileByEmail(string email);
        Task<bool> DeleteProfile(string id);
    }
}

