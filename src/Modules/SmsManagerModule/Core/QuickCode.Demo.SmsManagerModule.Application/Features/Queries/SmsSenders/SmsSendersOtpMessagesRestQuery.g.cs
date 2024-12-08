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
    public class SmsSendersSmsSendersOtpMessagesRestQuery : IRequest<Response<List<SmsSendersOtpMessagesRestResponseDto>>>
    {
        public int SmsSendersId { get; set; }

        public SmsSendersSmsSendersOtpMessagesRestQuery(int smsSendersId)
        {
            this.SmsSendersId = smsSendersId;
        }

        public class SmsSendersSmsSendersOtpMessagesRestHandler : IRequestHandler<SmsSendersSmsSendersOtpMessagesRestQuery, Response<List<SmsSendersOtpMessagesRestResponseDto>>>
        {
            private readonly ILogger<SmsSendersSmsSendersOtpMessagesRestHandler> _logger;
            private readonly IMapper _mapper;
            private readonly ISmsSendersRepository _repository;
            public SmsSendersSmsSendersOtpMessagesRestHandler(IMapper mapper, ILogger<SmsSendersSmsSendersOtpMessagesRestHandler> logger, ISmsSendersRepository repository)
            {
                _mapper = mapper;
                _logger = logger;
                _repository = repository;
            }

            public async Task<Response<List<SmsSendersOtpMessagesRestResponseDto>>> Handle(SmsSendersSmsSendersOtpMessagesRestQuery request, CancellationToken cancellationToken)
            {
                var returnValue = _mapper.Map<Response<List<SmsSendersOtpMessagesRestResponseDto>>>(await _repository.SmsSendersOtpMessagesRestAsync(request.SmsSendersId));
                return returnValue;
            }
        }
    }
}