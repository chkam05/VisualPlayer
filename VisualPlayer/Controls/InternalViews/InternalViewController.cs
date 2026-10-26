using Avalonia.Controls;
using VisualPlayer.Views.Base;

namespace VisualPlayer.Controls.InternalViews;

public class InternalViewController : InternalControllerBase
{
    protected override System.Type StyleKeyOverride => typeof(InternalViewController);


    protected override bool IsSupportedView(Control view)
    {
        return view is InternalViewBase;
    }

    public void Open(InternalViewBase view)
    {
        OpenCore(view);
    }

    public void Replace(InternalViewBase view)
    {
        ReplaceCore(view);
    }

    protected override void UpdateViewState(Control? view)
    {
        if (view is InternalViewBase internalView)
            internalView.CanGoBack = CanGoBack;
    }
}