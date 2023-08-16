using System;
using MongoDB.Driver;
using VulnerableApp4APISecurity.Core.Entities.Profile;
using VulnerableApp4APISecurity.Core.Interfaces.Repositories.Profile;
using VulnerableApp4APISecurity.Core.Interfaces.Utility.Database;
using VulnerableApp4APISecurity.Infrastructure.Repositories.Base;

namespace VulnerableApp4APISecurity.Infrastructure.Repositories.Profile
{
	public class ProfileRepository: BaseRepository<ProfileEntity>, IProfileRepository
    {
        private readonly IMongoCollection<ProfileEntity> _profile;

        public ProfileRepository(IDatabaseSettings settings) : base(settings)
        {
            var client = new MongoClient(settings.ConnectionString);
            var database = client.GetDatabase(settings.DatabaseName);

            _profile = database.GetCollection<ProfileEntity>("Profile");
        }

        public async Task<ProfileEntity> GetProfileByEmail(string email)
        {
            return await _profile.Find(prof => prof.Email == email).FirstOrDefaultAsync();
        }

        public async Task<bool> DeleteProfile(string id)
        {
            var deleteResult = await _profile.DeleteOneAsync(ent => ent.Id == id);
            return deleteResult.IsAcknowledged && deleteResult.DeletedCount > 0;
        }
    }
}

