using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;

using KoreCommon;
using KoreCommon.SkiaSharp;
using SkiaSharp;

namespace KoreCommon.UnitTest;

public static class KoreTestSkiaSharp
{
    // KoreTestSkiaSharp.RunTests(testLog)
    public static void RunTests(KoreTestLog testLog)
    {
        TestBasicImage(testLog);
    }

    // Draw a testcard image
    private static void TestBasicImage(KoreTestLog testLog)
    {
        // Create a new image with a testcard pattern
        var imagePlotter = new KoreSkiaSharpPlotter(1000, 1000);

        // Draw a boundary
        KoreXYRect boundsRect = new KoreXYRect(0, 0, 1000, 1000);
        KoreXYRect boundsRectInset = boundsRect.Inset(5);

        SKPaint fillPaint = new SKPaint
        {
            Style = SKPaintStyle.Stroke,
            StrokeWidth = 1,
            Color = SKColors.Black,
            IsAntialias = false // Match the line's anti-aliasing setting
        };
        imagePlotter.DrawRect(boundsRectInset, fillPaint);

        // Test Lines - Various line widths and colors
        int xStart = 10;
        int yStart = 10;
        int yEnd   = 100;

        KoreXYPoint startPnt = new KoreXYPoint(xStart, yStart);
        KoreXYPoint endPnt   = new KoreXYPoint(xStart, yEnd);

        imagePlotter.DrawSettings.LineWidth = 1;
        imagePlotter.DrawSettings.Color = SKColors.Black;
        imagePlotter.DrawSettings.IsAntialias = false; // Disable anti-aliasing for crisp 1px lines
        imagePlotter.DrawLine(startPnt, endPnt);

        // Draw text in a specific box
        imagePlotter.DrawSettings.Color = SKColors.Red;
        KoreXYPoint markPoint = new KoreXYPoint(100, 50);
        imagePlotter.DrawPointAsCross(markPoint, 5);
        KoreXYPoint markPoint2 = new KoreXYPoint(markPoint.X, markPoint.Y + 30); // Move down for the next text
        imagePlotter.DrawPointAsCross(markPoint2, 5);
        KoreXYPoint markPoint3 = new KoreXYPoint(markPoint2.X, markPoint2.Y + 30); // Move down for the next text
        imagePlotter.DrawPointAsCross(markPoint3, 5);

        imagePlotter.DrawSettings.Color = SKColors.Black;
        string testText = "Test Text";
        imagePlotter.DrawText(testText, markPoint, 20);
        imagePlotter.DrawTextCentered("Another Line of Text", markPoint2, 20);

        imagePlotter.DrawTextAtPosition("Yet Another Line of Text", markPoint3, KoreXYRectPosition.Center, 20);

        // Save the image to a file
        string filePath = "testcard.png";
        imagePlotter.Save(filePath);
    }
}


