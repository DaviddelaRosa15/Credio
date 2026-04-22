using Credio.Core.Application.Enums;
using Credio.Core.Application.Interfaces.Repositories;
using Credio.Core.Domain.Entities;
using Credio.Infrastructure.Identity.Entities;
using Microsoft.AspNetCore.Identity;

namespace Credio.Infrastructure.Identity.Seeds
{
    public static class DefaultSuperAdminUser
    {
        public static async Task SeedAsync(UserManager<ApplicationUser> userManager,
            IClientRepository clientRepository, IDocumentTypeRepository documentTypeRepository)
        {
            ApplicationUser defaultUser = new();
            defaultUser.UserName = "superAdminUser";
            defaultUser.Email = "superadminuser@email.com";
            defaultUser.FirstName = "SuperUser";
            defaultUser.LastName = "SuperUser";
            defaultUser.Address = "Brisas del Este";
            defaultUser.UrlImage = "no hay por ahora";
            defaultUser.EmailConfirmed = true;
            defaultUser.PhoneNumberConfirmed = true;

            await userManager.CreateAsync(defaultUser, "1505Pa@@word");
            await userManager.AddToRoleAsync(defaultUser, Roles.SuperAdmin.ToString());
            await userManager.AddToRoleAsync(defaultUser, Roles.Administrator.ToString());
            await userManager.AddToRoleAsync(defaultUser, Roles.Client.ToString());
            await userManager.AddToRoleAsync(defaultUser, Roles.Collector.ToString());
            await userManager.AddToRoleAsync(defaultUser, Roles.Officer.ToString());

            Address address = new Address
            {
                AddressLine1 = defaultUser.Address,
                City = "Santo Domingo Este",
                Region = "Santo Domingo",
                StreetNumber = "12",
                PostalCode = "12345"
            };

            // Obtener el tipo de documento "CEDULA" para asociarlo al empleado
            var documentType = await documentTypeRepository.GetByPropertyAsync(dt => dt.Name == "CEDULA");

            // Registrar el cliente asociado al usuario superadmin
            await RegisterSuperAdminInCore(defaultUser, address, clientRepository, documentType.Id);
        }

        private static async Task RegisterSuperAdminInCore(ApplicationUser defaultUser, Address address,
            IClientRepository clientRepository, string documentType)
        {

            var employee = new Core.Domain.Entities.Employee
            {
                Address = address,
                DocumentNumber = "123456",
                DocumentTypeId = documentType,
                Email = defaultUser.Email,
                EmployeeCode = defaultUser.UserName,
                FirstName = defaultUser.FirstName,
                IsCollector = true,
                LastName = defaultUser.LastName,
                Phone = "123456789",
                UserId = defaultUser.Id,
            };

            var client = new Client
            {
                Address = address,
                DocumentNumber = "123456",
                DocumentTypeId = documentType,
                Email = defaultUser.Email,
                Employee = employee,
                FirstName = defaultUser.FirstName,
                HomeLatitude = 0,
                HomeLongitude = 0,
                LastName = defaultUser.LastName,
                Phone = "123456789",
                UserId = defaultUser.Id,
            };

            await clientRepository.AddAsync(client);
        }
    }
}