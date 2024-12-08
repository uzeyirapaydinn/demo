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
    public class AspNetUserClaimsGetItemQuery : IRequest<Response<AspNetUserClaimsDto>>
    {
        public int Id { get; set; }

        public AspNetUserClaimsGetItemQuery(int id)
        {
            this.Id = id;
        }

        public class AspNetUserClaimsGetItemHandler : IRequestHandler<AspNetUserClaimsGetItemQuery, Response<AspNetUserClaimsDto>>
        {
            private readonly ILogger<AspNetUserClaimsGetItemHandler> _logger;
            private readonly IMapper _mapper;
            private readonly IAspNetUserClaimsRepository _repository;
            public AspNetUserClaimsGetItemHandler(IMapper mapper, ILogger<AspNetUserClaimsGetItemHandler> logger, IAspNetUserClaimsRepository repository)
            {
                _mapper = mapper;
                _logger = logger;
                _repository = repository;
            }

            public async Task<Response<AspNetUserClaimsDto>> Handle(AspNetUserClaimsGetItemQuery request, CancellationToken cancellationToken)
            {
                var returnValue = _mapper.Map<Response<AspNetUserClaimsDto>>(await _repository.GetByPkAsync(request.Id));
                return returnValue;
            }
        }
    }
}