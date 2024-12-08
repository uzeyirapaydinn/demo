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
    public class PortalPermissionsTotalItemCountQuery : IRequest<Response<int>>
    {
        public PortalPermissionsTotalItemCountQuery()
        {
        }

        public class PortalPermissionsTotalItemCountHandler : IRequestHandler<PortalPermissionsTotalItemCountQuery, Response<int>>
        {
            private readonly ILogger<PortalPermissionsTotalItemCountHandler> _logger;
            private readonly IMapper _mapper;
            private readonly IPortalPermissionsRepository _repository;
            public PortalPermissionsTotalItemCountHandler(IMapper mapper, ILogger<PortalPermissionsTotalItemCountHandler> logger, IPortalPermissionsRepository repository)
            {
                _mapper = mapper;
                _logger = logger;
                _repository = repository;
            }

            public async Task<Response<int>> Handle(PortalPermissionsTotalItemCountQuery request, CancellationToken cancellationToken)
            {
                var returnValue = _mapper.Map<Response<int>>(await _repository.CountAsync());
                return returnValue;
            }
        }
    }
}