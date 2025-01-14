//------------------------------------------------------------------------------ 
// <auto-generated> 
// This code was generated by QuickCode. 
// Runtime Version:1.0
// 
// Changes to this file may cause incorrect behavior and will be lost if 
// the code is regenerated. 
// </auto-generated> 
//------------------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Data.SqlClient;
using QuickCode.Demo.UserManagerModule.Application.Models;
using QuickCode.Demo.UserManagerModule.Domain.Entities;
using QuickCode.Demo.UserManagerModule.Application.Interfaces.Repositories;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using QuickCode.Demo.UserManagerModule.Persistence.Contexts;
using QuickCode.Demo.UserManagerModule.Application.Dtos;

namespace QuickCode.Demo.UserManagerModule.Persistence.Repositories
{
    public partial class PortalPermissionTypesRepository : IPortalPermissionTypesRepository
    {
        private readonly WriteDbContext _writeContext;
        private readonly ReadDbContext _readContext;
        private readonly ILogger<PortalPermissionTypesRepository> _logger;
        public PortalPermissionTypesRepository(ILogger<PortalPermissionTypesRepository> logger, WriteDbContext writeContext, ReadDbContext readContext)
        {
            _writeContext = writeContext;
            _readContext = readContext;
            _logger = logger;
        }

        public async Task<DLResponse<PortalPermissionTypes>> InsertAsync(PortalPermissionTypes value)
        {
            var returnValue = new DLResponse<PortalPermissionTypes>(value, "Success");
            try
            {
                await _writeContext.PortalPermissionTypes.AddAsync(value);
                await _writeContext.SaveChangesAsync();
                returnValue = new DLResponse<PortalPermissionTypes>(value, "Success");
            }
            catch (SqlException ex)
            {
                _logger.LogError("{repoName} SqlException {error}", "PortalPermissionTypes Insert", ex.Message);
                if (ex.Number.Equals(2627))
                {
                    returnValue.Code = 999;
                }
                else
                {
                    returnValue.Code = 998;
                }

                returnValue.Message = ex.ToString();
            }
            catch (Exception ex)
            {
                _logger.LogError("{repoName} Exception {error}", "PortalPermissionTypes Insert", ex.Message);
                returnValue.Code = 404;
                returnValue.Message = ex.ToString();
            }

            return returnValue;
        }

        public async Task<DLResponse<bool>> UpdateAsync(PortalPermissionTypes value)
        {
            var returnValue = new DLResponse<bool>(false, "Success");
            try
            {
                _writeContext.Set<PortalPermissionTypes>().Update(value);
                await _writeContext.SaveChangesAsync();
                returnValue.Value = true;
            }
            catch (SqlException ex)
            {
                _logger.LogError("{repoName} SqlException {error}", "PortalPermissionTypes Update", ex.Message);
                if (ex.Number.Equals(2627))
                {
                    returnValue.Code = 999;
                }
                else
                {
                    returnValue.Code = 998;
                }

                returnValue.Message = ex.ToString();
            }
            catch (Exception ex)
            {
                _logger.LogError("{repoName} Exception {error}", "PortalPermissionTypes", ex.Message);
                returnValue.Code = 404;
                returnValue.Message = ex.ToString();
            }

            return returnValue;
        }

        public async Task<DLResponse<bool>> DeleteAsync(PortalPermissionTypes value)
        {
            var returnValue = new DLResponse<bool>(false, "Success");
            try
            {
                _writeContext.PortalPermissionTypes.Remove(value);
                await _writeContext.SaveChangesAsync();
                returnValue.Value = true;
            }
            catch (SqlException ex)
            {
                _logger.LogError("{repoName} SqlException {error}", "PortalPermissionTypes Delete", ex.Message);
                if (ex.Number.Equals(2627))
                {
                    returnValue.Code = 999;
                }
                else
                {
                    returnValue.Code = 998;
                }

                returnValue.Message = ex.ToString();
            }
            catch (Exception ex)
            {
                _logger.LogError("{repoName} Exception {error}", "PortalPermissionTypes Delete", ex.Message);
                returnValue.Code = 404;
                returnValue.Message = ex.ToString();
            }

            return returnValue;
        }

