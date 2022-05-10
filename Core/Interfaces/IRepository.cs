using Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Interfaces
{
    public interface IRepository
    {
        Task<T> GetByIdAsync<T>(Guid id) where T : BaseEntity<T>;
        Task<List<T>> ListAsync<T>() where T : BaseEntity<T>;
        Task<T> AddAsync<T>(T entity) where T : BaseEntity<T>;
        Task UpdateAsync<T>(T entity) where T : BaseEntity<T>;
        Task DeleteAsync<T>(T entity) where T : BaseEntity<T>;
    }
}
