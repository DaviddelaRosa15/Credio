using System.Runtime.InteropServices.ComTypes;
using Credio.Core.Application.Interfaces.Repositories;
using Credio.Core.Domain.Entities;

namespace Credio.Core.Application.Seeds;

public static class DefaultApplicationStatus
{
    public static async Task Seed(IApplicationStatusRepository applicationStatusRepository)
    {
        List<ApplicationStatus> status =
        [
            new()
            {
                Name = "Pendiente",
                Description = "Pendiente"
            },
            new()
            {
                Name = "Aprobada",
                Description = "Aprobada"
            },
            new()
            {
                Name = "Rechazada",
                Description = "Rechazada"
            },
            new()
            {
                Name = "En Revision",
                Description = "En Revision"
            },
        ];
        
        await applicationStatusRepository.AddManyAsync(status);
    }
}