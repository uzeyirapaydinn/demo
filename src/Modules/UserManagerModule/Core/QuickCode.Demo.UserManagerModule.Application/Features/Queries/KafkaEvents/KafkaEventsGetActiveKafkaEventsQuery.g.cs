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
    public class KafkaEventsKafkaEventsGetActiveKafkaEventsQuery : IRequest<Response<List<KafkaEventsGetActiveKafkaEventsResponseDto>>>
    {
        public KafkaEventsKafkaEventsGetActiveKafkaEventsQuery()
        {
        }

        public class KafkaEventsKafkaEventsGetActiveKafkaEventsHandler : IRequestHandler<KafkaEventsKafkaEventsGetActiveKafkaEventsQuery, Response<List<KafkaEventsGetActiveKafkaEventsResponseDto>>>
        {
            private readonly ILogger<KafkaEventsKafkaEventsGetActiveKafkaEventsHandler> _logger;
            private readonly IMapper _mapper;
            private readonly IKafkaEventsRepository _repository;
            public KafkaEventsKafkaEventsGetActiveKafkaEventsHandler(IMapper mapper, ILogger<KafkaEventsKafkaEventsGetActiveKafkaEventsHandler> logger, IKafkaEventsRepository repository)
            {
                _mapper = mapper;
                _logger = logger;
                _repository = repository;
            }

            public async Task<Response<List<KafkaEventsGetActiveKafkaEventsResponseDto>>> Handle(KafkaEventsKafkaEventsGetActiveKafkaEventsQuery request, CancellationToken cancellationToken)
            {
                var returnValue = _mapper.Map<Response<List<KafkaEventsGetActiveKafkaEventsResponseDto>>>(await _repository.KafkaEventsGetActiveKafkaEventsAsync());
                return returnValue;
            }
        }
    }
}