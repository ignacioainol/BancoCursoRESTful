﻿using Application.DTOs;
using Application.Interfaces;
using Application.Specifications;
using Application.Wrappers;
using AutoMapper;
using Domain.Entities;
using MediatR;
using Microsoft.Extensions.Caching.Distributed;
using Newtonsoft.Json;
using System.Text;

namespace Application.Features.Clientes.Queries.GetAllClientes
{
    public class GetAllClientesQuery : IRequest<PagedResponse<List<ClienteDto>>>
    {
        public int PageSize { get; set; }
        public int PageNumber { get; set; }

        public string? Nombre { get; set; }

        public string? Apellido { get; set; }

        public class GetAllClientesQueryHandler : IRequestHandler<GetAllClientesQuery, PagedResponse<List<ClienteDto>>>
        {
            private readonly IRepositoryAsync<Cliente> _repositoryAsync;
            private readonly IDistributedCache _distributedCache;
            private readonly IMapper _mapper;

            public GetAllClientesQueryHandler(IRepositoryAsync<Cliente> repositoryAsync, IMapper mapper, IDistributedCache distributedCache)
            {
                _repositoryAsync = repositoryAsync;
                _mapper = mapper;
                _distributedCache = distributedCache;
            }

            public async Task<PagedResponse<List<ClienteDto>>> Handle(GetAllClientesQuery request, CancellationToken cancellationToken)
            {
                var cacheKey = $"listadoClientes_{request.PageSize}_{request.PageNumber}_{request.Nombre}_{request.Apellido}";
                string serializedListadoClientes;
                var listadoClientes = new List<Cliente>();
                var redisListadoClientes = await _distributedCache.GetAsync(cacheKey);

                if (redisListadoClientes != null)
                {
                    serializedListadoClientes = Encoding.UTF8.GetString(redisListadoClientes);
                    listadoClientes = JsonConvert.DeserializeObject<List<Cliente>>(serializedListadoClientes);
                }
                else
                {
                    listadoClientes = await _repositoryAsync.ListAsync(new PagedClientesSpecifications(request.PageSize, request.PageNumber, request.Nombre, request.Apellido));
                    serializedListadoClientes = JsonConvert.SerializeObject(listadoClientes);
                    redisListadoClientes = Encoding.UTF8.GetBytes(serializedListadoClientes);

                    var options = new DistributedCacheEntryOptions()
                        .SetAbsoluteExpiration(DateTime.Now.AddMinutes(10))
                        .SetSlidingExpiration(TimeSpan.FromMinutes(2));
                    await _distributedCache.SetAsync(cacheKey, redisListadoClientes, options);

                }

                var clientesDto = _mapper.Map<List<ClienteDto>>(listadoClientes);

                return new PagedResponse<List<ClienteDto>>(clientesDto, request.PageNumber, request.PageSize);
            }
        }
    }
}
