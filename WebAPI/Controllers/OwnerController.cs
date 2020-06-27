using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Contracts;
using Entities.DataTransferObjects;
using Entities.Helpers;
using Entities.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Microsoft.Net.Http.Headers;
using Newtonsoft.Json;
using WebAPI.Filters;

namespace WebAPI.Controllers
{
    [Route("api/owner")]
    [ApiController]
    public class OwnerController : ControllerBase
    {
        private ILoggerManager _logger;
        private IRepositoryWrapper _repoWrapper;
        private IMapper _mapper;
        private LinkGenerator _linkGenerator;

        public OwnerController(ILoggerManager logger,
            IRepositoryWrapper repository,
            IMapper mapper,
            LinkGenerator linkGenerator)
        {
            _logger = logger;
            _repoWrapper = repository;
            _mapper = mapper;
            _linkGenerator = linkGenerator;
        }

        [HttpGet]
        //[ServiceFilter(typeof(ValidateMediaTypeAttribute))]
        public async Task<IActionResult> GetOwners([FromQuery] OwnerParameters ownerParameters)
        {
            if (!ownerParameters.ValidYearRange)
            {
                return BadRequest("Max year of birth cannot be less than min year of birth.");
            }

            var owners = await _repoWrapper.Owner.GetOwnersAsync(ownerParameters);

            var metadata = new
            {
                owners.TotalCount,
                owners.PageSize,
                owners.CurrentPage,
                owners.TotalPages,
                owners.HasNext,
                owners.HasPrevious
            };

            Response.Headers.Add("X-Pagination", JsonConvert.SerializeObject(metadata));

            _logger.LogInfo($"Returned {owners.Count()} owners from database.");

            var shapedOwners = owners.Select(o => o.Entity).ToList();
            var mediaType = (MediaTypeHeaderValue)HttpContext.Items["AcceptHeaderMediaType"];

            if (!mediaType.SubTypeWithoutSuffix.EndsWith("hateoas", StringComparison.InvariantCultureIgnoreCase))
            {
                return Ok(shapedOwners);
            }

            for (var index = 0; index < owners.Count; index++)
            {
                var ownerLinks = CreateLinksForOwner(owners[index].Id, ownerParameters.Fields);
                shapedOwners[index].TryAdd("Links", ownerLinks);
            }

            var ownersWrapper = new LinkCollectionWrapper<Entity>(shapedOwners);

            return Ok(CreateLinksForOwners(ownersWrapper));
        }

        //[HttpGet]
        //public async Task<IActionResult> GetAllOwners()
        //{
        //    var owners = await _repoWrapper.Owner.GetAllOwnersAsync();
        //    _logger.LogInfo($"Returned all owners from database.");
            
        //    var ownersResult = _mapper.Map<IEnumerable<OwnerDto>>(owners);
            
        //    return Ok(ownersResult);
        //}

        [HttpGet("{id}", Name = "OwnerById")]
        public async Task<IActionResult> GetOwnerById(Guid id, [FromQuery] string fields)
        {
            var owner = await _repoWrapper.Owner.GetOwnerByIdAsync(id, fields);

            if (owner.Id == Guid.Empty)
            {
                _logger.LogError($"Owner with id: {id}, hasn't been found in db.");
                return NotFound();
            }

            var mediaType = (MediaTypeHeaderValue)HttpContext.Items["AcceptHeaderMediaType"];

            if (!mediaType.SubTypeWithoutSuffix.EndsWith("hateoas", StringComparison.InvariantCultureIgnoreCase))
            {
                _logger.LogInfo($"Returned shaped owner with id: {id}");
                return Ok(owner.Entity);
            }

            owner.Entity.Add("Links", CreateLinksForOwner(owner.Id, fields));

            return Ok(owner.Entity);
        }

