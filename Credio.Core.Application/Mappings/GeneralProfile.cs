using AutoMapper;
using Credio.Core.Application.Dtos.Account;
using Credio.Core.Application.Dtos.Catalog;
using Credio.Core.Application.Dtos.Client;
using Credio.Core.Application.Dtos.Common;
using Credio.Core.Application.Dtos.CoreConfiguration;
using Credio.Core.Application.Dtos.Employee;
using Credio.Core.Application.Dtos.Loan;
using Credio.Core.Application.Dtos.LoanApplication;
using Credio.Core.Application.Dtos.LoanStatus;
using Credio.Core.Application.Features.Account.Commands.Authenticate;
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

            CreateMap<RegisterRequest, RegisterEmployeeCommand>()
                .ForMember(x => x.Address, opt => opt.Ignore())
                .ForMember(x => x.Image, opt => opt.Ignore())
                .ForMember(x => x.Role, opt => opt.Ignore())
                .ReverseMap()
                .ForMember(x => x.PhoneNumber, opt => opt.MapFrom(y => y.Phone))
                .ForMember(x => x.Password, opt => opt.Ignore());
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

            #region AmortizationSchedule
            CreateMap<AmortizationSchedule, InstallmentDTO>()
                .ReverseMap()
                .ForMember(x => x.Id, opt => opt.Ignore())
                .ForMember(x => x.Created, opt => opt.Ignore())
                .ForMember(x => x.CreatedBy, opt => opt.Ignore())
                .ForMember(x => x.LastModified, opt => opt.Ignore())
                .ForMember(x => x.LastModifiedBy, opt => opt.Ignore())
                .ForMember(x => x.IsDeleted, opt => opt.Ignore())
                .ForMember(x => x.Deleted, opt => opt.Ignore());
            
            CreateMap<AmortizationSchedule, UpcomingInstallmentDTO>()
                .ForMember(dest => dest.Client,
                    opt => opt.MapFrom(src => src.Loan.Client.FirstName + " " + src.Loan.Client.LastName))
                .ForMember(dest => dest.Loan,
                    opt => opt.MapFrom(src => src.Loan.LoanNumber))
                .ForMember(dest => dest.State,
                    opt => opt.MapFrom(src => src.AmortizationStatus.Description));
            #endregion

            #region Catalog
            CreateMap<LoanStatus, LoanStatusDTO>();

            CreateMap<PaymentFrequency, PaymentFrequencyDTO>();

            CreateMap<ApplicationStatus, ApplicationStatusDTO>();

            CreateMap<DocumentType, DocumentTypeDTO>();

            CreateMap<PaymentMethod, PaymentMethodDTO>();
            #endregion

            #region Client
            CreateMap<Client, ClientDTO>()
                .ForMember(x => x.DocumentType, opt => opt.MapFrom(y => y.DocumentType.Name))
                .ForMember(x => x.OfficerId, opt => opt.MapFrom(y => y.EmployeeId))
                .ForMember(x => x.HomeLatitude, opt => opt.MapFrom(y => y.HomeLatitude))
                .ForMember(x => x.HomeLongitude, opt => opt.MapFrom(y => y.HomeLongitude))
                .ReverseMap()
                .ForMember(x => x.DocumentType, opt => opt.Ignore())
                .ForMember(x => x.Loans, opt => opt.Ignore())
                .ForMember(x => x.LoanApplications, opt => opt.Ignore())
                .ForMember(x => x.Route, opt => opt.Ignore())
                .ForMember(x => x.UserId, opt => opt.Ignore())
                .ForMember(x => x.Created, opt => opt.Ignore())
                .ForMember(x => x.CreatedBy, opt => opt.Ignore())
                .ForMember(x => x.LastModified, opt => opt.Ignore())
                .ForMember(x => x.LastModifiedBy, opt => opt.Ignore())
                .ForMember(x => x.IsDeleted, opt => opt.Ignore())
                .ForMember(x => x.Deleted, opt => opt.Ignore());

            CreateMap<Client, ClientDetailDTO>()
                .ForMember(x => x.DocumentType, opt => opt.MapFrom(y => y.DocumentType.Name))
                .ForMember(x => x.OfficerId, opt => opt.MapFrom(y => y.EmployeeId))
                .ReverseMap()
                .ForMember(x => x.DocumentType, opt => opt.Ignore())
                .ForMember(x => x.Loans, opt => opt.Ignore())
                .ForMember(x => x.LoanApplications, opt => opt.Ignore())
                .ForMember(x => x.Route, opt => opt.Ignore())
                .ForMember(x => x.UserId, opt => opt.Ignore())
                .ForMember(x => x.Created, opt => opt.Ignore())
                .ForMember(x => x.CreatedBy, opt => opt.Ignore())
                .ForMember(x => x.LastModified, opt => opt.Ignore())
                .ForMember(x => x.LastModifiedBy, opt => opt.Ignore())
                .ForMember(x => x.IsDeleted, opt => opt.Ignore())
                .ForMember(x => x.Deleted, opt => opt.Ignore());

            CreateMap<Client, ClientBasicDTO>()
                .ForMember(x => x.DocumentType, opt => opt.MapFrom(y => y.DocumentType.Name))
                .ForMember(x => x.FullName, opt => opt.MapFrom(y => y.FirstName + y.LastName))
                .ReverseMap()
                .ForMember(x => x.DocumentType, opt => opt.Ignore())
                .ForMember(x => x.Loans, opt => opt.Ignore())
                .ForMember(x => x.LoanApplications, opt => opt.Ignore())
                .ForMember(x => x.Route, opt => opt.Ignore())
                .ForMember(x => x.UserId, opt => opt.Ignore())
                .ForMember(x => x.Created, opt => opt.Ignore())
                .ForMember(x => x.CreatedBy, opt => opt.Ignore())
                .ForMember(x => x.LastModified, opt => opt.Ignore())
                .ForMember(x => x.LastModifiedBy, opt => opt.Ignore())
                .ForMember(x => x.IsDeleted, opt => opt.Ignore())
                .ForMember(x => x.Deleted, opt => opt.Ignore());
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

            CreateMap<Employee, EmployeeDetailDTO>()
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

            #region Loan
            CreateMap<Loan, LoanDTO>()
                .ReverseMap();

            CreateMap<Loan, LoanReportItemDto>()
                .ForMember(dest => dest.Client,
                    opt => opt.MapFrom(src => src.Client.FirstName + " " + src.Client.LastName))
                .ForMember(dest => dest.OriginalAmount,
                    opt => opt.MapFrom(src => src.Amount))
                .ForMember(dest => dest.State,
                    opt => opt.MapFrom(src => src.LoanStatus.Description))
                .ForMember(dest => dest.OutstandingBalance,
                    opt => opt.MapFrom(src => src.LoanBalance.TotalOutstanding))
                .ForMember(dest => dest.DaysInArrears,
                    opt => opt.MapFrom(src => src.LoanBalance.DaysInArrears))
                .ForMember(dest => dest.TotalFeeCount,
                    opt => opt.MapFrom(src => src.AmortizationSchedules.Count()))
                .ForMember(dest => dest.TotalFeePaidCount,
                    opt => opt.MapFrom(src => src.AmortizationSchedules.Count(x => x.AmortizationStatus.Description == "Pagado")));
            
            CreateMap<Loan, LoanStatusDTO>()
                .ForMember(x => x.Id, opt => opt.MapFrom(y => y.Id))
                .ForMember(x => x.Name, opt => opt.MapFrom(y => y.LoanStatus.Name))
                .ForMember(x => x.Description, opt => opt.MapFrom(y => y.LoanStatus.Description));

            CreateMap<Loan, LoanAmortizationScheduleResponseDTO>()
                .ForMember(dest => dest.ClientFullName,
                    opt => opt.MapFrom(src => src.Client.FirstName + " " + src.Client.LastName))
                .ForMember(dest => dest.LoanId,
                    opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.PrincipalAmount,
                    opt => opt.MapFrom(src => src.Amount))
                .ForMember(dest => dest.AmortizationSchedule,
                    opt => opt.MapFrom(src => src.AmortizationSchedules.OrderBy(a => a.InstallmentNumber)));                
            #endregion

            #region LoanApplication
            CreateMap<LoanApplication, LoanApplicationDto>()
                .ForMember(dest => dest.ClientName,
                    opt => opt.MapFrom(src => src.Client.FirstName + " " + src.Client.LastName))
                .ForMember(dest => dest.ApplicationStatusName,
                    opt => opt.MapFrom(src => src.ApplicationStatus.Name))
                .ForMember(dest => dest.PaymentFrequency,
                    opt => opt.MapFrom(src => src.PaymentFrequency.Name));
            #endregion

            #region SystemSettings
            CreateMap<SystemSettings, SystemSettingDTO>()
                .ReverseMap();
            #endregion
        }
    }
}
