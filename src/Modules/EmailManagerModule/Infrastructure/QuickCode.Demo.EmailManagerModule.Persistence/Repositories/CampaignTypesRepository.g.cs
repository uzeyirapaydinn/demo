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
using QuickCode.Demo.EmailManagerModule.Application.Models;
using QuickCode.Demo.EmailManagerModule.Domain.Entities;
using QuickCode.Demo.EmailManagerModule.Application.Interfaces.Repositories;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using QuickCode.Demo.EmailManagerModule.Persistence.Contexts;
using QuickCode.Demo.EmailManagerModule.Application.Dtos;

namespace QuickCode.Demo.EmailManagerModule.Persistence.Repositories
{
    public partial class CampaignTypesRepository : ICampaignTypesRepository
    {
        private readonly WriteDbContext _writeContext;
        private readonly ReadDbContext _readContext;
        private readonly ILogger<CampaignTypesRepository> _logger;
        public CampaignTypesRepository(ILogger<CampaignTypesRepository> logger, WriteDbContext writeContext, ReadDbContext readContext)
        {
            _writeContext = writeContext;
            _readContext = readContext;
            _logger = logger;
        }

        public async Task<DLResponse<CampaignTypes>> InsertAsync(CampaignTypes value)
        {
            var returnValue = new DLResponse<CampaignTypes>(value, "Not Defined");
            try
            {
                await _writeContext.CampaignTypes.AddAsync(value);
                await _writeContext.SaveChangesAsync();
                returnValue.Value = value;
            }
            catch (SqlException ex)
            {
                _logger.LogError("{repoName} SqlException {error}", "CampaignTypes Insert", ex.Message);
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
                _logger.LogError("{repoName} Exception {error}", "CampaignTypes Insert", ex.Message);
                returnValue.Code = 500;
                returnValue.Value = value;
                returnValue.Message = ex.ToString();
            }

            return returnValue;
        }

        public async Task<DLResponse<bool>> UpdateAsync(CampaignTypes value)
        {
            var returnValue = new DLResponse<bool>(false, "Success");
            try
            {
                _writeContext.Set<CampaignTypes>().Update(value);
                await _writeContext.SaveChangesAsync();
                returnValue.Value = true;
            }
            catch (SqlException ex)
            {
                _logger.LogError("{repoName} SqlException {error}", "CampaignTypes Update", ex.Message);
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
                _logger.LogError("{repoName} Exception {error}", "CampaignTypes", ex.Message);
                returnValue.Code = 404;
                returnValue.Message = ex.ToString();
            }

            return returnValue;
        }

        public async Task<DLResponse<bool>> DeleteAsync(CampaignTypes value)
        {
            var returnValue = new DLResponse<bool>(false, "Success");
            try
            {
                _writeContext.CampaignTypes.Remove(value);
                await _writeContext.SaveChangesAsync();
                returnValue.Value = true;
            }
            catch (SqlException ex)
            {
                _logger.LogError("{repoName} SqlException {error}", "CampaignTypes Delete", ex.Message);
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
                _logger.LogError("{repoName} Exception {error}", "CampaignTypes Delete", ex.Message);
                returnValue.Code = 404;
                returnValue.Message = ex.ToString();
            }

            return returnValue;
        }

