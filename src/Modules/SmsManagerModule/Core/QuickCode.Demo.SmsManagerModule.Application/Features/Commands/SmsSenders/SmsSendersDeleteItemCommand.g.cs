using AutoMapper;
using System.Linq;
using MediatR;
using Microsoft.Extensions.Logging;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using QuickCode.Demo.SmsManagerModule.Application.Models;
using QuickCode.Demo.SmsManagerModule.Domain.Entities;
using QuickCode.Demo.SmsManagerModule.Application.Interfaces.Repositories;
using QuickCode.Demo.SmsManagerModule.Application.Dtos;

namespace QuickCode.Demo.SmsManagerModule.Application.Features
{
    public class SmsSendersDeleteItemCommand : IRequest<Response<bool>>
    {
        public int Id { get; set; }

        public SmsSendersDeleteItemCommand(int id)
        {
            this.Id = id;
        }

        public class SmsSendersDeleteItemHandler : IRequestHandler<SmsSendersDeleteItemCommand, Response<bool>>
        {
            private readonly ILogger<SmsSendersDeleteItemHandler> _logger;
            private readonly IMapper _mapper;
            private readonly ISmsSendersRepository _repository;
            public SmsSendersDeleteItemHandler(IMapper mapper, ILogger<SmsSendersDeleteItemHandler> logger, ISmsSendersRepository repository)
            {
                _mapper = mapper;
                _logger = logger;
                _repository = repository;
            }

            public async Task<Response<bool>> Handle(SmsSendersDeleteItemCommand request, CancellationToken cancellationToken)
            {
                var deleteItem = await _repository.GetByPkAsync(request.Id);
                if (deleteItem.Code == 404)
                {
                    return new Response<bool>()
                    {
                        Code = 404,
                        Value = false
                    };
                }

                var returnValue = _mapper.Map<Response<bool>>(await _repository.DeleteAsync(deleteItem.Value));
                return returnValue;
            }
        }
    }
}