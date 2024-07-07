using Common;
using Newtonsoft.Json.Linq;
namespace ImageHandling;

public class PrepareToResizeImageDecorator(ICommandHandler<ResizeImageCommand> resizeImage) : ICommandHandlerAsync<PrepareToResizeImageDecoratorCommand>
{
    public async Task Execute(PrepareToResizeImageDecoratorCommand command)
    {
        command.FolderName = CommonStaticMethods.GetValue(command.Data, "folderName");

        command.ImageFileName = command.Data?["imageFileName"]?.ToString() ?? "default.jpg";

        var resizeImageCommand = new ResizeImageCommand
        {
            CanvasHeight = 200,
            CanvasWidth = 200,
            OriginalFileName = command.ImageFileName,
            SaveTo = command.ImageFileName
        };
        resizeImageCommand.CreateDirectories(command.FolderName);
        command.ImageFileNameWithFullPath = resizeImageCommand.OriginalFileName;

        string base64Image = command.Data?["base64Image"]?.ToString() ?? string.Empty;
        byte[] imageBytes = Convert.FromBase64String(base64Image);
        await File.WriteAllBytesAsync(resizeImageCommand.OriginalFileName, imageBytes);

        resizeImage.Execute(resizeImageCommand);
    }
}