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
    public class ApiPermissionGroupsApiPermissionGroupsGetApiPermissionGroupsQuery : IRequest<Response<List<ApiPermissionGroupsGetApiPermissionGroupsResponseDto>>>
    {
        public int ApiPermissionGroupsPermissionGroupId { get; set; }

        public ApiPermissionGroupsApiPermissionGroupsGetApiPermissionGroupsQuery(int apiPermissionGroupsPermissionGroupId)
        {
            this.ApiPermissionGroupsPermissionGroupId = apiPermissionGroupsPermissionGroupId;
        }

        public class ApiPermissionGroupsApiPermissionGroupsGetApiPermissionGroupsHandler : IRequestHandler<ApiPermissionGroupsApiPermissionGroupsGetApiPermissionGroupsQuery, Response<List<ApiPermissionGroupsGetApiPermissionGroupsResponseDto>>>
        {
            private readonly ILogger<ApiPermissionGroupsApiPermissionGroupsGetApiPermissionGroupsHandler> _logger;
            private readonly IMapper _mapper;
            private readonly IApiPermissionGroupsRepository _repository;
            public ApiPermissionGroupsApiPermissionGroupsGetApiPermissionGroupsHandler(IMapper mapper, ILogger<ApiPermissionGroupsApiPermissionGroupsGetApiPermissionGroupsHandler> logger, IApiPermissionGroupsRepository repository)
            {
                _mapper = mapper;
                _logger = logger;
                _repository = repository;
            }

            public async Task<Response<List<ApiPermissionGroupsGetApiPermissionGroupsResponseDto>>> Handle(ApiPermissionGroupsApiPermissionGroupsGetApiPermissionGroupsQuery request, CancellationToken cancellationToken)
            {
                var returnValue = _mapper.Map<Response<List<ApiPermissionGroupsGetApiPermissionGroupsResponseDto>>>(await _repository.ApiPermissionGroupsGetApiPermissionGroupsAsync(request.ApiPermissionGroupsPermissionGroupId));
                return returnValue;
            }
        }
    }
}