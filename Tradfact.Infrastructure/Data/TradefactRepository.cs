using Core.Interfaces;
using Core.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Tradefact.Data;

namespace Tradefact.Infrastructure.Data
{

    ///// <summary>
    ///// Implements IRepository for Entity Framework.
    ///// </summary>
    ///// <typeparam name="TDbContext">DbContext which contains <typeparamref name="TEntity"/>.</typeparam>
    ///// <typeparam name="TEntity">Type of the Entity for this repository</typeparam>
    ///// <typeparam name="TPrimaryKey">Primary key of the entity</typeparam>
    //public class EfRepositoryBase<TDbContext, TEntity, TPrimaryKey> : AbpRepositoryBase<TEntity, TPrimaryKey>, IRepositoryWithDbContext, ISupportsExplicitLoading<TEntity, TPrimaryKey>
    //    where TEntity : class, BaseEntity<TPrimaryKey>
    //    where TDbContext : DbContext
    //{
    //    public CancellationToken Token => CancellationToken.None;
    //    /// <summary>
    //    /// Gets EF DbContext object.
    //    /// </summary>
    //    public virtual TDbContext Context => _dbContextProvider.GetDbContext(MultiTenancySide);

    //    /// <summary>
    //    /// Gets DbSet for given entity.
    //    /// </summary>
    //    public virtual DbSet<TEntity> Table => Context.Set<TEntity>();

    //    public IQueryable<TEntity> GetAll()
    //    {
    //        return Table;
    //    }

    //    public async Task<List<TEntity>> GetAllListAsync()
    //    {
    //        return await GetAll().ToListAsync(Token);
    //    }

    //    public async Task<List<TEntity>> GetAllListAsync(Expression<Func<TEntity, bool>> predicate)
    //    {
    //        return await GetAll().Where(predicate).ToListAsync(Token);
    //    }

    //    public async Task<TEntity> SingleAsync(Expression<Func<TEntity, bool>> predicate)
    //    {
    //        return await GetAll().SingleAsync(predicate, Token);
    //    }

    //    public async Task<TEntity> FirstOrDefaultAsync(TPrimaryKey id)
    //    {
    //        return await GetAll().FirstOrDefaultAsync(CreateEqualityExpressionForId(id), Token);
    //    }

    //    public async Task<TEntity> FirstOrDefaultAsync(Expression<Func<TEntity, bool>> predicate)
    //    {
    //        return await GetAll().FirstOrDefaultAsync(predicate, Token);
    //    }

    //    protected static Expression<Func<TEntity, bool>> CreateEqualityExpressionForId(TPrimaryKey id)
    //    {
    //        ParameterExpression lambdaParam = Expression.Parameter(typeof(TEntity));

    //        var value = Convert.ChangeType(id, typeof(TPrimaryKey));
    //        var valueExpression = Expression.Constant(value, typeof(TPrimaryKey));

    //        BinaryExpression lambdaBody = Expression.Equal(
    //            Expression.PropertyOrField(lambdaParam, "Id"),
    //            valueExpression
    //        );

    //        return Expression.Lambda<Func<TEntity, bool>>(lambdaBody, lambdaParam);
    //    }
    //}

    ///// <summary>
    ///// Base class for custom repositories of the application.
    ///// </summary>
    ///// <typeparam name="TEntity">Entity type</typeparam>
    ///// <typeparam name="TPrimaryKey">Primary key type of the entity</typeparam>
    //public abstract class BackendRepositoryBase<TEntity, TPrimaryKey> : EfCoreRepositoryBase<TradefactDbContext, TEntity, TPrimaryKey>
    //    where TEntity : class, IEntity<TPrimaryKey>
    //{
    //    protected BackendRepositoryBase(IDbContextProvider<BackendDbContext> dbContextProvider)
    //        : base(dbContextProvider)
    //    {
    //    }

    //    // Add your common methods for all repositories
    //}

    ///// <summary>
    ///// Base class for custom repositories of the application.
    ///// This is a shortcut of <see cref="BackendRepositoryBase{TEntity,TPrimaryKey}"/> for <see cref="int"/> primary key.
    ///// </summary>
    ///// <typeparam name="TEntity">Entity type</typeparam>
    //public abstract class BackendRepositoryBase<TEntity> : BackendRepositoryBase<TEntity, int>, IRepository<TEntity>
    //    where TEntity : class, IEntity<int>
    //{
    //    protected BackendRepositoryBase(IDbContextProvider<BackendDbContext> dbContextProvider)
    //        : base(dbContextProvider)
    //    {
    //    }

    //    // Do not add any method here, add to the class above (since this inherits it)!!!
    //}


    //public interface IRepository<T> where T : BaseEntity

    //{

    //    T GetById(Int64 id);

    //    void Create(T entity);

    //    void Delete(T entity);

    //    void Update(T entity);

    //}


    //public class TradefactRepository: IRepository
    //{
    //    protected readonly TradefactDbContext _context;

    //    public TradefactRepository(TradefactDbContext context)
    //    {
    //        _context = context;
    //    }

    //    /// <summary>
    //    /// Gets DbSet for given entity.
    //    /// </summary>
    //    public virtual DbSet<TEntity> Table => Context.Set<TEntity>();

    //    public IQueryable<TEntity> GetAll()
    //    {
    //        return Table;
    //    }

    //    public T GetById<T>(Guid id) where T : BaseEntity<T>
    //    {
    //        return _context.Set<T>().SingleOrDefault(e => e.Id == id);
    //    }

    //    public Task<T> GetByIdAsync<T>(Guid id) where T : BaseEntity<T>
    //    {
    //        return _context.Set<T>().SingleOrDefaultAsync(e => e.Id == id);
    //    }

    //    public Task<List<T>> ListAsync<T>() where T : BaseEntity<T>
    //    {
    //        return _context.Set<T>().ToListAsync();
    //    }

    //    public async Task<T> AddAsync<T>(T entity) where T : BaseEntity<T>
    //    {
    //        await _context.Set<T>().AddAsync(entity);
    //        await _context.SaveChangesAsync();

    //        return entity;
    //    }

    //    public async Task UpdateAsync<T>(T entity) where T : BaseEntity<T>
    //    {
    //        _context.Entry(entity).State = EntityState.Modified;
    //        await _context.SaveChangesAsync();
    //    }

    //    public async Task DeleteAsync<T>(T entity) where T : BaseEntity<T>
    //    {
    //        _context.Set<T>().Remove(entity);
    //        await _context.SaveChangesAsync();
    //    }
    //}
//}
}
