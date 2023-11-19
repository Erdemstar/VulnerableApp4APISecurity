using System.Linq.Expressions;

namespace VulnerableApp4APISecurity.Core.Interfaces.Repositories.Base;

//Unit of Work Pattern
public interface IBaseRepository<T> where T : class
{
    Task<T> GetAsync(Expression<Func<T, bool>> predicate);
    Task<T> GetByIdAsync(string Id);
    Task<List<T>> GetAllAsync();
    Task<List<T>> GetAllAsync(Expression<Func<T, bool>> predicate);
    Task<long> GetCountAsync(Expression<Func<T, bool>> predicate);
    Task<T> CreateAsync(T entity);
    Task<T> UpdateAsync(string Id, T entity);
    Task<T> UpdateAsync(T entity, Expression<Func<T, bool>> predicate);
    Task<T> SoftDeleteAsync(string Id, T entity);
    Task<T> DeleteAsync(T entity);
    Task<T> DeleteAsync(string Id);
    Task<T> DeleteAsync(Expression<Func<T, bool>> filter);
}