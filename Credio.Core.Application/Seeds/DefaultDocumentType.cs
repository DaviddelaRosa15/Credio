using Credio.Core.Application.Interfaces.Repositories;
using Credio.Core.Domain.Entities;

namespace Credio.Core.Application.Seeds
{
    public static class DefaultDocumentType
    {
        public static async Task SeedAsync(IDocumentTypeRepository documentTypeRepository)
        {
            List<DocumentType> documentTypeList = new();
            try
            {
                var documentTypes = new List<string>
                    {
                        "CEDULA",
                        "PASAPORTE",
                        "RNC"
                    };

                foreach (var item in documentTypes)
                {
                    DocumentType documentType = new()
                    {
                        Name = item,
                        Description = item
                    };

                    documentTypeList.Add(documentType);
                }

                await documentTypeRepository.AddManyAsync(documentTypeList);
            }
            catch (Exception ex)
            {
                throw;
            }
        }
    }
}
