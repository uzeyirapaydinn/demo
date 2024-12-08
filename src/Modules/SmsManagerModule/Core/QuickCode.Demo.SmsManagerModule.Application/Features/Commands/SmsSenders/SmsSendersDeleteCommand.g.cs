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
    public class SmsSendersDeleteCommand : IRequest<Response<bool>>
    {
        public SmsSendersDto request { get; set; }

        public SmsSendersDeleteCommand(SmsSendersDto request)
        {
            this.request = request;
        }

        public class SmsSendersDeleteHandler : IRequestHandler<SmsSendersDeleteCommand, Response<bool>>
        {
            private readonly ILogger<SmsSendersDeleteHandler> _logger;
            private readonly IMapper _mapper;
            private readonly ISmsSendersRepository _repository;
            public SmsSendersDeleteHandler(IMapper mapper, ILogger<SmsSendersDeleteHandler> logger, ISmsSendersRepository repository)
            {
                _mapper = mapper;
                _logger = logger;
                _repository = repository;
            }

            public async Task<Response<bool>> Handle(SmsSendersDeleteCommand request, CancellationToken cancellationToken)
            {
                var model = _mapper.Map<SmsSenders>(request.request);
                var returnValue = _mapper.Map<Response<bool>>(await _repository.DeleteAsync(model));
                return returnValue;
            }
        }
    }
}