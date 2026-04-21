using Credio.Core.Application.Dtos.Loan;
using Credio.Core.Application.Dtos.Payment;
using Credio.Core.Application.Interfaces.Services;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace Credio.Infrastructure.Shared.Services
{
    public class PdfService : IPdfService
    {
        // Paleta de colores inspirada en el branding de Credio
        private readonly string CredioPrimary = "#6b7a3a"; // Verde base del gradiente
        private readonly string CredioSuccess = "#8a9a50"; // Verde secundario / acentos
        private readonly string CredioTextSecondary = "#777777"; // Gris para legibilidad

        private readonly string logoPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "EmailTemplates", "logo.png");

        public byte[] GenerateDisbursementReceipt(DisburseLoanNotificationDTO data)
        {
            QuestPDF.Settings.License = LicenseType.Community;
            return Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Margin(1, Unit.Inch);
                    page.DefaultTextStyle(x => x.FontSize(11).FontFamily(Fonts.Verdana));

                    // 1. HEADER
                    page.Header().Background(CredioPrimary).Padding(20).Row(row =>
                    {
                        row.ConstantItem(60).Height(60).Background(Colors.White).Padding(8).Image(logoPath);

                        row.RelativeItem().PaddingLeft(15).Column(col =>
                        {
                            col.Item().Text("Comprobante de Desembolso").FontSize(20).FontColor(Colors.White).SemiBold();
                            col.Item().Text(data.EffectiveDate.ToString("D")).FontSize(10).FontColor(Colors.White);
                        });

                        row.ConstantItem(100).AlignRight().Column(loanCol => {
                            loanCol.Item().Text("Préstamo").FontSize(9).FontColor(Colors.White);
                            loanCol.Item().Text(data.LoanNumber.ToString()).FontSize(14).SemiBold().FontColor(Colors.White);
                        });
                    });

                    // 2. CONTENT (Datos y Condiciones)
                    page.Content().PaddingVertical(20).Column(col =>
                    {
                        // Monto Principal
                        col.Item().AlignCenter().PaddingBottom(10).Text("Monto Total Desembolsado").FontSize(14).FontColor(CredioTextSecondary);
                        col.Item().AlignCenter().Text($"RD$ {data.LoanAmount:N2}").FontSize(36).ExtraBold().FontColor(Colors.Black);

                        col.Item().PaddingVertical(15).LineHorizontal(1).LineColor(Colors.Grey.Lighten2);

                        // Sección: Datos del Cliente
                        col.Item().PaddingBottom(10).Text("Datos del Cliente").FontSize(12).SemiBold().FontColor(CredioPrimary);
                        col.Item().Table(table =>
                        {
                            table.ColumnsDefinition(columns => { columns.RelativeColumn(); columns.RelativeColumn(); });
                            AddRow(table, "Nombre Completo", data.ClientName);
                            AddRow(table, "Documento de Identidad", data.DocumentNumber);
                        });

                        col.Item().PaddingVertical(15).LineHorizontal(1).LineColor(Colors.Grey.Lighten4);

                        // Sección: Condiciones del Préstamo
                        col.Item().PaddingBottom(10).Text("Condiciones del Préstamo").FontSize(12).SemiBold().FontColor(CredioPrimary);
                        col.Item().Table(table =>
                        {
                            table.ColumnsDefinition(columns => { columns.RelativeColumn(); columns.RelativeColumn(); });
                            AddRow(table, "Tasa de Interés", $"{data.InterestRate}%");
                            AddRow(table, "Plazo", data.Term.ToString());
                            AddRow(table, "Frecuencia de Pago", data.PaymentFrequency);
                        });
                    });

                    // 3. FOOTER
                    page.Footer().Column(col =>
                    {
                        col.Item().PaddingTop(40).AlignCenter().Width(250).LineHorizontal(1).LineColor(Colors.Black);
                        col.Item().AlignCenter().Text("Firma del Cliente").FontSize(10).FontColor(CredioTextSecondary);

                        col.Item().PaddingTop(10).AlignCenter().Text("Confirmo la recepción conforme de los fondos descritos en este documento.").FontSize(8).FontColor(CredioTextSecondary).Italic();

                        col.Item().PaddingTop(20).AlignCenter().Column(f => {
                            f.Item().Text("Sistema de Gestión de Préstamos - Credio").FontSize(9).FontColor(Colors.Grey.Medium);
                        });
                    });
                });
            }).GeneratePdf();
        }

        public byte[] GeneratePaymentReceipt(PaymentNotificationDTO data)
        {
            // Importante: Configurar la licencia de QuestPDF
            QuestPDF.Settings.License = LicenseType.Community;

            return Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Margin(1, Unit.Inch);
                    page.DefaultTextStyle(x => x.FontSize(11).FontFamily(Fonts.Verdana));

                    // 1. HEADER
                    page.Header().Background(CredioPrimary).Padding(20).Column(col =>
                    {
                        col.Item().Row(row =>
                        {
                            // Logo a la izquierda
                            row.ConstantItem(60).Height(60).Background(Colors.White).Padding(8).Image(logoPath);

                            row.RelativeItem().PaddingLeft(15).Column(innerCol =>
                            {
                                innerCol.Item().Text("Recibo de Pago").FontSize(22).FontColor(Colors.White).SemiBold();

                                // Receipt Number destacado sutilmente
                                innerCol.Item().Text($"Comprobante: {data.ReceiptNumber}").FontSize(10).FontColor(Colors.White);

                                innerCol.Item().Text(data.PaymentDate.ToString("D")).FontSize(10).FontColor(Colors.White);
                            });

                            row.ConstantItem(100).AlignRight().Column(loanCol => {
                                loanCol.Item().Text("Préstamo").FontSize(9).FontColor(Colors.White);
                                loanCol.Item().Text(data.LoanNumber.ToString()).FontSize(14).SemiBold().FontColor(Colors.White);
                            });
                        });
                    });

                    // 2. CONTENT (Monto y Detalles)
                    page.Content().PaddingVertical(20).Column(col =>
                    {
                        // Icono de éxito y Monto
                        col.Item().AlignCenter().PaddingBottom(10).Text("Monto Pagado").FontSize(14).FontColor(CredioTextSecondary);
                        col.Item().AlignCenter().Text($"RD$ {data.AmountPaid:N2}").FontSize(36).ExtraBold().FontColor(Colors.Black);

                        col.Item().PaddingVertical(10).LineHorizontal(1).LineColor(Colors.Grey.Lighten2);

                        // Tabla de detalles
                        col.Item().Table(table =>
                        {
                            table.ColumnsDefinition(columns =>
                            {
                                columns.RelativeColumn();
                                columns.RelativeColumn();
                            });

                            AddRow(table, "Cliente", data.ClientName);
                            AddRow(table, "Método de Pago", data.PaymentMethod);
                            AddRow(table, "Cuotas Pagadas", data.InstallmentsProgress);
                        });

                        col.Item().PaddingVertical(15).LineHorizontal(1).LineColor(Colors.Grey.Lighten4);

                        // Distribución del Pago
                        col.Item().PaddingBottom(10).Text("Distribución del Pago").FontSize(12).SemiBold().FontColor(CredioPrimary);

                        col.Item().Table(table =>
                        {
                            table.ColumnsDefinition(columns =>
                            {
                                columns.RelativeColumn();
                                columns.RelativeColumn();
                            });

                            AddRow(table, "Abono a Capital", $"RD$ {data.TotalPrincipalAppliedAmount:N2}");
                            AddRow(table, "Interés", $"RD$ {data.TotalInterestAppliedAmount:N2}");
                            AddRow(table, "Mora (Late Fee)", $"RD$ {data.TotalLateFeeAppliedAmount:N2}");
                        });

                        col.Item().PaddingVertical(15).LineHorizontal(1).LineColor(Colors.Grey.Lighten2);

                        // Saldo Final
                        col.Item().Table(table =>
                        {
                            table.ColumnsDefinition(columns =>
                            {
                                columns.RelativeColumn();
                                columns.RelativeColumn();
                            });

                            AddRow(table, "Saldo Pendiente", $"RD$ {data.RemainingBalance:N2}");
                        });
                    });

                    // 3. FOOTER
                    page.Footer().AlignCenter().Column(f => {
                        f.Item().Text("Sistema de Gestión de Préstamos").FontSize(9).FontColor(Colors.Grey.Medium);
                        f.Item().Text("Este recibo es válido como comprobante de pago").FontSize(9).FontColor(Colors.Grey.Medium);
                    });
                });
            }).GeneratePdf();
        }

        private void AddRow(TableDescriptor table, string label, string value)
        {
            table.Cell().PaddingVertical(5).Text(label).FontColor(CredioTextSecondary);
            table.Cell().PaddingVertical(5).AlignRight().Text(value).SemiBold();
        }
    }
}
