using Contracts;
using Entities;
using Entities.Helpers;
using Entities.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository
{
    public class ReportRepository : RepositoryBase<Report>, IReportRepository
    {
        private ISortHelper<Report> _sortHelper;
        private IDataShaper<Report> _dataShaper;

        public ReportRepository(RepositoryContext repositoryContext,
            ISortHelper<Report> sortHelper,
            IDataShaper<Report> dataShaper)
            :base(repositoryContext)
        {
            _sortHelper = sortHelper;
            _dataShaper = dataShaper;
        }

        public async Task<Report> GetReportByIdAsync(Guid reportId)
        {
            return await this.FindByCondition(r => r.ReportId.Equals(reportId))
                .FirstOrDefaultAsync();
        }

        public async Task<IEnumerable<Report>> ReportsByOwnerAsync(Guid ownerId, ReportParameters reportParameters)
        {
            var reportList = await this.FindByCondition(r => r.OwnerId.Equals(ownerId))
                .ToListAsync();

            var reports = reportList.AsQueryable();

            var sortedReports = _sortHelper.ApplySort(reports, reportParameters.OrderBy);

            return sortedReports;
        }

        public void CreateReport(Report report)
        {
            Create(report);
        }

        public void UpdateReport(Report report)
        {
            Update(report);
        }

        public void DeleteReport(Report report)
        {
            Delete(report);
        }
    }
}
