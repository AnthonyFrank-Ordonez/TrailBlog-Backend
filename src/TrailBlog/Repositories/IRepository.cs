namespace TrailBlog.Api.Repositories
{
    public interface IRepository<T> where T : class
    {
        IQueryable<T> GetAll();
        Task<T?> GetByIdAsync(Guid id);
        Task<T> AddAsync(T entity);
        Task<T?> UpdateAsync(Guid id, T entity);
        Task DeleteAsync(T entity);
    }
}