        public async Task<DLResponse<CampaignTypes>> GetByPkAsync(int id)
        {
            var returnValue = new DLResponse<CampaignTypes>();
            try
            {
                var result =
                    from campaign_types in _readContext.CampaignTypes
                    where campaign_types.Id.Equals(id)select campaign_types;
                returnValue.Value = await result.FirstAsync();
                if (returnValue.Value == null)
                {
                    returnValue.Code = 404;
                    returnValue.Message = $"Not found in CampaignTypes";
                    return returnValue;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError("{repoName} Exception {error}", "CampaignTypes GetByPk", ex.Message);
                returnValue.Code = 404;
                returnValue.Message = ex.ToString();
            }

            return returnValue;
        }

        public async Task<DLResponse<List<CampaignTypes>>> ListAsync(int? pageNumber = null, int? pageSize = null)
        {
            var returnValue = new DLResponse<List<CampaignTypes>>();
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
                        returnValue.Value = await _readContext.CampaignTypes.Skip(skip.Value).Take(take.Value).ToListAsync();
                    }
                    else
                    {
                        returnValue.Value = await _readContext.CampaignTypes.ToListAsync();
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
                returnValue.Value = await _readContext.CampaignTypes.CountAsync();
            }
            catch (Exception ex)
            {
                returnValue.Code = 404;
                returnValue.Message = ex.ToString();
            }

            return returnValue;
        }

        public async Task<DLResponse<List<CampaignTypesCampaignMessagesRestResponseDto>>> CampaignTypesCampaignMessagesRestAsync(int campaignTypesId)
        {
            var returnValue = new DLResponse<List<CampaignTypesCampaignMessagesRestResponseDto>>();
            try
            {
                var queryableResult =
                    from campaign_messages in _readContext.CampaignMessages
                    join campaign_types in _readContext.CampaignTypes on campaign_messages.CampaignTypeId equals campaign_types.Id
                    where campaign_types.Id.Equals(campaignTypesId)select new CampaignTypesCampaignMessagesRestResponseDto()
                    {
                        Id = campaign_messages.Id,
                        EmailSenderId = campaign_messages.EmailSenderId,
                        CampaignTypeId = campaign_messages.CampaignTypeId,
                        GsmNumber = campaign_messages.GsmNumber,
                        Message = campaign_messages.Message,
                        MessageDate = campaign_messages.MessageDate,
                        MessageSid = campaign_messages.MessageSid,
                        DailyCounter = campaign_messages.DailyCounter
                    };
                var result = await queryableResult.ToListAsync();
                returnValue.Value = result;
            }
            catch (Exception ex)
            {
                _logger.LogError("{repoName} Exception {error}", "CampaignTypes CampaignTypesCampaignMessagesRest", ex.Message);
                returnValue.Code = 404;
                returnValue.Message = ex.ToString();
            }

            return returnValue;
        }

        public async Task<DLResponse<CampaignTypesCampaignMessagesKeyRestResponseDto>> CampaignTypesCampaignMessagesKeyRestAsync(int campaignTypesId, int campaignMessagesId)
        {
            var returnValue = new DLResponse<CampaignTypesCampaignMessagesKeyRestResponseDto>();
            try
            {
                var queryableResult =
                    from campaign_messages in _readContext.CampaignMessages
                    join campaign_types in _readContext.CampaignTypes on campaign_messages.CampaignTypeId equals campaign_types.Id
                    where campaign_types.Id.Equals(campaignTypesId) && campaign_messages.Id.Equals(campaignMessagesId)select new CampaignTypesCampaignMessagesKeyRestResponseDto()
                    {
                        Id = campaign_messages.Id,
                        EmailSenderId = campaign_messages.EmailSenderId,
                        CampaignTypeId = campaign_messages.CampaignTypeId,
                        GsmNumber = campaign_messages.GsmNumber,
                        Message = campaign_messages.Message,
                        MessageDate = campaign_messages.MessageDate,
                        MessageSid = campaign_messages.MessageSid,
                        DailyCounter = campaign_messages.DailyCounter
                    };
                var result = await queryableResult.FirstAsync();
                if (result == null)
                {
                    returnValue.Code = 404;
                    returnValue.Message = $"Not found in CampaignTypes";
                }
                else
                {
                    returnValue.Value = result;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError("{repoName} Exception {error}", "CampaignTypes CampaignTypesCampaignMessagesKeyRest", ex.Message);
                returnValue.Code = 404;
                returnValue.Message = ex.ToString();
            }

            return returnValue;
        }
    }
}