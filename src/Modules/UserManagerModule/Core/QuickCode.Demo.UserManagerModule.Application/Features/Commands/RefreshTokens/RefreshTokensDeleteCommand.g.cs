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
    public class RefreshTokensDeleteCommand : IRequest<Response<bool>>
    {
        public RefreshTokensDto request { get; set; }

        public RefreshTokensDeleteCommand(RefreshTokensDto request)
        {
            this.request = request;
        }

        public class RefreshTokensDeleteHandler : IRequestHandler<RefreshTokensDeleteCommand, Response<bool>>
        {
            private readonly ILogger<RefreshTokensDeleteHandler> _logger;
            private readonly IMapper _mapper;
            private readonly IRefreshTokensRepository _repository;
            public RefreshTokensDeleteHandler(IMapper mapper, ILogger<RefreshTokensDeleteHandler> logger, IRefreshTokensRepository repository)
            {
                _mapper = mapper;
                _logger = logger;
                _repository = repository;
            }

            public async Task<Response<bool>> Handle(RefreshTokensDeleteCommand request, CancellationToken cancellationToken)
            {
                var model = _mapper.Map<RefreshTokens>(request.request);
                var returnValue = _mapper.Map<Response<bool>>(await _repository.DeleteAsync(model));
                return returnValue;
            }
        }
    }
}