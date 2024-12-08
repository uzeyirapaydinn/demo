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
    public partial class AspNetUserClaimsRepository : IAspNetUserClaimsRepository
    {
        private readonly WriteDbContext _writeContext;
        private readonly ReadDbContext _readContext;
        private readonly ILogger<AspNetUserClaimsRepository> _logger;
        public AspNetUserClaimsRepository(ILogger<AspNetUserClaimsRepository> logger, WriteDbContext writeContext, ReadDbContext readContext)
        {
            _writeContext = writeContext;
            _readContext = readContext;
            _logger = logger;
        }

        public async Task<DLResponse<AspNetUserClaims>> InsertAsync(AspNetUserClaims value)
        {
            var returnValue = new DLResponse<AspNetUserClaims>(value, "Not Defined");
            try
            {
                await _writeContext.AspNetUserClaims.AddAsync(value);
                await _writeContext.SaveChangesAsync();
                returnValue.Value = value;
            }
            catch (SqlException ex)
            {
                _logger.LogError("{repoName} SqlException {error}", "AspNetUserClaims Insert", ex.Message);
                if (ex.Number.Equals(2627))
                {
                    returnValue.Code = 999;
                    returnValue.Value = value;
                }
                else
                {
                    returnValue.Code = 998;
                    returnValue.Value = value;
                }

                returnValue.Message = ex.ToString();
            }
            catch (Exception ex)
            {
                _logger.LogError("{repoName} Exception {error}", "AspNetUserClaims Insert", ex.Message);
                returnValue.Code = 500;
                returnValue.Value = value;
                returnValue.Message = ex.ToString();
            }

            return returnValue;
        }

        public async Task<DLResponse<bool>> UpdateAsync(AspNetUserClaims value)
        {
            var returnValue = new DLResponse<bool>(false, "Success");
            try
            {
                _writeContext.Set<AspNetUserClaims>().Update(value);
                await _writeContext.SaveChangesAsync();
                returnValue.Value = true;
            }
            catch (SqlException ex)
            {
                _logger.LogError("{repoName} SqlException {error}", "AspNetUserClaims Update", ex.Message);
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
                _logger.LogError("{repoName} Exception {error}", "AspNetUserClaims", ex.Message);
                returnValue.Code = 404;
                returnValue.Message = ex.ToString();
            }

            return returnValue;
        }

        public async Task<DLResponse<bool>> DeleteAsync(AspNetUserClaims value)
        {
            var returnValue = new DLResponse<bool>(false, "Success");
            try
            {
                _writeContext.AspNetUserClaims.Remove(value);
                await _writeContext.SaveChangesAsync();
                returnValue.Value = true;
            }
            catch (SqlException ex)
            {
                _logger.LogError("{repoName} SqlException {error}", "AspNetUserClaims Delete", ex.Message);
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
                _logger.LogError("{repoName} Exception {error}", "AspNetUserClaims Delete", ex.Message);
                returnValue.Code = 404;
                returnValue.Message = ex.ToString();
            }

            return returnValue;
        }

        public async Task<DLResponse<AspNetUserClaims>> GetByPkAsync(int id)
        {
            var returnValue = new DLResponse<AspNetUserClaims>();
            try
            {
                var result =
                    from asp_net_user_claims in _readContext.AspNetUserClaims
                    where asp_net_user_claims.Id.Equals(id)select asp_net_user_claims;
                returnValue.Value = await result.FirstAsync();
                if (returnValue.Value == null)
                {
                    returnValue.Code = 404;
                    returnValue.Message = $"Not found in AspNetUserClaims";
                    return returnValue;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError("{repoName} Exception {error}", "AspNetUserClaims GetByPk", ex.Message);
                returnValue.Code = 404;
                returnValue.Message = ex.ToString();
            }

            return returnValue;
        }

        public async Task<DLResponse<List<AspNetUserClaims>>> ListAsync(int? pageNumber = null, int? pageSize = null)
        {
            var returnValue = new DLResponse<List<AspNetUserClaims>>();
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
                        returnValue.Value = await _readContext.AspNetUserClaims.Skip(skip.Value).Take(take.Value).ToListAsync();
                    }
                    else
                    {
                        returnValue.Value = await _readContext.AspNetUserClaims.ToListAsync();
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
                returnValue.Value = await _readContext.AspNetUserClaims.CountAsync();
            }
            catch (Exception ex)
            {
                returnValue.Code = 404;
                returnValue.Message = ex.ToString();
            }

            return returnValue;
        }
    }
}