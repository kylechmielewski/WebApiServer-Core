using Entities.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Contracts
{
    public interface IReportRepository : IRepositoryBase<Report>
    {
        Task<Report> GetReportByIdAsync(Guid reportId);
        Task<IEnumerable<Report>> ReportsByOwnerAsync(Guid ownerId, ReportParameters reportParameters);

        void CreateReport(Report report);
        void UpdateReport(Report report);
        void DeleteReport(Report report);
    }
}
