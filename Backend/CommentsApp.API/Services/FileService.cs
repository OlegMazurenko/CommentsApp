using CommentsApp.API.Models;
using CommentsApp.API.Services.Interfaces;

using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;

namespace CommentsApp.API.Services;

public class FileService : IFileService
{
    public List<FileUpload> ValidateAndProcessFiles(IEnumerable<UploadedFileDto> fileDtos, Comment comment)
    {
        var files = new List<FileUpload>();

        foreach (var fileDto in fileDtos)
        {
            byte[] content;

            try
            {
                content = Convert.FromBase64String(fileDto.Base64Content);
            }
            catch
            {
                throw new InvalidDataException($"Invalid base64 in file: {fileDto.FileName}");
            }

            if (fileDto.ContentType == "text/plain" && content.Length > 100_000)
            {
                throw new InvalidDataException("Text file too large");
            }

            if (fileDto.ContentType.StartsWith("image/"))
            {
                content = ResizeImageIfNeeded(content, 320, 240);
            }

            files.Add(new FileUpload
            {
                FileName = fileDto.FileName,
                ContentType = fileDto.ContentType,
                Content = content,
                Comment = comment
            });
        }

        return files;
    }

    private static byte[] ResizeImageIfNeeded(byte[] imageBytes, int maxWidth, int maxHeight)
    {
        using var inputStream = new MemoryStream(imageBytes);
        using var image = Image.Load<Rgba32>(inputStream);

        if (image.Width <= maxWidth && image.Height <= maxHeight)
            return imageBytes;

        image.Mutate(ctx => ctx.Resize(new ResizeOptions
        {
            Mode = ResizeMode.Max,
            Size = new Size(maxWidth, maxHeight)
        }));

        using var outputStream = new MemoryStream();
        image.SaveAsPng(outputStream);
        return outputStream.ToArray();
    }
}
