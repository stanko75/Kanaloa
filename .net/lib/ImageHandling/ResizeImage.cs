using Common;
using SkiaSharp;

namespace ImageHandling;

public class ResizeImage : ICommandHandler<ResizeImageCommand>
{
    public void Execute(ResizeImageCommand command)
    {
        if (command.OriginalFileName is null || command.SaveTo is null)
        {
            throw new NullReferenceException(
                "OriginalFileName or SaveTo is null");
        }

        if (command.CanvasWidth <= 0 || command.CanvasHeight <= 0)
        {
            throw new ArgumentOutOfRangeException(
                nameof(command), "Canvas dimensions must be positive.");
        }

        using var codec = SKCodec.Create(command.OriginalFileName)
            ?? throw new InvalidOperationException("Unable to decode image.");

        var orientation = codec.EncodedOrigin;

        using var original = SKBitmap.Decode(codec)
            ?? throw new InvalidOperationException("Unable to decode bitmap.");

        bool swapDimensions = orientation is
            SKEncodedOrigin.LeftTop or
            SKEncodedOrigin.RightTop or
            SKEncodedOrigin.RightBottom or
            SKEncodedOrigin.LeftBottom;

        int originalWidth = swapDimensions ? original.Height : original.Width;
        int originalHeight = swapDimensions ? original.Width : original.Height;

        double ratioX = command.CanvasWidth / (double)originalWidth;
        double ratioY = command.CanvasHeight / (double)originalHeight;
        double ratio = Math.Min(ratioX, ratioY);

        int newWidth = Math.Max(1, Convert.ToInt32(originalWidth * ratio));
        int newHeight = Math.Max(1, Convert.ToInt32(originalHeight * ratio));

        int posX = Convert.ToInt32((command.CanvasWidth - newWidth) / 2.0);
        int posY = Convert.ToInt32((command.CanvasHeight - newHeight) / 2.0);

        using var thumbnail = new SKBitmap(
            command.CanvasWidth,
            command.CanvasHeight);

        using (var canvas = new SKCanvas(thumbnail))
        {
            canvas.Clear(SKColors.White);

            canvas.Save();

            // EXIF-Orientierung anwenden
            switch (orientation)
            {
                case SKEncodedOrigin.TopRight:
                    canvas.Translate(command.CanvasWidth, 0);
                    canvas.Scale(-1, 1);
                    break;

                case SKEncodedOrigin.BottomRight:
                    canvas.Translate(command.CanvasWidth, command.CanvasHeight);
                    canvas.RotateDegrees(180);
                    break;

                case SKEncodedOrigin.BottomLeft:
                    canvas.Translate(0, command.CanvasHeight);
                    canvas.Scale(1, -1);
                    break;

                case SKEncodedOrigin.LeftTop:
                    canvas.Translate(0, command.CanvasHeight);
                    canvas.RotateDegrees(-90);
                    canvas.Scale(-1, 1);
                    break;

                case SKEncodedOrigin.RightTop:
                    canvas.Translate(command.CanvasWidth, 0);
                    canvas.RotateDegrees(90);
                    break;

                case SKEncodedOrigin.RightBottom:
                    canvas.Translate(command.CanvasWidth, 0);
                    canvas.RotateDegrees(90);
                    canvas.Scale(-1, 1);
                    break;

                case SKEncodedOrigin.LeftBottom:
                    canvas.Translate(0, command.CanvasHeight);
                    canvas.RotateDegrees(-90);
                    break;
            }

            // Bild mit hochwertiger Interpolation zeichnen
            using var paint = new SKPaint
            {
                IsAntialias = true
            };

            var source = new SKRect(
                0, 0, original.Width, original.Height);

            // Zielabmessungen vor der Orientierungstransformation
            int drawWidth = swapDimensions ? newHeight : newWidth;
            int drawHeight = swapDimensions ? newWidth : newHeight;

            var destination = new SKRect(
                posX,
                posY,
                posX + drawWidth,
                posY + drawHeight);

            canvas.DrawBitmap(original, source, destination, paint);

            canvas.Restore();
            canvas.Flush();
        }

        using var image = SKImage.FromBitmap(thumbnail);

        using var data = image.Encode(SKEncodedImageFormat.Jpeg, 100)
            ?? throw new InvalidOperationException("Unable to encode JPEG.");

        using var stream = File.Create(command.SaveTo);
        data.SaveTo(stream);
    }
}