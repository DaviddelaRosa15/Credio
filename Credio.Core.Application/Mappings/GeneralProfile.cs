using AutoMapper;
using Credio.Core.Application.Dtos.Account;
using Credio.Core.Application.Dtos.Common;
using Credio.Core.Application.Dtos.Employee;
using Credio.Core.Application.Features.Account.Commands.Authenticate;
using Credio.Core.Application.Features.Account.Commands.RegisterClient;
using Credio.Core.Application.Features.Employee.Commands.RegisterEmployee;
using Credio.Core.Domain.Entities;

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
                .ForMember(x => x.PhoneNumber, opt => opt.MapFrom(y => y.Phone))
                .ForMember(x => x.Password, opt => opt.Ignore());
            #endregion

            #region Employee
            CreateMap<Employee, RegisterEmployeeCommand>()
                .ForMember(x => x.Image, opt => opt.Ignore())
                .ForMember(x => x.Role, opt => opt.Ignore())
                .ReverseMap()
                .ForMember(x => x.Address, opt => opt.Ignore())
                .ForMember(x => x.AddressId, opt => opt.Ignore())
                .ForMember(x => x.Clients, opt => opt.Ignore())
                .ForMember(x => x.DocumentType, opt => opt.Ignore())
                .ForMember(x => x.EmployeeCode, opt => opt.Ignore())
                .ForMember(x => x.Id, opt => opt.Ignore())
                .ForMember(x => x.Loans, opt => opt.Ignore())
                .ForMember(x => x.LoanApplications, opt => opt.Ignore())
                .ForMember(x => x.Routes, opt => opt.Ignore())
                .ForMember(x => x.UserId, opt => opt.Ignore())
                .ForMember(x => x.Payments, opt => opt.Ignore())
                .ForMember(x => x.Created, opt => opt.Ignore())
                .ForMember(x => x.CreatedBy, opt => opt.Ignore())
                .ForMember(x => x.LastModified, opt => opt.Ignore())
                .ForMember(x => x.LastModifiedBy, opt => opt.Ignore())
                .ForMember(x => x.IsDeleted, opt => opt.Ignore())
                .ForMember(x => x.Deleted, opt => opt.Ignore());

            CreateMap<Employee, RegisterEmployeeCommandResponse>()
                .ReverseMap()
                .ForMember(x => x.AddressId, opt => opt.Ignore())
                .ForMember(x => x.Clients, opt => opt.Ignore())
                .ForMember(x => x.DocumentType, opt => opt.Ignore())
                .ForMember(x => x.EmployeeCode, opt => opt.Ignore())
                .ForMember(x => x.Loans, opt => opt.Ignore())
                .ForMember(x => x.LoanApplications, opt => opt.Ignore())
                .ForMember(x => x.Routes, opt => opt.Ignore())
                .ForMember(x => x.UserId, opt => opt.Ignore())
                .ForMember(x => x.Payments, opt => opt.Ignore())
                .ForMember(x => x.Created, opt => opt.Ignore())
                .ForMember(x => x.CreatedBy, opt => opt.Ignore())
                .ForMember(x => x.LastModified, opt => opt.Ignore())
                .ForMember(x => x.LastModifiedBy, opt => opt.Ignore())
                .ForMember(x => x.IsDeleted, opt => opt.Ignore())
                .ForMember(x => x.Deleted, opt => opt.Ignore());

            CreateMap<Employee, EmployeeDTO>()
                .ForMember(x => x.DocumentType, opt => opt.MapFrom(y => y.DocumentType.Name))
                .ReverseMap()
                .ForMember(x => x.Clients, opt => opt.Ignore())
                .ForMember(x => x.DocumentType, opt => opt.Ignore())
                .ForMember(x => x.Loans, opt => opt.Ignore())
                .ForMember(x => x.LoanApplications, opt => opt.Ignore())
                .ForMember(x => x.Routes, opt => opt.Ignore())
                .ForMember(x => x.UserId, opt => opt.Ignore())
                .ForMember(x => x.Payments, opt => opt.Ignore())
                .ForMember(x => x.Created, opt => opt.Ignore())
                .ForMember(x => x.CreatedBy, opt => opt.Ignore())
                .ForMember(x => x.LastModified, opt => opt.Ignore())
                .ForMember(x => x.LastModifiedBy, opt => opt.Ignore())
                .ForMember(x => x.IsDeleted, opt => opt.Ignore())
                .ForMember(x => x.Deleted, opt => opt.Ignore());
            #endregion

            #region Address
            CreateMap<Address, AddressDTO>()
                .ReverseMap()
                .ForMember(x => x.Id, opt => opt.Ignore())
                .ForMember(x => x.Employees, opt => opt.Ignore())
                .ForMember(x => x.Clients, opt => opt.Ignore())
                .ForMember(x => x.Created, opt => opt.Ignore())
                .ForMember(x => x.CreatedBy, opt => opt.Ignore())
                .ForMember(x => x.LastModified, opt => opt.Ignore())
                .ForMember(x => x.LastModifiedBy, opt => opt.Ignore())
                .ForMember(x => x.IsDeleted, opt => opt.Ignore())
                .ForMember(x => x.Deleted, opt => opt.Ignore());
            #endregion
        }
    }
}
