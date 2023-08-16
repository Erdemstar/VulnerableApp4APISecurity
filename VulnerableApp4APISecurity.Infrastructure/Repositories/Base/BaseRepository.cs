using System;
using MongoDB.Driver;
using System.Linq.Expressions;
using VulnerableApp4APISecurity.Core.Entities.Base;
using VulnerableApp4APISecurity.Core.Interfaces.Repositories.Base;
using VulnerableApp4APISecurity.Core.Interfaces.Utility.Database;

namespace VulnerableApp4APISecurity.Infrastructure.Repositories.Base
{
	public abstract class BaseRepository<T>: IBaseRepository<T> where T : BaseEntity
    {
        protected readonly IMongoDatabase Db;

        private readonly IMongoCollection<T> _collection;

        protected BaseRepository(IDatabaseSettings settings)
        {
            var client = new MongoClient(settings.ConnectionString);
            Db = client.GetDatabase(settings.DatabaseName);
            _collection = Db.GetCollection<T>(typeof(T).Name.Substring(0, typeof(T).Name.Length - 6));
        }

        public virtual async Task<T> GetAsync(Expression<Func<T, bool>> predicate)
        {
            return await _collection.Find(predicate).FirstOrDefaultAsync();
        }

        public virtual async Task<T> GetByIdAsync(string Id)
        {
            return await _collection.Find(x => x.Id == Id).FirstOrDefaultAsync();
        }

        public virtual async Task<long> GetCountAsync(Expression<Func<T, bool>> predicate)
        {
            return await _collection.Find(predicate).CountDocumentsAsync();
        }

        public async Task<List<T>> GetAllAsync()
        {
            return await _collection.Find(x => true).ToListAsync();
        }

        public async Task<List<T>> GetAllAsync(Expression<Func<T, bool>> predicate)
        {
            return await _collection.Find(predicate).ToListAsync();
        }

        public virtual async Task<T> CreateAsync(T entity)
        {
            var options = new InsertOneOptions { BypassDocumentValidation = false };
            await _collection.InsertOneAsync(entity, options);
            return entity;
        }

        public virtual async Task<T> UpdateAsync(string Id, T entity)
        {
            return await _collection.FindOneAndReplaceAsync(x => x.Id == Id, entity);
        }

        public virtual async Task<T> UpdateAsync(T entity, Expression<Func<T, bool>> predicate)
        {
            return await _collection.FindOneAndReplaceAsync(predicate, entity);
        }

        public virtual async Task<T> SoftDeleteAsync(string Id, T entity)
        {
            return await _collection.FindOneAndReplaceAsync(x => x.Id == Id, entity);
        }

        public virtual async Task<T> DeleteAsync(T entity)
        {
            return await _collection.FindOneAndDeleteAsync(x => x.Id == entity.Id);
        }

        public virtual async Task<T> DeleteAsync(string Id)
        {
            return await _collection.FindOneAndDeleteAsync(x => x.Id == Id);
        }

        public virtual async Task<T> DeleteAsync(Expression<Func<T, bool>> filter)
        {
            return await _collection.FindOneAndDeleteAsync(filter);
        }

    }
}

