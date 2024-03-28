using ecommerce.DTO;

namespace ecommerce.Services.Interface
{
    public interface IUploadFilesService
    {
        Task<ApiResponse<string>> UploadFileAsync(IFormFile file,string nameFolder);
        Task<ApiResponse<List<string>>> UploadFilesAsync(List<IFormFile> files,string nameFolder);
        // remove file
        Task<ApiResponse<string>> RemoveFileAsync(string fileName, string nameFolder);
        Task<ApiResponse<List<string>>> RemoveFilesAsync(List<string> fileNames, string nameFolder);
        // Get file path 
        string GetFilePath(string fileName, string nameFolder);
        // Get image host path by file name, name folder
        string GetImageHostPath(string fileName, string nameFolder);
    }
}
