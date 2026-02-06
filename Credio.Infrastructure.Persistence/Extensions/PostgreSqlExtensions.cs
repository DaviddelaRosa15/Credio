using Microsoft.EntityFrameworkCore;

namespace Credio.Infrastructure.Persistence.Extensions
{
    public static class PostgreSqlExtensions
    {
        public static void ConfigurePostgreSqlTypes(this ModelBuilder modelBuilder)
        {
            foreach (var entityType in modelBuilder.Model.GetEntityTypes())
            {
                foreach (var property in entityType.GetProperties())
                {
                    ConfigurePropertyType(property);
                }
            }
        }

        private static void ConfigurePropertyType(Microsoft.EntityFrameworkCore.Metadata.IMutableProperty property)
        {
            switch (property.ClrType)
            {
                case Type t when t == typeof(DateTime) || t == typeof(DateTime?):
                    property.SetColumnType("timestamp without time zone");
                    break;
            }
        }
    }
}