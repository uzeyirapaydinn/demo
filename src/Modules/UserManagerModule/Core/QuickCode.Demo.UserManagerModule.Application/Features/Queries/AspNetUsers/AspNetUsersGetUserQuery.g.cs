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
    public class AspNetUsersAspNetUsersGetUserQuery : IRequest<Response<AspNetUsersGetUserResponseDto>>
    {
        public string? AspNetUsersEmail { get; set; }

        public AspNetUsersAspNetUsersGetUserQuery(string? aspNetUsersEmail)
        {
            this.AspNetUsersEmail = aspNetUsersEmail;
        }

        public class AspNetUsersAspNetUsersGetUserHandler : IRequestHandler<AspNetUsersAspNetUsersGetUserQuery, Response<AspNetUsersGetUserResponseDto>>
        {
            private readonly ILogger<AspNetUsersAspNetUsersGetUserHandler> _logger;
            private readonly IMapper _mapper;
            private readonly IAspNetUsersRepository _repository;
            public AspNetUsersAspNetUsersGetUserHandler(IMapper mapper, ILogger<AspNetUsersAspNetUsersGetUserHandler> logger, IAspNetUsersRepository repository)
            {
                _mapper = mapper;
                _logger = logger;
                _repository = repository;
            }

            public async Task<Response<AspNetUsersGetUserResponseDto>> Handle(AspNetUsersAspNetUsersGetUserQuery request, CancellationToken cancellationToken)
            {
                var returnValue = _mapper.Map<Response<AspNetUsersGetUserResponseDto>>(await _repository.AspNetUsersGetUserAsync(request.AspNetUsersEmail));
                return returnValue;
            }
        }
    }
}