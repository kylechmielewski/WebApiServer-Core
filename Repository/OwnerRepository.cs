using Contracts;
using Entities;
using Entities.Helpers;
using Entities.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Repository
{
    public class OwnerRepository : RepositoryBase<Owner>, IOwnerRepository
    {
        private ISortHelper<Owner> _sortHelper;
        private IDataShaper<Owner> _dataShaper;

        public OwnerRepository(RepositoryContext repositoryContext,
            ISortHelper<Owner> sortHelper,
            IDataShaper<Owner> dataShaper)
            :base(repositoryContext)
        {
            _sortHelper = sortHelper;
            _dataShaper = dataShaper;
        }

        public async Task<PagedList<ShapedEntity>> GetOwnersAsync(OwnerParameters ownerParameters)
        {
            var ownersList = await this.FindByCondition(o => o.DateOfBirth.Year >= ownerParameters.MinYearOfBirth
                                                        && o.DateOfBirth.Year <= ownerParameters.MaxYearOfBirth)
                .OrderBy(o => o.Name)
                .ToListAsync();

            var owners = ownersList.AsQueryable();

            SearchByName(ref owners, ownerParameters.Name);

            var sortedOwners = _sortHelper.ApplySort(owners, ownerParameters.OrderBy);
            var shapedOwners = _dataShaper.ShapeData(owners, ownerParameters.Fields);

            return PagedList<ShapedEntity>.ToPagedList(shapedOwners.AsQueryable(), ownerParameters.PageNumber, ownerParameters.PageSize);
        }

        public async Task<IEnumerable<Owner>> GetAllOwnersAsync()
        {
            return await this.FindAll()
                .OrderBy(o => o.Name)
                .ToListAsync();
        }

        public async Task<Owner> GetOwnerByIdAsync(Guid ownerId)
        {
            return await this.FindByCondition(o => o.OwnerId.Equals(ownerId))
                .DefaultIfEmpty(new Owner())
                .FirstOrDefaultAsync();
        }

        public async Task<ShapedEntity> GetOwnerByIdAsync(Guid ownerId, string fields)
        {
            var owner = await this.FindByCondition(o => o.OwnerId.Equals(ownerId))
                .DefaultIfEmpty(new Owner())
                .FirstOrDefaultAsync();

            return _dataShaper.ShapeData(owner, fields);
        }

        public async Task<Owner> GetOwnerWithDetailsAsync(Guid ownerId)
        {
            return await this.FindByCondition(o => o.OwnerId.Equals(ownerId))
                .Include(r => r.Reports)
                .FirstOrDefaultAsync();
        }

        public void CreateOwner(Owner owner)
        {
            Create(owner);
        }

        public void UpdateOwner(Owner owner)
        {
            Update(owner);
        }

        public void DeleteOwner(Owner owner)
        {
            Delete(owner);
        }

        private void SearchByName(ref IQueryable<Owner> owners, string ownerName)
        {
            if (!owners.Any() || string.IsNullOrWhiteSpace(ownerName))
                return;

            owners = owners.Where(o => o.Name.ToLower().Contains(ownerName.Trim().ToLower()));
        }
    }
}
