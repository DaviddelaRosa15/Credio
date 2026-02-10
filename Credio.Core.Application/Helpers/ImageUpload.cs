using Microsoft.AspNetCore.Http;

namespace Avalanche.Core.Application.Helpers
{
    public static class ImageUpload
    {
        //Subir imagen
        public static string UploadImage(IFormFile file, string item, bool isEditMode = false, string imagePath = "")
        {
            if (isEditMode && file == null)
            {
                return imagePath;
            }

            string basePath = $"/Assets/Images/{item}/";
            string path = Path.Combine(Directory.GetCurrentDirectory(), $"wwwroot{basePath}");

            //create folder if not exist
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }

            //get file extension
            if (file != null)
            {
                Guid guid = Guid.NewGuid();
                FileInfo fileInfo = new(file.FileName);
                string fileName = guid + fileInfo.Extension;

                string fileNameWithPath = Path.Combine(path, fileName);

                using (var stream = new FileStream(fileNameWithPath, FileMode.Create))
                {
                    file.CopyTo(stream);
                }

                if (isEditMode)
                {
                    if (imagePath != null)
                    {
                        string[] oldImagePart = imagePath.Split("/");
                        string oldImagePath = oldImagePart[^1];
                        string completeImageOldPath = Path.Combine(path, oldImagePath);

                        if (System.IO.File.Exists(completeImageOldPath))
                        {
                            System.IO.File.Delete(completeImageOldPath);
                        }
                    }

                }
                return $"{basePath}{fileName}";
            }
            return null;
        }

        //Eliminar imagen
        public static void DeleteFile(string url)
        {

            //Get current directory
            string basePath = url;

            if (basePath != "/Assets/Images/default.jpg")
            {
                string servePath = Directory.GetCurrentDirectory();

                string path = Path.Combine(servePath, $"wwwroot{basePath}");

                if (File.Exists(path))
                {
                    File.Delete(path);
                }
            }
        }
    }
}
