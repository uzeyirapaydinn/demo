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
    public class PortalPermissionTypesUpdateCommand : IRequest<Response<bool>>
    {
        public int Id { get; set; }
        public PortalPermissionTypesDto request { get; set; }

        public PortalPermissionTypesUpdateCommand(int id, PortalPermissionTypesDto request)
        {
            this.request = request;
            this.Id = id;
        }

        public class PortalPermissionTypesUpdateHandler : IRequestHandler<PortalPermissionTypesUpdateCommand, Response<bool>>
        {
            private readonly ILogger<PortalPermissionTypesUpdateHandler> _logger;
            private readonly IMapper _mapper;
            private readonly IPortalPermissionTypesRepository _repository;
            public PortalPermissionTypesUpdateHandler(IMapper mapper, ILogger<PortalPermissionTypesUpdateHandler> logger, IPortalPermissionTypesRepository repository)
            {
                _mapper = mapper;
                _logger = logger;
                _repository = repository;
            }

            public async Task<Response<bool>> Handle(PortalPermissionTypesUpdateCommand request, CancellationToken cancellationToken)
            {
                var updateItem = await _repository.GetByPkAsync(request.Id);
                if (updateItem.Code == 404)
                {
                    return new Response<bool>()
                    {
                        Code = 404,
                        Value = false
                    };
                }

                var model = _mapper.Map<PortalPermissionTypes>(request.request);
                var returnValue = _mapper.Map<Response<bool>>(await _repository.UpdateAsync(model));
                return returnValue;
            }
        }
    }
}