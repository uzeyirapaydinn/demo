using AutoMapper;
using System.Linq;
using MediatR;
using Microsoft.Extensions.Logging;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using QuickCode.Demo.SmsManagerModule.Application.Models;
using QuickCode.Demo.SmsManagerModule.Domain.Entities;
using QuickCode.Demo.SmsManagerModule.Application.Interfaces.Repositories;
using QuickCode.Demo.SmsManagerModule.Application.Dtos;

namespace QuickCode.Demo.SmsManagerModule.Application.Features
{
    public class InfoTypesTotalItemCountQuery : IRequest<Response<int>>
    {
        public InfoTypesTotalItemCountQuery()
        {
        }

        public class InfoTypesTotalItemCountHandler : IRequestHandler<InfoTypesTotalItemCountQuery, Response<int>>
        {
            private readonly ILogger<InfoTypesTotalItemCountHandler> _logger;
            private readonly IMapper _mapper;
            private readonly IInfoTypesRepository _repository;
            public InfoTypesTotalItemCountHandler(IMapper mapper, ILogger<InfoTypesTotalItemCountHandler> logger, IInfoTypesRepository repository)
            {
                _mapper = mapper;
                _logger = logger;
                _repository = repository;
            }

            public async Task<Response<int>> Handle(InfoTypesTotalItemCountQuery request, CancellationToken cancellationToken)
            {
                var returnValue = _mapper.Map<Response<int>>(await _repository.CountAsync());
                return returnValue;
            }
        }
    }
}