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
    public class CampaignTypesCampaignTypesCampaignMessagesKeyRestQuery : IRequest<Response<CampaignTypesCampaignMessagesKeyRestResponseDto>>
    {
        public int CampaignTypesId { get; set; }
        public int CampaignMessagesId { get; set; }

        public CampaignTypesCampaignTypesCampaignMessagesKeyRestQuery(int campaignTypesId, int campaignMessagesId)
        {
            this.CampaignTypesId = campaignTypesId;
            this.CampaignMessagesId = campaignMessagesId;
        }

        public class CampaignTypesCampaignTypesCampaignMessagesKeyRestHandler : IRequestHandler<CampaignTypesCampaignTypesCampaignMessagesKeyRestQuery, Response<CampaignTypesCampaignMessagesKeyRestResponseDto>>
        {
            private readonly ILogger<CampaignTypesCampaignTypesCampaignMessagesKeyRestHandler> _logger;
            private readonly IMapper _mapper;
            private readonly ICampaignTypesRepository _repository;
            public CampaignTypesCampaignTypesCampaignMessagesKeyRestHandler(IMapper mapper, ILogger<CampaignTypesCampaignTypesCampaignMessagesKeyRestHandler> logger, ICampaignTypesRepository repository)
            {
                _mapper = mapper;
                _logger = logger;
                _repository = repository;
            }

            public async Task<Response<CampaignTypesCampaignMessagesKeyRestResponseDto>> Handle(CampaignTypesCampaignTypesCampaignMessagesKeyRestQuery request, CancellationToken cancellationToken)
            {
                var returnValue = _mapper.Map<Response<CampaignTypesCampaignMessagesKeyRestResponseDto>>(await _repository.CampaignTypesCampaignMessagesKeyRestAsync(request.CampaignTypesId, request.CampaignMessagesId));
                return returnValue;
            }
        }
    }
}