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
    public class OtpTypesDeleteCommand : IRequest<Response<bool>>
    {
        public OtpTypesDto request { get; set; }

        public OtpTypesDeleteCommand(OtpTypesDto request)
        {
            this.request = request;
        }

        public class OtpTypesDeleteHandler : IRequestHandler<OtpTypesDeleteCommand, Response<bool>>
        {
            private readonly ILogger<OtpTypesDeleteHandler> _logger;
            private readonly IMapper _mapper;
            private readonly IOtpTypesRepository _repository;
            public OtpTypesDeleteHandler(IMapper mapper, ILogger<OtpTypesDeleteHandler> logger, IOtpTypesRepository repository)
            {
                _mapper = mapper;
                _logger = logger;
                _repository = repository;
            }

            public async Task<Response<bool>> Handle(OtpTypesDeleteCommand request, CancellationToken cancellationToken)
            {
                var model = _mapper.Map<OtpTypes>(request.request);
                var returnValue = _mapper.Map<Response<bool>>(await _repository.DeleteAsync(model));
                return returnValue;
            }
        }
    }
}