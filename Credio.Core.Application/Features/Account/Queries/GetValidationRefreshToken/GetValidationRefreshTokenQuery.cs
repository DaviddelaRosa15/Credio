using Credio.Core.Application.Interfaces.Services;
using MediatR;

namespace Credio.Core.Application.Features.Account.Queries.GetValidationRefreshToken
{
    public class GetValidationRefreshTokenQuery : IRequest<GetValidationRefreshTokenQueryResponse>
    {
    }

    public class GetValidationRefreshTokenQueryHandler : IRequestHandler<GetValidationRefreshTokenQuery, GetValidationRefreshTokenQueryResponse>
    {
        private readonly ITokenService _tokenService;

        public GetValidationRefreshTokenQueryHandler(ITokenService tokenService)
        {
            _tokenService = tokenService;
        }


        public async Task<GetValidationRefreshTokenQueryResponse> Handle(GetValidationRefreshTokenQuery command, CancellationToken cancellationToken)
        {
            GetValidationRefreshTokenQueryResponse response = new();
            try
            {
                var result = _tokenService.ValidateRefreshToken();
                response.ValidRefreshToken = result != "" && !result.Contains("Error");
                return response;
            }
            catch (Exception ex)
            {
                response.ValidRefreshToken = false;
                return response;
            }
        }

    }
}
