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
    public class CampaignMessagesDeleteCommand : IRequest<Response<bool>>
    {
        public CampaignMessagesDto request { get; set; }

        public CampaignMessagesDeleteCommand(CampaignMessagesDto request)
        {
            this.request = request;
        }

        public class CampaignMessagesDeleteHandler : IRequestHandler<CampaignMessagesDeleteCommand, Response<bool>>
        {
            private readonly ILogger<CampaignMessagesDeleteHandler> _logger;
            private readonly IMapper _mapper;
            private readonly ICampaignMessagesRepository _repository;
            public CampaignMessagesDeleteHandler(IMapper mapper, ILogger<CampaignMessagesDeleteHandler> logger, ICampaignMessagesRepository repository)
            {
                _mapper = mapper;
                _logger = logger;
                _repository = repository;
            }

            public async Task<Response<bool>> Handle(CampaignMessagesDeleteCommand request, CancellationToken cancellationToken)
            {
                var model = _mapper.Map<CampaignMessages>(request.request);
                var returnValue = _mapper.Map<Response<bool>>(await _repository.DeleteAsync(model));
                return returnValue;
            }
        }
    }
}