using Contracts;
using Entities;
using Entities.Helpers;
using Entities.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Repository
{
    public class RepositoryWrapper : IRepositoryWrapper
    {
        private RepositoryContext _repoContext;
        private IOwnerRepository _owner;
        private IReportRepository _report;
        private ISortHelper<Owner> _ownerSortHelper;
        private ISortHelper<Report> _reportSortHelper;
        private IDataShaper<Owner> _ownerDataShaper;
        private IDataShaper<Report> _reportDataShaper;

        public IOwnerRepository Owner
        {
            get
            {
                if (_owner == null)
                {
                    _owner = new OwnerRepository(_repoContext, _ownerSortHelper, _ownerDataShaper);
                }
                return _owner;
            }
        }

        public IReportRepository Report
        {
            get
            {
                if (_report == null)
                {
                    _report = new ReportRepository(_repoContext, _reportSortHelper, _reportDataShaper);
                }
                return _report;
            }
        }

        public RepositoryWrapper(RepositoryContext repositoryContext,
            ISortHelper<Owner> ownerSortHelper,
            ISortHelper<Report> reportSortHelper,
            IDataShaper<Owner> ownerDataShaper,
            IDataShaper<Report> reportDataShaper)
        {
            _repoContext = repositoryContext;
            _ownerSortHelper = ownerSortHelper;
            _reportSortHelper = reportSortHelper;
            _ownerDataShaper = ownerDataShaper;
            _reportDataShaper = reportDataShaper;
        }

        public async Task SaveAsync()
        {
            await _repoContext.SaveChangesAsync();
        }
    }
}
