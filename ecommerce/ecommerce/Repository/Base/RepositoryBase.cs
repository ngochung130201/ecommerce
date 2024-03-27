using ecommerce.Context;
using ecommerce.Middleware;
using ecommerce.Repository.Interface;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace ecommerce.Repository.Base
{
    public class RepositoryBase<T> : IRepositoryBase<T> where T : class
    {
        private readonly EcommerceContext _repositoryContext;

        public RepositoryBase(EcommerceContext repositoryContext)
        {
            _repositoryContext = repositoryContext;
        }

        public async Task<IEnumerable<T>> FindAllAsync()
        {
            return await _repositoryContext.Set<T>().ToListAsync();
        }

        public async Task<IEnumerable<T>> FindByConditionAsync(Expression<Func<T, bool>> expression)
        {
            return await _repositoryContext.Set<T>().Where(expression).ToListAsync();
        }

        public async Task<T> FindByIdAsync(int id)
        {
            return await _repositoryContext.Set<T>().FindAsync(id);
        }

        public void Create(T entity)
        {
            _repositoryContext.Set<T>().Add(entity);
        }

        public void Update(T entity)
        {
            _repositoryContext.Set<T>().Update(entity);
        }

        public void Delete(T entity)
        {
            _repositoryContext.Set<T>().Remove(entity);
        }

        public void CreateRange(IEnumerable<T> entities)
        {
            var list = entities.ToList();
            _repositoryContext.Set<T>().AddRange(list);
        }

        public void UpdateRange(IEnumerable<T> entities)
        {
            var list = entities.ToList();
            _repositoryContext.Set<T>().UpdateRange(list);
        }

        public void DeleteRange(IEnumerable<T> entities)
        {
            var list = entities.ToList();
            _repositoryContext.Set<T>().RemoveRange(list);
        }

        public async Task<IEnumerable<T>> FindByRangeAsync(int skip, int take)
        {
            var entities = await _repositoryContext.Set<T>().Skip(skip).Take(take).ToListAsync();
            return entities;
            
        }

        public async Task<IEnumerable<T>> FindByConditionAndRangeAsync(Expression<Func<T, bool>> expression, int skip, int take)
        {
            var entities = await _repositoryContext.Set<T>().Where(expression).Skip(skip).Take(take).ToListAsync();
            return entities;
        }

        public async Task<IEnumerable<T>> FindByListOfIdsAsync(List<int> ids)
        {
            var entities = new List<T>();
            foreach (var id in ids)
            {
                var entity = await _repositoryContext.Set<T>().FindAsync(id);
                if (entity == null)
                {
                    throw new CustomException("Entity not found", 404);
                }
                entities.Add(entity);
            }
            return entities;
        }
    }
}