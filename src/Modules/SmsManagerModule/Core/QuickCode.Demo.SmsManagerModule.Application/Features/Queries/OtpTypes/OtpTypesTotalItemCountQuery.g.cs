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
    public class OtpTypesTotalItemCountQuery : IRequest<Response<int>>
    {
        public OtpTypesTotalItemCountQuery()
        {
        }

        public class OtpTypesTotalItemCountHandler : IRequestHandler<OtpTypesTotalItemCountQuery, Response<int>>
        {
            private readonly ILogger<OtpTypesTotalItemCountHandler> _logger;
            private readonly IMapper _mapper;
            private readonly IOtpTypesRepository _repository;
            public OtpTypesTotalItemCountHandler(IMapper mapper, ILogger<OtpTypesTotalItemCountHandler> logger, IOtpTypesRepository repository)
            {
                _mapper = mapper;
                _logger = logger;
                _repository = repository;
            }

            public async Task<Response<int>> Handle(OtpTypesTotalItemCountQuery request, CancellationToken cancellationToken)
            {
                var returnValue = _mapper.Map<Response<int>>(await _repository.CountAsync());
                return returnValue;
            }
        }
    }
}