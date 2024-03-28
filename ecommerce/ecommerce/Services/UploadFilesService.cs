using ecommerce.DTO;
using ecommerce.Services.Interface;

namespace ecommerce.Services
{
    public class UploadFilesService : IUploadFilesService
    {
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public UploadFilesService(IWebHostEnvironment webHostEnvironment, IHttpContextAccessor httpContextAccessor)
        {
            _webHostEnvironment = webHostEnvironment;
            _httpContextAccessor = httpContextAccessor;
        }

        public string GetFilePath(string fileName, string nameFolder)
        {
            string ImageUrl = string.Empty;
            string HostUrl = $"{_httpContextAccessor.HttpContext.Request.Scheme}://{_httpContextAccessor.HttpContext.Request.Host}";
            var filePath = Path.Combine(_webHostEnvironment.WebRootPath, nameFolder, fileName);
            if (!File.Exists(filePath))
            {
                ImageUrl = HostUrl + "/none.png";
            }
            else
            {
                ImageUrl = HostUrl + "/" + nameFolder + "/" + fileName;
            }
            return ImageUrl;
        }

        public string GetImageHostPath(string fileName, string nameFolder)
        {
            return _webHostEnvironment.WebRootPath + $"\\{nameFolder}\\";
        }

        public async Task<ApiResponse<string>> RemoveFileAsync(string fileName, string nameFolder)
        {
            var filePath = Path.Combine(_webHostEnvironment.WebRootPath, nameFolder, fileName);
            if (File.Exists(filePath))
            {
                File.Delete(filePath);
                return new ApiResponse<string> { Data = fileName, Message = "File removed successfully", Status = true };
            }
            return new ApiResponse<string> { Message = "File not found", Status = false };

        }

        public async Task<ApiResponse<List<string>>> RemoveFilesAsync(List<string> fileNames, string nameFolder)
        {
            var removedFiles = new List<string>();
            foreach (var fileName in fileNames)
            {
                var filePath = Path.Combine(_webHostEnvironment.WebRootPath, nameFolder, fileName);
                if (File.Exists(filePath))
                {
                    File.Delete(filePath);
                    removedFiles.Add(fileName);
                }
            }
            if (removedFiles.Count > 0)
            {
                return new ApiResponse<List<string>> { Data = removedFiles, Message = "Files removed successfully", Status = true };
            }
            return new ApiResponse<List<string>> { Message = "Files not found", Status = false };
        }

        public async Task<ApiResponse<string>> UploadFileAsync(IFormFile file, string nameFolder = "uploads")
        {
            if (file != null)
            {
                string uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, nameFolder);

                if (!Directory.Exists(uploadsFolder))
                {
                    Directory.CreateDirectory(uploadsFolder);
                }

                string uniqueFileName = Guid.NewGuid().ToString() + "_" + file.FileName;
                uniqueFileName.Replace(" ", "");
                string filePath = Path.Combine(uploadsFolder, uniqueFileName);

                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(fileStream);
                }

                // Do something with the uploaded image file path if needed
                return new ApiResponse<string> { Data = uniqueFileName, Message = "File uploaded successfully", Status = true };
            }
            else
            {
                return new ApiResponse<string> { Message = "File is empty", Status = false };
            }
        }

        public async Task<ApiResponse<List<string>>> UploadFilesAsync(List<IFormFile> files, string nameFolder)
        {
            if (files != null && files.Count > 0)
            {
                string galleryFolder = Path.Combine(_webHostEnvironment.WebRootPath, $"{nameFolder}");

                if (!Directory.Exists(galleryFolder))
                {
                    Directory.CreateDirectory(galleryFolder);
                }
                var fileNames = new List<string>();

                foreach (var file in files)
                {
                    if (file.Length > 0)
                    {
                        string uniqueFileName = Guid.NewGuid().ToString() + "_" + file.FileName;
                        uniqueFileName.Replace(" ", "");
                        string filePath = Path.Combine(galleryFolder, uniqueFileName);

                        using (var fileStream = new FileStream(filePath, FileMode.Create))
                        {
                            await file.CopyToAsync(fileStream);
                        }

                        // Do something with the uploaded gallery file paths if needed
                        fileNames.Add(uniqueFileName);
                    }
                }
                return new ApiResponse<List<string>> { Data = fileNames.ToList(), Message = "Files uploaded successfully", Status = true };
            }
            return new ApiResponse<List<string>> { Message = "Files are empty", Status = false };
        }
    }
}