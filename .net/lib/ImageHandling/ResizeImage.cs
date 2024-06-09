using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using Common;

namespace ImageHandling;
[System.Runtime.Versioning.SupportedOSPlatform("windows")]
public class ResizeImage: ICommandHandler<ResizeImageCommand>
{
    public void Execute(ResizeImageCommand command)
    {
        if (command.OriginalFileName is null || command.SaveTo is null)
        {
            throw new NullReferenceException("OriginalFileName or SaveTo is null");
        }

        using Image image = Image.FromFile(command.OriginalFileName);
        using Image thumbnail = new Bitmap(command.CanvasWidth, command.CanvasHeight);
        using Graphics graphic = Graphics.FromImage(thumbnail);
        graphic.InterpolationMode = InterpolationMode.HighQualityBicubic;
        graphic.SmoothingMode = SmoothingMode.HighQuality;
        graphic.PixelOffsetMode = PixelOffsetMode.HighQuality;
        graphic.CompositingQuality = CompositingQuality.HighQuality;

        int originalWidth = image.Width;
        int originalHeight = image.Height;

        double ratioX = command.CanvasWidth / (double)originalWidth;
        double ratioY = command.CanvasHeight / (double)originalHeight;

        double ratio = ratioX < ratioY ? ratioX : ratioY;

        int newHeight = Convert.ToInt32(originalHeight * ratio);
        int newWidth = Convert.ToInt32(originalWidth * ratio);

        int posX = Convert.ToInt32((command.CanvasWidth - originalWidth * ratio) / 2);
        int posY = Convert.ToInt32((command.CanvasHeight - originalHeight * ratio) / 2);

        graphic.Clear(Color.White); // white padding

        graphic.DrawImage(image, posX, posY, newWidth, newHeight);

        ImageCodecInfo[] info = ImageCodecInfo.GetImageEncoders();
        var encoderParameters = new EncoderParameters(1);
        encoderParameters.Param[0] = new EncoderParameter(Encoder.Quality,
            100L);

        int orientationKey = 0x0112;
        const int notSpecified = 0;
        const int normalOrientation = 1;
        const int mirrorHorizontal = 2;
        const int upsideDown = 3;
        const int mirrorVertical = 4;
        const int mirrorHorizontalAndRotateRight = 5;
        const int rotateLeft = 6;
        const int mirrorHorizontalAndRotateLeft = 7;
        const int rotateRight = 8;

        if (image.PropertyIdList.Contains(orientationKey))
        {
            if (image.PropertyIdList.Contains(orientationKey))
            {
                byte[]? value = image.GetPropertyItem(orientationKey)!.Value;
                if (value != null)
                {
                    var orientation = (int)value[0];
                    switch (orientation)
                    {
                        case notSpecified: // Assume it is good.
                        case normalOrientation:
                            // No rotation required.
                            break;
                        case mirrorHorizontal:
                            thumbnail.RotateFlip(RotateFlipType.RotateNoneFlipX);
                            break;
                        case upsideDown:
                            thumbnail.RotateFlip(RotateFlipType.Rotate180FlipNone);
                            break;
                        case mirrorVertical:
                            thumbnail.RotateFlip(RotateFlipType.Rotate180FlipX);
                            break;
                        case mirrorHorizontalAndRotateRight:
                            thumbnail.RotateFlip(RotateFlipType.Rotate90FlipX);
                            break;
                        case rotateLeft:
                            thumbnail.RotateFlip(RotateFlipType.Rotate90FlipNone);
                            break;
                        case mirrorHorizontalAndRotateLeft:
                            thumbnail.RotateFlip(RotateFlipType.Rotate270FlipX);
                            break;
                        case rotateRight:
                            thumbnail.RotateFlip(RotateFlipType.Rotate270FlipNone);
                            break;
                        default:
                            throw new NotImplementedException("An orientation of " + orientation +
                                                              " isn't implemented.");
                    }
                }
            }
        }

        //thumbnail.RotateFlip(RotateFlipType.Rotate90FlipNone);
        thumbnail.Save(command.SaveTo, info[1], encoderParameters);
    }
}