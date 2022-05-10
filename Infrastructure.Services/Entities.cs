using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Infrastructure.Services
{
    public interface ITradefactContext : IDisposable
    {
        DatabaseFacade Database { get; }
        ChangeTracker ChangeTracker { get; }
        EntityEntry Add(object entity);
        EntityEntry<TEntity> Add<TEntity>(TEntity entity) where TEntity : class;
        Task<EntityEntry> AddAsync(object entity, CancellationToken cancellationToken = default(CancellationToken));
        Task<EntityEntry<TEntity>> AddAsync<TEntity>(TEntity entity, CancellationToken cancellationToken = default(CancellationToken)) where TEntity : class;
        void AddRange(IEnumerable<object> entities);
        void AddRange(params object[] entities);
        Task AddRangeAsync(IEnumerable<object> entities, CancellationToken cancellationToken = default(CancellationToken));
        Task AddRangeAsync(params object[] entities);
        EntityEntry<TEntity> Attach<TEntity>(TEntity entity) where TEntity : class;
        EntityEntry Attach(object entity);
        void AttachRange(params object[] entities);
        void AttachRange(IEnumerable<object> entities);
        EntityEntry<TEntity> Entry<TEntity>(TEntity entity) where TEntity : class;
        EntityEntry Entry(object entity);
        bool Equals(object obj);
        object Find(Type entityType, params object[] keyValues);
        TEntity Find<TEntity>(params object[] keyValues) where TEntity : class;
        Task<TEntity> FindAsync<TEntity>(params object[] keyValues) where TEntity : class;
        Task<object> FindAsync(Type entityType, object[] keyValues, CancellationToken cancellationToken);
        Task<TEntity> FindAsync<TEntity>(object[] keyValues, CancellationToken cancellationToken) where TEntity : class;
        Task<object> FindAsync(Type entityType, params object[] keyValues);
        int GetHashCode();
        DbSet<TQuery> Query<TQuery>() where TQuery : class;
        EntityEntry Remove(object entity);
        EntityEntry<TEntity> Remove<TEntity>(TEntity entity) where TEntity : class;
        void RemoveRange(IEnumerable<object> entities);
        void RemoveRange(params object[] entities);
        int SaveChanges(bool acceptAllChangesOnSuccess);
        int SaveChanges();
        Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess, CancellationToken cancellationToken = default(CancellationToken));
        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default(CancellationToken));
        DbSet<TEntity> Set<TEntity>() where TEntity : class;
        string ToString();
        EntityEntry Update(object entity);
        EntityEntry<TEntity> Update<TEntity>(TEntity entity) where TEntity : class;
        void UpdateRange(params object[] entities);
        void UpdateRange(IEnumerable<object> entities);
    }



    public interface IEntities<TEntity> where TEntity : class
    {
        TEntity Create(TEntity entity);
        TEntity Delete(TEntity entity);
        int SaveChanges();
        IEnumerable<Expression<Func<TEntity, dynamic>>> CombineExpresssions(params IEnumerable<Expression<Func<TEntity, dynamic>>>[] queries);
    }


    public abstract class Entities<TEntity> : IEntities<TEntity> where TEntity : class
    {
        protected readonly ITradefactContext _context;

        public Entities(ITradefactContext context)
        {
            _context = context;
        }


        public TEntity Create(TEntity entity)
        {
            _context.Set<TEntity>().Attach(entity);
            _context.Entry(entity).State = EntityState.Added;
            return entity;
        }

        public IEnumerable<Expression<Func<TEntity, dynamic>>> CombineExpresssions(params IEnumerable<Expression<Func<TEntity, dynamic>>>[] queries)
        {
            throw new NotImplementedException();
        }


        public TEntity Delete(TEntity entity)
        {
            throw new NotImplementedException();
        }

        public int SaveChanges()
        {
            throw new NotImplementedException();
        }
    }



}
