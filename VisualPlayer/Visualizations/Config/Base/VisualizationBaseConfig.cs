namespace VisualPlayer.Visualizations.Config.Base;

public static class VisualizationBaseConfig
{
    #region CONST MIN-MAX

    public const int BARS_COUNT_MAXIMUM = 256;
    public const int BARS_COUNT_MINIMUM = 4;
    public const int BAR_HEIGHT_MAXIMUM = 64;
    public const int BAR_HEIGHT_MINIMUM = 1;
    public const int BAR_SPACE_MAXIMUM = 32;
    public const int BAR_SPACE_MINIMUM = 0;
    public const int DESCENT_RATE_INCREMENT = 10;
    public const int DESCENT_RATE_MAXIMUM = 2000;
    public const int DESCENT_RATE_MINIMUM = 1;
    public const int LINE_THICKNESS_MAXIMUM = 20;
    public const int LINE_THICKNESS_MINIMUM = 1;
    public const int INNER_RADIUS_INCREMENT = 4;
    public const int INNER_RADIUS_MAXIMUM = 512;
    public const int INNER_RADIUS_MINIMUM = 4;
    public const int ROTATION_INCREMENT = 5;
    public const int ROTATION_MAXIMUM = 359;
    public const int ROTATION_MINIMUM = 0;
    public const int ROTATION_SPEED_INCREMENT = 5;
    public const int ROTATION_SPEED_MAXIMUM = 360;
    public const int ROTATION_SPEED_MINIMUM = 1;
    public const int CENTER_OFFSET_INCREMENT = 4;
    public const int CENTER_OFFSET_MAXIMUM = 1000;
    public const int CENTER_OFFSET_MINIMUM = -1000;
    public const int TRANSFORMATION_MINIMUM_INCREMENT = 5;
    public const int TRANSFORMATION_MINIMUM_MAXIMUM = 90;
    public const int TRANSFORMATION_MINIMUM_MINIMUM = 0;

    #endregion CONST MIN-MAX

    #region CONST DEFAULT

    public const int DEFAULT_BAR_HEIGHT = 3;
    public const int DEFAULT_BAR_HORIZONTAL_SPACE = 2;
    public const int DEFAULT_BAR_VERTICAL_SPACE = 2;
    public const int DEFAULT_BARS_COUNT = 48;
    public const int DEFAULT_CIRCLE_BARS_COUNT = 64;
    public const int DEFAULT_DESCENT_RATE = 360;
    public const int DEFAULT_FLOATING_DESCENT_RATE = 60;
    public const int DEFAULT_LINE_THICKNESS = 1;
    public const int DEFAULT_ROTATION_SPEED = 10;
    public const int DEFAULT_CENTER_OFFSET_X = -12;
    public const int DEFAULT_CENTER_OFFSET_Y = 0;
    public const int DEFAULT_TRANSFORMATION_MINIMUM = 0;

    #endregion CONST DEFAULT
}
