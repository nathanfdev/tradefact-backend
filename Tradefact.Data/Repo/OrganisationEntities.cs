using Core.Enums;
using Core.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Tradefact.Data.Repo
{
    public interface IRepositoryBase<T>
    {
        IQueryable<T> Table();
        IQueryable<T> FindByCondition(Expression<Func<T, bool>> expression);
        void Create(T entity);
        void Update(T entity);
        void Delete(T entity);
    }

    public abstract class RepositoryBase<T> : IRepositoryBase<T> where T : class
    {
        protected TradefactDbContext RepositoryContext { get; set; }

        public RepositoryBase(TradefactDbContext repositoryContext)
        {
            this.RepositoryContext = repositoryContext;
        }

        public IQueryable<T> Table() => this.RepositoryContext.Set<T>().AsNoTracking();

        public IQueryable<T> FindByCondition(Expression<Func<T, bool>> expression)
        {
            return this.RepositoryContext.Set<T>()
                .Where(expression).AsNoTracking();
        }

        public void Create(T entity)
        {
            this.RepositoryContext.Set<T>().Add(entity);
        }

        public void Update(T entity)
        {
            this.RepositoryContext.Set<T>().Update(entity);
        }

        public void Delete(T entity)
        {
            this.RepositoryContext.Set<T>().Remove(entity);
        }
    }

    public interface IOrganisationRepository : IRepositoryBase<Organisation>
    {
        Task<Organisation> GetAccount(Guid id);
    }


    public class OrganisationRepository : RepositoryBase<Organisation>, IOrganisationRepository
    {
        public OrganisationRepository(TradefactDbContext repositoryContext)
            : base(repositoryContext)
        {
        }

        public Task<Organisation> GetAccount(Guid id)
        {
            return FindByCondition(o => o.Id.Equals(id)).FirstOrDefaultAsync();
        }
    }





    public interface ITradefactRepository
    {

    }

    public class TradefactRepository : ITradefactRepository
    {
        private readonly TradefactDbContext _context;

        public TradefactRepository(TradefactDbContext context)
        {
            _context = context;
        }

        public IQueryable<Organisation> GetOrganisations()
        {
            return _context.Organisations.Where(x => x.IsActive);
        }

        public IQueryable<Organisation> GetDirectory(Guid parentId, OrganisationTypeEnum organisationType)
        {
            return GetOrganisations().Where(x => x.ParentId == parentId  && x.OrganisationTypeId == organisationType);
        }

        public async Task<Organisation> GetDirectoryEntryById(Guid parentId, Guid id, OrganisationTypeEnum organisationType)
        {
            return await GetDirectory(parentId, organisationType).SingleOrDefaultAsync<Organisation>(x => x.Id == id);
        }

    }
}
