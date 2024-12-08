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
using QuickCode.Demo.SmsManagerModule.Application.Models;
using QuickCode.Demo.SmsManagerModule.Domain.Entities;
using QuickCode.Demo.SmsManagerModule.Application.Dtos;
using System.Threading.Tasks;

namespace QuickCode.Demo.SmsManagerModule.Application.Interfaces.Repositories
{
    public partial interface ISmsSendersRepository : IBaseRepository<SmsSenders>
    {
        Task<DLResponse<SmsSenders>> GetByPkAsync(int id);
        Task<DLResponse<List<SmsSendersInfoMessagesRestResponseDto>>> SmsSendersInfoMessagesRestAsync(int smsSendersId);
        Task<DLResponse<SmsSendersInfoMessagesKeyRestResponseDto>> SmsSendersInfoMessagesKeyRestAsync(int smsSendersId, int infoMessagesId);
        Task<DLResponse<List<SmsSendersOtpMessagesRestResponseDto>>> SmsSendersOtpMessagesRestAsync(int smsSendersId);
        Task<DLResponse<SmsSendersOtpMessagesKeyRestResponseDto>> SmsSendersOtpMessagesKeyRestAsync(int smsSendersId, int otpMessagesId);
        Task<DLResponse<List<SmsSendersCampaignMessagesRestResponseDto>>> SmsSendersCampaignMessagesRestAsync(int smsSendersId);
        Task<DLResponse<SmsSendersCampaignMessagesKeyRestResponseDto>> SmsSendersCampaignMessagesKeyRestAsync(int smsSendersId, int campaignMessagesId);
    }
}