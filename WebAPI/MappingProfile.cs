using AutoMapper;
using Entities.DataTransferObjects;
using Entities.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebAPI
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            //Owner
            CreateMap<Owner, OwnerDto>();
            CreateMap<OwnerForCreationDto, Owner>();
            CreateMap<OwnerForUpdateDto, Owner>();

            //Report
            CreateMap<Report, ReportDto>();
            CreateMap<ReportForCreationDto, Report>();
            CreateMap<ReportForUpdateDto, Report>();
        }
    }
}
