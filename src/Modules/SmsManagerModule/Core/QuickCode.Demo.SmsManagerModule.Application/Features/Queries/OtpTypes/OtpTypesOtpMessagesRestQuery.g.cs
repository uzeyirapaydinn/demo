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
    public class OtpTypesOtpTypesOtpMessagesRestQuery : IRequest<Response<List<OtpTypesOtpMessagesRestResponseDto>>>
    {
        public int OtpTypesId { get; set; }

        public OtpTypesOtpTypesOtpMessagesRestQuery(int otpTypesId)
        {
            this.OtpTypesId = otpTypesId;
        }

        public class OtpTypesOtpTypesOtpMessagesRestHandler : IRequestHandler<OtpTypesOtpTypesOtpMessagesRestQuery, Response<List<OtpTypesOtpMessagesRestResponseDto>>>
        {
            private readonly ILogger<OtpTypesOtpTypesOtpMessagesRestHandler> _logger;
            private readonly IMapper _mapper;
            private readonly IOtpTypesRepository _repository;
            public OtpTypesOtpTypesOtpMessagesRestHandler(IMapper mapper, ILogger<OtpTypesOtpTypesOtpMessagesRestHandler> logger, IOtpTypesRepository repository)
            {
                _mapper = mapper;
                _logger = logger;
                _repository = repository;
            }

            public async Task<Response<List<OtpTypesOtpMessagesRestResponseDto>>> Handle(OtpTypesOtpTypesOtpMessagesRestQuery request, CancellationToken cancellationToken)
            {
                var returnValue = _mapper.Map<Response<List<OtpTypesOtpMessagesRestResponseDto>>>(await _repository.OtpTypesOtpMessagesRestAsync(request.OtpTypesId));
                return returnValue;
            }
        }
    }
}