        [HttpGet("{id}/report")]
        public async Task<IActionResult> GetOwnerWithDetails(Guid id)
        {
            var owner = await _repoWrapper.Owner.GetOwnerWithDetailsAsync(id);

            if (owner == null)
            {
                _logger.LogError($"Owner with id: {id}, not found.");
                return NotFound();
            }
            else
            {
                _logger.LogInfo($"Returned owner with details for id: {id}");
                var ownerResult = _mapper.Map<OwnerWithReportsDto>(owner);
                return Ok(ownerResult);
            }
        }

        //Because the incoming object is a complex type, [FromBody] is required
        [HttpPost]
        public async Task<IActionResult> CreateOwner([FromBody]OwnerForCreationDto owner)
        {
            if (owner == null)
            {
                _logger.LogError("Owner object sent from client is null.");
                return BadRequest("Owner object is null");
            }

            //This checks all of the validation attributes on the object to make sure the values are valid
            if (!ModelState.IsValid)
            {
                _logger.LogError("Invalid owner object sent from client.");
                return BadRequest("Invalid model object");
            }

            var ownerEntity = _mapper.Map<Owner>(owner);

            _repoWrapper.Owner.CreateOwner(ownerEntity);
            await _repoWrapper.SaveAsync();

            var createdOwner = _mapper.Map<OwnerDto>(ownerEntity);
            return CreatedAtRoute("OwnerById", new { id = createdOwner.Id }, createdOwner);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateOwner(Guid id, [FromBody]OwnerForUpdateDto owner)
        {
            if (owner == null)
            {
                _logger.LogError("Owner object sent from client is null.");
                return BadRequest("Owner object is null");
            }
            if (!ModelState.IsValid)
            {
                _logger.LogError("Invalid owner object sent from client.");
                return BadRequest("Invalid model object");
            }

            var ownerEntity = await _repoWrapper.Owner.GetOwnerByIdAsync(id);
            if (ownerEntity == null)
            {
                _logger.LogError($"Owner with id: {id}, not found.");
                return NotFound();
            }

            _mapper.Map(owner, ownerEntity);
            _repoWrapper.Owner.UpdateOwner(ownerEntity);
            await _repoWrapper.SaveAsync();
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteOwner(Guid id)
        {
            var owner = await _repoWrapper.Owner.GetOwnerByIdAsync(id);
            if (owner == null)
            {
                _logger.LogError($"Owner with id: {id}, hasn't been found in db.");
                return NotFound();
            }
            var reportsByOwner = await _repoWrapper.Report.ReportsByOwnerAsync(id, new ReportParameters());
            if (reportsByOwner.Any())
            {
                _logger.LogError($"Cannot delete owner with id: {id}. It has related reports/account. Delete those first.");
                return BadRequest("Cannot delete owner. It has related reports/accounts. Delete those first.");
            }

            _repoWrapper.Owner.DeleteOwner(owner);
            await _repoWrapper.SaveAsync();

            return NoContent();
        }

        private IEnumerable<Link> CreateLinksForOwner(Guid id, string fields = "")
        {
            var links = new List<Link>
            {
                new Link(_linkGenerator.GetUriByAction(HttpContext, nameof(GetOwnerById), values: new { id, fields }),
                "self",
                "GET"),

                new Link(_linkGenerator.GetUriByAction(HttpContext, nameof(DeleteOwner), values: new { id }),
                "delete_owner",
                "DELETE"),

                new Link(_linkGenerator.GetUriByAction(HttpContext, nameof(UpdateOwner), values: new { id }),
                "update_owner",
                "PUT")
            };
            return links;
        }

        private LinkCollectionWrapper<Entity> CreateLinksForOwners(LinkCollectionWrapper<Entity> ownersWrapper)
        {
            ownersWrapper.Links.Add(new Link(_linkGenerator.GetUriByAction(HttpContext, nameof(GetOwners), values: new { }),
                    "self",
                    "GET"));

            return ownersWrapper;
        }
    }
}
