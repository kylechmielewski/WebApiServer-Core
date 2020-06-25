using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Contracts
{
    public interface IRepositoryWrapper
    {
        IOwnerRepository Owner { get; }
        IReportRepository Report { get; }
        Task SaveAsync();
    }
}
