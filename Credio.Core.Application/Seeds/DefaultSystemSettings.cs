using Credio.Core.Application.Constants;
using Credio.Core.Application.Interfaces.Repositories;
using Credio.Core.Domain.Entities;

namespace Credio.Core.Application.Seeds
{
    public static class DefaultSystemSettings
    {
        public static async Task SeedAsync(ISystemSettingsRepository systemSettingsRepository)
        {            
            try
            {
                List<SystemSettings> systemSettingsList =
                [
                    new()
                    {
                        Key = EndOfDaySettings.COBExecutionTimeKey,
                        Description = "Hora del COB",
                        Value = "23:59",
                        DataType = "Time",
                        GroupName = EndOfDaySettings.GroupName,
                        IsEditable = true
                    },
                    new()
                    {
                        Key = EndOfDaySettings.DailyLateFeeRateKey,
                        Description = "Tasa de mora",
                        Value = "0.01",
                        DataType = "Decimal",
                        GroupName = EndOfDaySettings.GroupName,
                        IsEditable = true
                    },
                    new()
                    {
                        Key = EndOfDaySettings.GracePeriodDaysKey,
                        Description = "Dias de gracia",
                        Value = "2",
                        DataType = "Integer",
                        GroupName = EndOfDaySettings.GroupName,
                        IsEditable = true
                    },
                    new()
                    {
                        Key = EndOfDaySettings.TechSupportEmailKey,
                        Description = "Correo tecnico",
                        Value = "RLOPEZMORALES626@GMAIL.COM",
                        DataType = "Email",
                        GroupName = EndOfDaySettings.GroupName,
                        IsEditable = true
                    },
                    new()
                    {
                        Key = FinancialRulesSettings.LoanMinInterestRateKey,
                        Description = "Tasa mínima mensual",
                        Value = "0.01",
                        DataType = "Decimal",
                        GroupName = FinancialRulesSettings.GroupName,
                        IsEditable = true
                    },
                    new()
                    {
                        Key = FinancialRulesSettings.LoanMaxInterestRateKey,
                        Description = "Tasa máxima mensual",
                        Value = "0.60",
                        DataType = "Decimal",
                        GroupName = FinancialRulesSettings.GroupName,
                        IsEditable = true
                    },
                    new()
                    {
                        Key = FinancialRulesSettings.LoanMinTermMonthsKey,
                        Description = "Cantidad minima de meses/cuotas",
                        Value = "1",
                        DataType = "Integer",
                        GroupName = FinancialRulesSettings.GroupName,
                        IsEditable = true
                    },
                    new()
                    {
                        Key = FinancialRulesSettings.LoanMaxTermMonthsKey,
                        Description = "Cantidad maxima de meses/cuotas",
                        Value = "60",
                        DataType = "Integer",
                        GroupName = FinancialRulesSettings.GroupName,
                        IsEditable = true
                    },
                ];

                await systemSettingsRepository.AddManyAsync(systemSettingsList);
            }
            catch
            {
                throw;
            }
        }
    }
}