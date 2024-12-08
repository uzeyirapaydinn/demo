using System;
using System.Linq;
using AutoMapper;
using System.Net;
using QuickCode.Demo.SmsManagerModule.Domain.Entities;
using QuickCode.Demo.SmsManagerModule.Application.Models;
using QuickCode.Demo.SmsManagerModule.Application.Dtos;
using QuickCode.Demo.SmsManagerModule.Application.Interfaces.Repositories;

namespace QuickCode.Demo.SmsManagerModule.Persistence.Mappers
{
    public class MappingProfiles : Profile
    {
        public MappingProfiles()
        {
            CreateMap(typeof(DLResponse<>), typeof(Response<>));
            AddDtoMappers();
        }
        
        void AddDtoMappers()
        {
            var dtoTypes = typeof(IBaseRepository<>).Assembly.GetTypes().Where(i => i.Namespace != null && i.Namespace.EndsWith("Application.Dtos"));
            var entityTypes = typeof(BaseDomainEntity).Assembly.GetTypes().Where(i => i.Namespace != null && i.Namespace.EndsWith("Domain.Entities")).ToList();
        
            foreach (var dtoType in dtoTypes)
            {
                var entityType = entityTypes.Find(i => i.Name == ($"{dtoType.Name.Replace("Dto", string.Empty)}"));
                if (entityType == null) continue;
                CreateMap(dtoType, entityType);
                CreateMap(entityType, dtoType);
            }
        }
    }
}
