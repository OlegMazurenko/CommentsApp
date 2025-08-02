using CommentsApp.API.Models;

namespace CommentsApp.API.Services.Interfaces;

public interface IFileService
{
    List<FileUpload> ValidateAndProcessFiles(IEnumerable<UploadedFileDto> fileDtos, Comment comment);
}