using System.Linq.Expressions;

namespace ecommerce.Repository.Interface
{
    public interface IRepositoryBase<T> where T : class
    {
        Task<IEnumerable<T>> FindAllAsync();
        Task<IEnumerable<T>> FindByConditionAsync(Expression<Func<T, bool>> expression);
        Task<T> FindByIdAsync(int id);
        void Create(T entity);
        void CreateRange(IEnumerable<T> entities);
        void Update(T entity);
        void UpdateRange(IEnumerable<T> entities);
        void Delete(T entity);
        void DeleteRange(IEnumerable<T> entities);
        // get list in a range
        Task<IEnumerable<T>> FindByRangeAsync(int skip, int take);
        // Find by condition and range
        Task<IEnumerable<T>> FindByConditionAndRangeAsync(Expression<Func<T, bool>> expression, int skip, int take);
        // Find by list of ids
        Task<IEnumerable<T>> FindByListOfIdsAsync(List<int> ids);
    }
}
