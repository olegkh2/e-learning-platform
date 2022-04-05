using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace eLearning.Data.Base
{
    public interface IEntityBaseRepository<T> where T: CourseEntity
    {
        Task<IEnumerable<T>> GetAllAsync();
        Task<IEnumerable<T>> GetAllAsync(params Expression<Func<T, object>>[] includeProperties);
        Task<T> GetByIdAsync(int id);
        Task<T> GetByIdAsync(int id, params Expression<Func<T, object>>[] includeProperties);
        Task AddAsync(T entity);
        Task DeleteAsync(int id);
        Task HideAsync(int id);
        Task<int> GetMaxPosition(int id);
        Task<int> GetCourseIdAsync(int id);
    }
}
