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
    public class TopicWorkflowsDeleteCommand : IRequest<Response<bool>>
    {
        public TopicWorkflowsDto request { get; set; }

        public TopicWorkflowsDeleteCommand(TopicWorkflowsDto request)
        {
            this.request = request;
        }

        public class TopicWorkflowsDeleteHandler : IRequestHandler<TopicWorkflowsDeleteCommand, Response<bool>>
        {
            private readonly ILogger<TopicWorkflowsDeleteHandler> _logger;
            private readonly IMapper _mapper;
            private readonly ITopicWorkflowsRepository _repository;
            public TopicWorkflowsDeleteHandler(IMapper mapper, ILogger<TopicWorkflowsDeleteHandler> logger, ITopicWorkflowsRepository repository)
            {
                _mapper = mapper;
                _logger = logger;
                _repository = repository;
            }

            public async Task<Response<bool>> Handle(TopicWorkflowsDeleteCommand request, CancellationToken cancellationToken)
            {
                var model = _mapper.Map<TopicWorkflows>(request.request);
                var returnValue = _mapper.Map<Response<bool>>(await _repository.DeleteAsync(model));
                return returnValue;
            }
        }
    }
}