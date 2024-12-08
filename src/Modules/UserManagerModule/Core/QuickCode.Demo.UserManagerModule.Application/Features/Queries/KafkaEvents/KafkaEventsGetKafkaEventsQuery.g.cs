using AutoMapper;
using System.Linq;
using MediatR;
using Microsoft.Extensions.Logging;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using QuickCode.Demo.UserManagerModule.Application.Models;
using QuickCode.Demo.UserManagerModule.Domain.Entities;
using QuickCode.Demo.UserManagerModule.Application.Interfaces.Repositories;
using QuickCode.Demo.UserManagerModule.Application.Dtos;

namespace QuickCode.Demo.UserManagerModule.Application.Features
{
    public class KafkaEventsKafkaEventsGetKafkaEventsQuery : IRequest<Response<List<KafkaEventsGetKafkaEventsResponseDto>>>
    {
        public KafkaEventsKafkaEventsGetKafkaEventsQuery()
        {
        }

        public class KafkaEventsKafkaEventsGetKafkaEventsHandler : IRequestHandler<KafkaEventsKafkaEventsGetKafkaEventsQuery, Response<List<KafkaEventsGetKafkaEventsResponseDto>>>
        {
            private readonly ILogger<KafkaEventsKafkaEventsGetKafkaEventsHandler> _logger;
            private readonly IMapper _mapper;
            private readonly IKafkaEventsRepository _repository;
            public KafkaEventsKafkaEventsGetKafkaEventsHandler(IMapper mapper, ILogger<KafkaEventsKafkaEventsGetKafkaEventsHandler> logger, IKafkaEventsRepository repository)
            {
                _mapper = mapper;
                _logger = logger;
                _repository = repository;
            }

            public async Task<Response<List<KafkaEventsGetKafkaEventsResponseDto>>> Handle(KafkaEventsKafkaEventsGetKafkaEventsQuery request, CancellationToken cancellationToken)
            {
                var returnValue = _mapper.Map<Response<List<KafkaEventsGetKafkaEventsResponseDto>>>(await _repository.KafkaEventsGetKafkaEventsAsync());
                return returnValue;
            }
        }
    }
}