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
    public class EmailSendersEmailSendersInfoMessagesRestQuery : IRequest<Response<List<EmailSendersInfoMessagesRestResponseDto>>>
    {
        public int EmailSendersId { get; set; }

        public EmailSendersEmailSendersInfoMessagesRestQuery(int emailSendersId)
        {
            this.EmailSendersId = emailSendersId;
        }

        public class EmailSendersEmailSendersInfoMessagesRestHandler : IRequestHandler<EmailSendersEmailSendersInfoMessagesRestQuery, Response<List<EmailSendersInfoMessagesRestResponseDto>>>
        {
            private readonly ILogger<EmailSendersEmailSendersInfoMessagesRestHandler> _logger;
            private readonly IMapper _mapper;
            private readonly IEmailSendersRepository _repository;
            public EmailSendersEmailSendersInfoMessagesRestHandler(IMapper mapper, ILogger<EmailSendersEmailSendersInfoMessagesRestHandler> logger, IEmailSendersRepository repository)
            {
                _mapper = mapper;
                _logger = logger;
                _repository = repository;
            }

            public async Task<Response<List<EmailSendersInfoMessagesRestResponseDto>>> Handle(EmailSendersEmailSendersInfoMessagesRestQuery request, CancellationToken cancellationToken)
            {
                var returnValue = _mapper.Map<Response<List<EmailSendersInfoMessagesRestResponseDto>>>(await _repository.EmailSendersInfoMessagesRestAsync(request.EmailSendersId));
                return returnValue;
            }
        }
    }
}