        public async Task<DLResponse<PortalPermissionTypes>> GetByPkAsync(int id)
        {
            var returnValue = new DLResponse<PortalPermissionTypes>();
            try
            {
                var result =
                    from portal_permission_types in _readContext.PortalPermissionTypes
                    where portal_permission_types.Id.Equals(id)select portal_permission_types;
                returnValue.Value = await result.FirstAsync();
                if (returnValue.Value == null)
                {
                    returnValue.Code = 404;
                    returnValue.Message = $"Not found in PortalPermissionTypes";
                    return returnValue;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError("{repoName} Exception {error}", "PortalPermissionTypes GetByPk", ex.Message);
                returnValue.Code = 404;
                returnValue.Message = ex.ToString();
            }

            return returnValue;
        }

        public async Task<DLResponse<List<PortalPermissionTypes>>> ListAsync(int? pageNumber = null, int? pageSize = null)
        {
            var returnValue = new DLResponse<List<PortalPermissionTypes>>();
            try
            {
                if (pageNumber < 1)
                {
                    returnValue.Code = 404;
                    returnValue.Message = "Page Number must be greater than 1";
                }
                else
                {
                    if (pageNumber != null)
                    {
                        var skip = ((pageNumber - 1) * pageSize);
                        var take = pageSize;
                        returnValue.Value = await _readContext.PortalPermissionTypes.Skip(skip.Value).Take(take.Value).ToListAsync();
                    }
                    else
                    {
                        returnValue.Value = await _readContext.PortalPermissionTypes.ToListAsync();
                    }
                }
            }
            catch (Exception ex)
            {
                returnValue.Code = 404;
                returnValue.Message = ex.ToString();
            }

            return returnValue;
        }

        public async Task<DLResponse<int>> CountAsync()
        {
            var returnValue = new DLResponse<int>();
            try
            {
                returnValue.Value = await _readContext.PortalPermissionTypes.CountAsync();
            }
            catch (Exception ex)
            {
                returnValue.Code = 404;
                returnValue.Message = ex.ToString();
            }

            return returnValue;
        }

        public async Task<DLResponse<List<PortalPermissionTypesPortalPermissionGroups_RESTResponseDto>>> PortalPermissionTypesPortalPermissionGroups_RESTAsync(int portalPermissionTypesId)
        {
            var returnValue = new DLResponse<List<PortalPermissionTypesPortalPermissionGroups_RESTResponseDto>>();
            try
            {
                var queryableResult =
                    from portal_permission_groups in _readContext.PortalPermissionGroups
                    join portal_permission_types in _readContext.PortalPermissionTypes on portal_permission_groups.PortalPermissionTypeId equals portal_permission_types.Id
                    where portal_permission_types.Id.Equals(portalPermissionTypesId)select new PortalPermissionTypesPortalPermissionGroups_RESTResponseDto()
                    {
                        Id = portal_permission_groups.Id,
                        PortalPermissionId = portal_permission_groups.PortalPermissionId,
                        PermissionGroupId = portal_permission_groups.PermissionGroupId,
                        PortalPermissionTypeId = portal_permission_groups.PortalPermissionTypeId
                    };
                var result = await queryableResult.ToListAsync();
                returnValue.Value = result;
            }
            catch (Exception ex)
            {
                _logger.LogError("{repoName} Exception {error}", "PortalPermissionTypes PortalPermissionTypesPortalPermissionGroups_REST", ex.Message);
                returnValue.Code = 404;
                returnValue.Message = ex.ToString();
            }

            return returnValue;
        }

        public async Task<DLResponse<PortalPermissionTypesPortalPermissionGroups_KEY_RESTResponseDto>> PortalPermissionTypesPortalPermissionGroups_KEY_RESTAsync(int portalPermissionTypesId, int portalPermissionGroupsId)
        {
            var returnValue = new DLResponse<PortalPermissionTypesPortalPermissionGroups_KEY_RESTResponseDto>();
            try
            {
                var queryableResult =
                    from portal_permission_groups in _readContext.PortalPermissionGroups
                    join portal_permission_types in _readContext.PortalPermissionTypes on portal_permission_groups.PortalPermissionTypeId equals portal_permission_types.Id
                    where portal_permission_types.Id.Equals(portalPermissionTypesId) && portal_permission_groups.Id.Equals(portalPermissionGroupsId)select new PortalPermissionTypesPortalPermissionGroups_KEY_RESTResponseDto()
                    {
                        Id = portal_permission_groups.Id,
                        PortalPermissionId = portal_permission_groups.PortalPermissionId,
                        PermissionGroupId = portal_permission_groups.PermissionGroupId,
                        PortalPermissionTypeId = portal_permission_groups.PortalPermissionTypeId
                    };
                var result = await queryableResult.FirstAsync();
                if (result == null)
                {
                    returnValue.Code = 404;
                    returnValue.Message = $"Not found in PortalPermissionTypes";
                }
                else
                {
                    returnValue.Value = result;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError("{repoName} Exception {error}", "PortalPermissionTypes PortalPermissionTypesPortalPermissionGroups_KEY_REST", ex.Message);
                returnValue.Code = 404;
                returnValue.Message = ex.ToString();
            }

            return returnValue;
        }
    }
}