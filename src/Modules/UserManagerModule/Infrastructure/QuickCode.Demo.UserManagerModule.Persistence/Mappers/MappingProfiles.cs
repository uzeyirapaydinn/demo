using System;
using System.Linq;
using AutoMapper;
using System.Net;
using QuickCode.Demo.UserManagerModule.Domain.Entities;
using QuickCode.Demo.UserManagerModule.Application.Models;
using QuickCode.Demo.UserManagerModule.Application.Dtos;
using QuickCode.Demo.UserManagerModule.Application.Interfaces.Repositories;

namespace QuickCode.Demo.UserManagerModule.Persistence.Mappers
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
