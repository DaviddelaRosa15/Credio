using AutoMapper;
using Credio.Core.Application.Dtos.Account;
using Credio.Core.Application.Features.Account.Commands.Authenticate;
using Credio.Core.Application.Features.Account.Commands.RegisterClient;
using Credio.Core.Application.Features.Account.Commands.RegisterEmployee;

namespace Credio.Core.Application.Mappings
{
    public class GeneralProfile : Profile
	{
		public GeneralProfile()
		{
			#region Account
			CreateMap<AuthenticationRequest, AuthenticateCommand>()
				.ReverseMap();

            CreateMap<RegisterRequest, RegisterClientCommand>()
                .ForMember(x => x.Address, opt => opt.Ignore())
                .ForMember(x => x.Image, opt => opt.Ignore())
                .ReverseMap()
                .ForMember(x => x.Password, opt => opt.Ignore());

            CreateMap<RegisterRequest, RegisterEmployeeCommand>()
                .ForMember(x => x.Address, opt => opt.Ignore())
                .ForMember(x => x.Image, opt => opt.Ignore())
                .ForMember(x => x.Role, opt => opt.Ignore())
                .ReverseMap()
                .ForMember(x => x.Password, opt => opt.Ignore());
            #endregion
        }
    }
}
