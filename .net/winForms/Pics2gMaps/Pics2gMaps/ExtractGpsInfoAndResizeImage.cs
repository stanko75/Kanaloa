using Common;
using ImageHandling;

namespace Pics2gMaps;

public class ExtractGpsInfoAndResizeImage : ICommandHandlerAsync<ResizeImageDesktopCommand>
{
    public async Task Execute(ResizeImageDesktopCommand command)
    {
        var resizeImageCommand = new ResizeImageCommand
        {
            CanvasHeight = 200,
            CanvasWidth = 200,
            OriginalFileName = Path.GetFileName("imageFileName"),
            //SaveTo = Path.Join(@"C:\projects\KanaloaGalleryTest\mariaLaach\thumbs", imageFileName)
            SaveTo = Path.GetFileName("imageFileName")
        };
        resizeImageCommand.CreateDirectories("folderName");

        ResizeImage resizeImage = new ResizeImage();
        List<object> tt = new List<object>();
        tt.Add(new ResizeImage());


        /*
        var t = new ParallelForEachAndExtractGpsInfoWrapper();
        await t.Execute(command);
        */
    }
}