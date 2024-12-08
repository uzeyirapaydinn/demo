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
    public class CampaignTypesTotalItemCountQuery : IRequest<Response<int>>
    {
        public CampaignTypesTotalItemCountQuery()
        {
        }

        public class CampaignTypesTotalItemCountHandler : IRequestHandler<CampaignTypesTotalItemCountQuery, Response<int>>
        {
            private readonly ILogger<CampaignTypesTotalItemCountHandler> _logger;
            private readonly IMapper _mapper;
            private readonly ICampaignTypesRepository _repository;
            public CampaignTypesTotalItemCountHandler(IMapper mapper, ILogger<CampaignTypesTotalItemCountHandler> logger, ICampaignTypesRepository repository)
            {
                _mapper = mapper;
                _logger = logger;
                _repository = repository;
            }

            public async Task<Response<int>> Handle(CampaignTypesTotalItemCountQuery request, CancellationToken cancellationToken)
            {
                var returnValue = _mapper.Map<Response<int>>(await _repository.CountAsync());
                return returnValue;
            }
        }
    }
}