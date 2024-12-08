using AutoMapper;
using System.Linq;
using MediatR;
using Microsoft.Extensions.Logging;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using QuickCode.Demo.EmailManagerModule.Application.Models;
using QuickCode.Demo.EmailManagerModule.Domain.Entities;
using QuickCode.Demo.EmailManagerModule.Application.Interfaces.Repositories;
using QuickCode.Demo.EmailManagerModule.Application.Dtos;

namespace QuickCode.Demo.EmailManagerModule.Application.Features
{
    public class EmailSendersEmailSendersCampaignMessagesKeyRestQuery : IRequest<Response<EmailSendersCampaignMessagesKeyRestResponseDto>>
    {
        public int EmailSendersId { get; set; }
        public int CampaignMessagesId { get; set; }

        public EmailSendersEmailSendersCampaignMessagesKeyRestQuery(int emailSendersId, int campaignMessagesId)
        {
            this.EmailSendersId = emailSendersId;
            this.CampaignMessagesId = campaignMessagesId;
        }

        public class EmailSendersEmailSendersCampaignMessagesKeyRestHandler : IRequestHandler<EmailSendersEmailSendersCampaignMessagesKeyRestQuery, Response<EmailSendersCampaignMessagesKeyRestResponseDto>>
        {
            private readonly ILogger<EmailSendersEmailSendersCampaignMessagesKeyRestHandler> _logger;
            private readonly IMapper _mapper;
            private readonly IEmailSendersRepository _repository;
            public EmailSendersEmailSendersCampaignMessagesKeyRestHandler(IMapper mapper, ILogger<EmailSendersEmailSendersCampaignMessagesKeyRestHandler> logger, IEmailSendersRepository repository)
            {
                _mapper = mapper;
                _logger = logger;
                _repository = repository;
            }

            public async Task<Response<EmailSendersCampaignMessagesKeyRestResponseDto>> Handle(EmailSendersEmailSendersCampaignMessagesKeyRestQuery request, CancellationToken cancellationToken)
            {
                var returnValue = _mapper.Map<Response<EmailSendersCampaignMessagesKeyRestResponseDto>>(await _repository.EmailSendersCampaignMessagesKeyRestAsync(request.EmailSendersId, request.CampaignMessagesId));
                return returnValue;
            }
        }
    }
}