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
    public class SmsSendersTotalItemCountQuery : IRequest<Response<int>>
    {
        public SmsSendersTotalItemCountQuery()
        {
        }

        public class SmsSendersTotalItemCountHandler : IRequestHandler<SmsSendersTotalItemCountQuery, Response<int>>
        {
            private readonly ILogger<SmsSendersTotalItemCountHandler> _logger;
            private readonly IMapper _mapper;
            private readonly ISmsSendersRepository _repository;
            public SmsSendersTotalItemCountHandler(IMapper mapper, ILogger<SmsSendersTotalItemCountHandler> logger, ISmsSendersRepository repository)
            {
                _mapper = mapper;
                _logger = logger;
                _repository = repository;
            }

            public async Task<Response<int>> Handle(SmsSendersTotalItemCountQuery request, CancellationToken cancellationToken)
            {
                var returnValue = _mapper.Map<Response<int>>(await _repository.CountAsync());
                return returnValue;
            }
        }
    }
}