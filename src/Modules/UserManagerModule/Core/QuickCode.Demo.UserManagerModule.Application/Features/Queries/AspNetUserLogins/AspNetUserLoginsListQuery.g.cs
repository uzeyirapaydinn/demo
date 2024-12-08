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
    public class AspNetUserLoginsListQuery : IRequest<Response<List<AspNetUserLoginsDto>>>
    {
        public int? PageNumber { get; set; }
        public int? PageSize { get; set; }

        public AspNetUserLoginsListQuery(int? pageNumber, int? pageSize)
        {
            this.PageNumber = pageNumber;
            this.PageSize = pageSize;
        }

        public class AspNetUserLoginsListHandler : IRequestHandler<AspNetUserLoginsListQuery, Response<List<AspNetUserLoginsDto>>>
        {
            private readonly ILogger<AspNetUserLoginsListHandler> _logger;
            private readonly IMapper _mapper;
            private readonly IAspNetUserLoginsRepository _repository;
            public AspNetUserLoginsListHandler(IMapper mapper, ILogger<AspNetUserLoginsListHandler> logger, IAspNetUserLoginsRepository repository)
            {
                _mapper = mapper;
                _logger = logger;
                _repository = repository;
            }

            public async Task<Response<List<AspNetUserLoginsDto>>> Handle(AspNetUserLoginsListQuery request, CancellationToken cancellationToken)
            {
                var returnValue = _mapper.Map<Response<List<AspNetUserLoginsDto>>>(await _repository.ListAsync(request.PageNumber, request.PageSize));
                return returnValue;
            }
        }
    }
}