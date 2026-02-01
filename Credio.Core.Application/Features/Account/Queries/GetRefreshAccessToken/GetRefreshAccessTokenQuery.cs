using Credio.Core.Application.Dtos.Account;
using Credio.Core.Application.Interfaces.Services;
using Credio.Core.Domain.Settings;
using MediatR;
using Microsoft.Extensions.Options;

namespace Credio.Core.Application.Features.Account.Queries.GetRefreshAccessToken
{
    public class GetRefreshAccessTokenQuery : IRequest<RefreshTokenResponse>
	{

	}

	public class GetRefreshAccessTokenQueryHandler : IRequestHandler<GetRefreshAccessTokenQuery, RefreshTokenResponse>
	{

		private readonly IAccountService _accountService;
        private readonly JWTSettings _jwtSettings;

		public GetRefreshAccessTokenQueryHandler(IAccountService accountService, IOptions<JWTSettings> jwtSettings)
		{
			_accountService = accountService;
			_jwtSettings = jwtSettings.Value;
		}

		public async Task<RefreshTokenResponse> Handle(GetRefreshAccessTokenQuery request, CancellationToken cancellationToken)
		{
			RefreshTokenResponse response = new();

			var result = _accountService.ValidateRefreshToken();

			if(result.Contains("Error") || result == "")
			{
				response.HasError = true;
				response.Error = "No hay refresh token disponible";
				return response;
			}

			var refresh = await _accountService.GenerateJWToken(result);
			response.JWToken = refresh;
            response.ExpiresIn = (_jwtSettings.DurationInMinutes * 60).ToString();
            response.ExpiresAt = DateTime.Now.AddMinutes(_jwtSettings.DurationInMinutes);

            return response;
		}
	}
}
