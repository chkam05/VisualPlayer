using Avalonia;
using Avalonia.Media;
using VisualPlayer.Enums;
using VisualPlayer.Utils;
using VisualPlayer.Visualizations.Config;
using VisualPlayer.Visualizations.Config.Base;

namespace VisualPlayer.Visualizations;

public class StripesBouncingVisualization : BouncingVisualizationBase<StripesBouncingVisualizationConfig>
{
    protected override int ColumnCount => Config.StripesCount;

    public StripesBouncingVisualization(StripesBouncingVisualizationConfig config, VisualizationColorConfig colorConfig)
        : base(config, colorConfig) { }

    protected override IBrush PrepareBrush(IBrush brush)
    {
        var stretch = ColorConfig.Stretch / 100d;
        return ColorConfig.Mode switch
        {
            VisualizationColorMode.Solid => new SolidColorBrush(ColorConfig.SolidColor.ToColor()),
            VisualizationColorMode.Gradient => GradientBrushUtils.Create(ColorConfig.Gradient, GradientOffsetX, GradientOffsetY, stretch),
            VisualizationColorMode.Rainbow => GradientBrushUtils.CreateRainbow(GradientOffsetX, GradientOffsetY,
                stretch, ColorConfig.RainbowAngle, ColorConfig.RainbowGradientType),
            _ => brush
        };
    }

    protected override void DrawBand(DrawingContext context, IBrush brush,
        double x, double width, double innerLevel, double outerLevel)
    {
        var height = outerLevel - innerLevel;
        if (innerLevel <= 0)
        {
            DrawSlice(context, brush, new Rect(x, DrawingArea.Center.Y - outerLevel, width, outerLevel * 2));
            return;
        }

        DrawSlice(context, brush, new Rect(x, DrawingArea.Center.Y - outerLevel, width, height));
        DrawSlice(context, brush, new Rect(x, DrawingArea.Center.Y + innerLevel, width, height));
    }

    private void DrawSlice(DrawingContext context, IBrush brush, Rect slice)
    {
        using (context.PushClip(slice))
            context.DrawRectangle(brush, null, DrawingArea);
    }
}
