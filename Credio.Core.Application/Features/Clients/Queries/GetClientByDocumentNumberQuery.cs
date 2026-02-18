using MediatR;
using Credio.Core.Application.Dtos.User;


namespace Credio.Core.Application.Features.Clients.Queries 
{
    public sealed class GetClientByDocumentNumberQuery
    : IRequest<ClientListDto>
    {
        public string DocumentNumber { get; init; }
    }
}

