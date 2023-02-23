﻿using Application.Interfaces;
using Application.Wrappers;
using Domain.Entities;
using MediatR;

namespace Application.Features.Clientes.Commands.DeleteClienteCommand
{
    public class DeleteClienteCommand : IRequest<Response<int>>
    {
        public int Id { get; set; }
    }

    public class DeleteClienteCommandHandler : IRequestHandler<DeleteClienteCommand, Response<int>>
    {
        private readonly IRepositoryAsync<Cliente> _repositoryAsync;

        public DeleteClienteCommandHandler(IRepositoryAsync<Cliente> repositoryAsync)
        {
            _repositoryAsync = repositoryAsync;
        }

        public async Task<Response<int>> Handle(DeleteClienteCommand request, CancellationToken cancellationToken)
        {
            var record = await _repositoryAsync.GetByIdAsync(request.Id);

            if (record == null)
            {
                throw new KeyNotFoundException($"Registro no encontrado con id {request.Id}");
            }
            else
            {
                await _repositoryAsync.DeleteAsync(record);

                return new Response<int>(record.Id);
            }
        }
    }
}
