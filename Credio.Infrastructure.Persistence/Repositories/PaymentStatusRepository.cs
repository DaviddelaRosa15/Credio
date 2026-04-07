using Credio.Core.Application.Interfaces.Repositories;
using Credio.Core.Domain.Entities;
using Credio.Infrastructure.Persistence.Contexts;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Credio.Infrastructure.Persistence.Repositories
{
    internal class PaymentStatusRepository : GenericRepository<PaymentStatus>, IPaymentStatusRepository
    {
        public PaymentStatusRepository(IDbContextFactory<ApplicationContext> dbContextFactory) : base(dbContextFactory)
        {
        }
    }
}

