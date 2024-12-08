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
    public class CampaignTypesListQuery : IRequest<Response<List<CampaignTypesDto>>>
    {
        public int? PageNumber { get; set; }
        public int? PageSize { get; set; }

        public CampaignTypesListQuery(int? pageNumber, int? pageSize)
        {
            this.PageNumber = pageNumber;
            this.PageSize = pageSize;
        }

        public class CampaignTypesListHandler : IRequestHandler<CampaignTypesListQuery, Response<List<CampaignTypesDto>>>
        {
            private readonly ILogger<CampaignTypesListHandler> _logger;
            private readonly IMapper _mapper;
            private readonly ICampaignTypesRepository _repository;
            public CampaignTypesListHandler(IMapper mapper, ILogger<CampaignTypesListHandler> logger, ICampaignTypesRepository repository)
            {
                _mapper = mapper;
                _logger = logger;
                _repository = repository;
            }

            public async Task<Response<List<CampaignTypesDto>>> Handle(CampaignTypesListQuery request, CancellationToken cancellationToken)
            {
                var returnValue = _mapper.Map<Response<List<CampaignTypesDto>>>(await _repository.ListAsync(request.PageNumber, request.PageSize));
                return returnValue;
            }
        }
    }
}