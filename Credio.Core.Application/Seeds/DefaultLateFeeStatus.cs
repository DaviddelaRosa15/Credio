using Credio.Core.Application.Interfaces.Repositories;
using Credio.Core.Domain.Entities;

namespace Credio.Core.Application.Seeds;

public static class DefaultLateFeeStatus
{
    public static async Task SeedAsync(ILateFeeStatusRepository lateFeeStatusRepository)
    {
        List<LateFeeStatus> status =
        [
            new()
            {
                Name = "Pendiente",
                Description = "Pendiente"
            },
            new()
            {
                Name = "Pagada",
                Description = "Pagada"
            },
        ];
        
        await lateFeeStatusRepository.AddManyAsync(status);
    }
}