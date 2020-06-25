using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Contracts;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace WebAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WeatherForecastController : ControllerBase
    {
        private readonly IRepositoryWrapper _repoWrapper;
        private readonly ILoggerManager _logger;

        public WeatherForecastController(ILoggerManager logger, IRepositoryWrapper repositoryWrapper)
        {
            _logger = logger;
            _repoWrapper = repositoryWrapper;
        }

        [HttpGet]
        public IEnumerable<string> Get()
        {
            var domesticReports = _repoWrapper.Report.FindByCondition(x => x.ReportType.Equals("Domestic"));
            var owners = _repoWrapper.Owner.FindAll();


            _logger.LogInfo("Info message.");
            _logger.LogDebug("Debug message.");
            _logger.LogWarn("Warn message.");
            _logger.LogError("Error message.");

            return new string[] { "value1", "value2" };
        }
    }
}
