using System.Threading.Tasks;
using Avalonia.Controls;
using VisualPlayer.Enums.UI;
using VisualPlayer.Models.Config;
using VisualPlayer.Views.Base;
using VisualPlayer.Views.Dialogs;

namespace VisualPlayer.Controls;

public class InternalMessageController : InternalControllerBase
{
    protected override System.Type StyleKeyOverride => typeof(InternalMessageController);


    protected override bool IsSupportedView(Control view)
    {
        return view is InternalMessageBase;
    }

    public void Open(InternalMessageBase message)
    {
        OpenCore(message);
    }

    public void Replace(InternalMessageBase message)
    {
        ReplaceCore(message);
    }

    public Task<AccentColor?> ShowColorPickerAsync(InternalMessageColorPicker colorPicker)
    {
        Open(colorPicker);
        return colorPicker.ResultTask;
    }

    public Task<InternalDialogResult> ShowDialogAsync(InternalMessageDialog dialog)
    {
        Open(dialog);
        return dialog.ResultTask;
    }

    public Task<InternalDialogResult> ShowDialogAsync(InternalMessageInput input)
    {
        Open(input);
        return input.ResultTask;
    }

    public Task<InternalDialogResult> ShowDialogAsync(InternalMessagePlayListPicker picker)
    {
        Open(picker);
        return picker.ResultTask;
    }
}
