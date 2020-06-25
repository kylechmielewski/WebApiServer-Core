using Entities.Helpers;
using Entities.Models;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Text;
using System.Threading.Tasks;

namespace Contracts
{
    public interface IOwnerRepository : IRepositoryBase<Owner>
    {
        Task<PagedList<ShapedEntity>> GetOwnersAsync(OwnerParameters ownerParameters);
        Task<ShapedEntity> GetOwnerByIdAsync(Guid ownerId, string fields);
        Task<IEnumerable<Owner>> GetAllOwnersAsync();
        Task<Owner> GetOwnerByIdAsync(Guid ownerId);
        Task<Owner> GetOwnerWithDetailsAsync(Guid ownerId);
        void CreateOwner(Owner owner);
        void UpdateOwner(Owner owner);
        void DeleteOwner(Owner owner);
    }
}
