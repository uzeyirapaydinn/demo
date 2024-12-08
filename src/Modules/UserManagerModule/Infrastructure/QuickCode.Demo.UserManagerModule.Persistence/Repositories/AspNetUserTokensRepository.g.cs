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
    public partial class AspNetUserTokensRepository : IAspNetUserTokensRepository
    {
        private readonly WriteDbContext _writeContext;
        private readonly ReadDbContext _readContext;
        private readonly ILogger<AspNetUserTokensRepository> _logger;
        public AspNetUserTokensRepository(ILogger<AspNetUserTokensRepository> logger, WriteDbContext writeContext, ReadDbContext readContext)
        {
            _writeContext = writeContext;
            _readContext = readContext;
            _logger = logger;
        }

        public async Task<DLResponse<AspNetUserTokens>> InsertAsync(AspNetUserTokens value)
        {
            var returnValue = new DLResponse<AspNetUserTokens>(value, "Success");
            try
            {
                await _writeContext.AspNetUserTokens.AddAsync(value);
                await _writeContext.SaveChangesAsync();
                returnValue = new DLResponse<AspNetUserTokens>(value, "Success");
            }
            catch (SqlException ex)
            {
                _logger.LogError("{repoName} SqlException {error}", "AspNetUserTokens Insert", ex.Message);
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
                _logger.LogError("{repoName} Exception {error}", "AspNetUserTokens Insert", ex.Message);
                returnValue.Code = 404;
                returnValue.Message = ex.ToString();
            }

            return returnValue;
        }

        public async Task<DLResponse<bool>> UpdateAsync(AspNetUserTokens value)
        {
            var returnValue = new DLResponse<bool>(false, "Success");
            try
            {
                _writeContext.Set<AspNetUserTokens>().Update(value);
                await _writeContext.SaveChangesAsync();
                returnValue.Value = true;
            }
            catch (SqlException ex)
            {
                _logger.LogError("{repoName} SqlException {error}", "AspNetUserTokens Update", ex.Message);
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
                _logger.LogError("{repoName} Exception {error}", "AspNetUserTokens", ex.Message);
                returnValue.Code = 404;
                returnValue.Message = ex.ToString();
            }

            return returnValue;
        }

        public async Task<DLResponse<bool>> DeleteAsync(AspNetUserTokens value)
        {
            var returnValue = new DLResponse<bool>(false, "Success");
            try
            {
                _writeContext.AspNetUserTokens.Remove(value);
                await _writeContext.SaveChangesAsync();
                returnValue.Value = true;
            }
            catch (SqlException ex)
            {
                _logger.LogError("{repoName} SqlException {error}", "AspNetUserTokens Delete", ex.Message);
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
                _logger.LogError("{repoName} Exception {error}", "AspNetUserTokens Delete", ex.Message);
                returnValue.Code = 404;
                returnValue.Message = ex.ToString();
            }

            return returnValue;
        }

        public async Task<DLResponse<AspNetUserTokens>> GetByPkAsync(string userId)
        {
            var returnValue = new DLResponse<AspNetUserTokens>();
            try
            {
                var result =
                    from asp_net_user_tokens in _readContext.AspNetUserTokens
                    where asp_net_user_tokens.UserId.Equals(userId)select asp_net_user_tokens;
                returnValue.Value = await result.FirstAsync();
                if (returnValue.Value == null)
                {
                    returnValue.Code = 404;
                    returnValue.Message = $"Not found in AspNetUserTokens";
                    return returnValue;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError("{repoName} Exception {error}", "AspNetUserTokens GetByPk", ex.Message);
                returnValue.Code = 404;
                returnValue.Message = ex.ToString();
            }

            return returnValue;
        }

        public async Task<DLResponse<List<AspNetUserTokens>>> ListAsync(int? pageNumber = null, int? pageSize = null)
        {
            var returnValue = new DLResponse<List<AspNetUserTokens>>();
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
                        returnValue.Value = await _readContext.AspNetUserTokens.Skip(skip.Value).Take(take.Value).ToListAsync();
                    }
                    else
                    {
                        returnValue.Value = await _readContext.AspNetUserTokens.ToListAsync();
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
                returnValue.Value = await _readContext.AspNetUserTokens.CountAsync();
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