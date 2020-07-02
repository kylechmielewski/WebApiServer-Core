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
            CreateMap<Owner, OwnerWithReportsDto>();
            CreateMap<OwnerForCreationDto, Owner>();
            CreateMap<OwnerForUpdateDto, Owner>();
            //CreateMap<OwnerWithReportsDto, Owner>();
            

            //Report
            CreateMap<Report, ReportDto>();
            CreateMap<ReportForCreationDto, Report>();
            CreateMap<ReportForUpdateDto, Report>();
        }
    }
}
