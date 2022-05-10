using Core.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

namespace Flare.Data
{
    public interface IContext
    {
        DatabaseFacade Database { get; }
        ChangeTracker ChangeTracker { get; }
        EntityEntry Add(object entity);
        EntityEntry<TEntity> Entry<TEntity>(TEntity entity) where TEntity : class;
        EntityEntry Entry(object entity);
        int SaveChanges();
        Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess, CancellationToken cancellationToken = default);
        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
        DbSet<TEntity> Set<TEntity>() where TEntity : class;
    }

    // IdentityDbContext<ApplicationUser>
    public abstract class AbstractContext<TContext> : IdentityDbContext<ApplicationUser>, IContext where TContext : IdentityDbContext<ApplicationUser>
    {
        public DbSet<OrganisationType> OrganisationTypes { get; set; }
        public DbSet<Organisation> Organisations { get; set; }

        public Task<int> SaveChangesAsync() => SaveChangesAsync(default);

        public virtual async Task RunMigrationsAsync(CancellationToken cancellationToken)
            => await Database.MigrateAsync(cancellationToken);

        public abstract bool IsUniqueConstraintViolationException(DbUpdateException exception);

        public virtual bool SupportsLimitInSubqueries => true;
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
        protected readonly IContext _context;

        public Entities(IContext context)
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
            return queries.SelectMany(q => q);
        }

        public IEnumerable<Expression<Func<TEntity, dynamic>>> CombineExpresssionsObject(params IEnumerable<Expression<Func<TEntity, dynamic>>>[] queries)
        {
            return queries.SelectMany(q => q);
        }

        public TEntity Delete(TEntity entity)
        {
            _context.Set<TEntity>().Attach(entity);
            _context.Entry(entity).State = EntityState.Deleted;
            return entity;
        }

        public int SaveChanges()
        {
            return _context.SaveChanges();
        }
    }
}
