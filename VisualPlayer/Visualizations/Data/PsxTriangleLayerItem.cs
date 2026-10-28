using VisualPlayer.ViewModels;
using VisualPlayer.Visualizations.Config;

namespace VisualPlayer.Visualizations.Data;

public sealed class PsxTriangleLayerItem : ViewModelBase
{
    private readonly PsxTrianglesVisualizationConfig _config;

    #region PROPERTIES

    public string Description { get; }

    public int GroupIndex { get; }

    public double Opacity
    {
        get => _config.GetLayerOpacity(GroupIndex);
        set
        {
            var previousValue = Opacity;
            _config.SetLayerOpacity(GroupIndex, value);
            if (!Opacity.Equals(previousValue))
                NotifyPropertyChanged(nameof(Opacity));
        }
    }

    public string Title { get; }

    public int TriangleCount
    {
        get => _config.GetTriangleCount(GroupIndex);
        set
        {
            var previousValue = TriangleCount;
            _config.SetTriangleCount(GroupIndex, value);
            if (TriangleCount != previousValue)
                NotifyPropertyChanged(nameof(TriangleCount));
        }
    }

    #endregion PROPERTIES

    #region CONSTRUCTORS

    public PsxTriangleLayerItem(int groupIndex, string title, string description, PsxTrianglesVisualizationConfig config)
    {
        GroupIndex = groupIndex;
        Title = title;
        Description = description;
        _config = config;
    }

    #endregion CONSTRUCTORS
}
