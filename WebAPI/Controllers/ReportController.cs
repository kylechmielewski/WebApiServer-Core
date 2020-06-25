using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Contracts;
using Entities.DataTransferObjects;
using Entities.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace WebAPI.Controllers
{
    [Route("api/report")]
    [ApiController]
    public class ReportController : ControllerBase
    {
        private ILoggerManager _logger;
        private IRepositoryWrapper _repoWrapper;
        private IMapper _mapper;

        public ReportController(ILoggerManager logger, IRepositoryWrapper repository, IMapper mapper)
        {
            _logger = logger;
            _repoWrapper = repository;
            _mapper = mapper;
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetReportsByOwner(Guid ownerId, ReportParameters reportParameters)
        {
            var reports = await _repoWrapper.Report.ReportsByOwnerAsync(ownerId, reportParameters);

            if (reports == null)
            {
                _logger.LogError($"No reports found for Owner: {ownerId}");
                return NotFound();
            }
            else
            {
                _logger.LogInfo($"Returned reports for Owner: {ownerId}");
                var reportsResult = _mapper.Map<ReportDto>(reports);
                return Ok(reportsResult);
            }
        }

        [HttpPost]
        public async Task<IActionResult> CreateReport([FromBody]ReportForCreationDto report)
        {
            if (report == null)
            {
                _logger.LogError("Report object sent from client is null.");
                return BadRequest("Report object is null");
            }

            //This checks all of the validation attributes on the object to make sure the values are valid
            if (!ModelState.IsValid)
            {
                _logger.LogError("Invalid report object sent from client.");
                return BadRequest("Invalid model object");
            }

            var reportEntity = _mapper.Map<Report>(report);
            reportEntity.DateCreated = DateTime.Now;

            _repoWrapper.Report.CreateReport(reportEntity);
            await _repoWrapper.SaveAsync();

            var createdReport = _mapper.Map<ReportDto>(reportEntity);
            return CreatedAtRoute("ReportById", new { id = createdReport.Id }, createdReport);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateReport(Guid id, [FromBody]ReportForUpdateDto report)
        {
            if (report == null)
            {
                _logger.LogError("Report object sent from client is null.");
                return BadRequest("Report object is null");
            }

            //This checks all of the validation attributes on the object to make sure the values are valid
            if (!ModelState.IsValid)
            {
                _logger.LogError("Invalid report object sent from client.");
                return BadRequest("Invalid model object");
            }

            var reportEntity = await _repoWrapper.Report.GetReportByIdAsync(id);
            if (reportEntity == null)
            {
                _logger.LogError($"report with id: {id} not found");
                return NotFound();
            }

            _mapper.Map(report, reportEntity);
            _repoWrapper.Report.UpdateReport(reportEntity);
            await _repoWrapper.SaveAsync();
            return NoContent();
        }

        [HttpDelete("id")]
        public async Task<IActionResult> DeleteReport(Guid id)
        {
            var report = await _repoWrapper.Report.GetReportByIdAsync(id);
            if (report == null)
            {
                _logger.LogError($"Report with id: {id} not found in database.");
                return NotFound();
            }

            _repoWrapper.Report.DeleteReport(report);
            await _repoWrapper.SaveAsync();

            return NoContent();
        }
    }
}
