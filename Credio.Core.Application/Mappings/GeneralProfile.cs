using AutoMapper;
using Credio.Core.Application.Dtos.Account;
using Credio.Core.Application.Features.Account.Commands.Authenticate;

namespace Credio.Core.Application.Mappings
{
    public class GeneralProfile : Profile
	{
		public GeneralProfile()
		{
			#region Account
			CreateMap<AuthenticationRequest, AuthenticateCommand>()
				.ReverseMap();
            #endregion
        }
    }
}
