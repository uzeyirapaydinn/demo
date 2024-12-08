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
    public class TopicWorkflowsGetItemQuery : IRequest<Response<TopicWorkflowsDto>>
    {
        public int Id { get; set; }

        public TopicWorkflowsGetItemQuery(int id)
        {
            this.Id = id;
        }

        public class TopicWorkflowsGetItemHandler : IRequestHandler<TopicWorkflowsGetItemQuery, Response<TopicWorkflowsDto>>
        {
            private readonly ILogger<TopicWorkflowsGetItemHandler> _logger;
            private readonly IMapper _mapper;
            private readonly ITopicWorkflowsRepository _repository;
            public TopicWorkflowsGetItemHandler(IMapper mapper, ILogger<TopicWorkflowsGetItemHandler> logger, ITopicWorkflowsRepository repository)
            {
                _mapper = mapper;
                _logger = logger;
                _repository = repository;
            }

            public async Task<Response<TopicWorkflowsDto>> Handle(TopicWorkflowsGetItemQuery request, CancellationToken cancellationToken)
            {
                var returnValue = _mapper.Map<Response<TopicWorkflowsDto>>(await _repository.GetByPkAsync(request.Id));
                return returnValue;
            }
        }
    }
}