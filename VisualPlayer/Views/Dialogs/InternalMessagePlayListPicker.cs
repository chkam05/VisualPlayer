using System.Collections.Generic;
using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Controls.Metadata;
using Avalonia.Controls.Primitives;
using Avalonia.Interactivity;
using VisualPlayer.Enums.UI;
using VisualPlayer.ViewModels;
using VisualPlayer.Views.Base;
using VisualPlayer.Contexts;

namespace VisualPlayer.Views.Dialogs;

[TemplatePart("PART_CancelButton", typeof(Button), IsRequired = true)]
[TemplatePart("PART_OkButton", typeof(Button), IsRequired = true)]
public class InternalMessagePlayListPicker : InternalMessageBase
{
    private static TranslationContext T => ApplicationContext.Instance.Translations;

    private readonly TaskCompletionSource<InternalDialogResult> _resultSource
        = new(TaskCreationOptions.RunContinuationsAsynchronously);

    private Button? _cancelButton;
    private Button? _okButton;

    #region PROPERTIES

    public IReadOnlyList<SavedPlayListViewModel> PlayLists { get; }
    public Task<InternalDialogResult> ResultTask => _resultSource.Task;
    public SavedPlayListViewModel? SelectedPlayList { get; set; }
    protected override System.Type StyleKeyOverride => typeof(InternalMessagePlayListPicker);

    #endregion PROPERTIES

    #region CONSTRUCTORS

    public InternalMessagePlayListPicker(IReadOnlyList<SavedPlayListViewModel> playLists)
    {
        Title = T["DIALOG_PLAYLIST_PICKER_TITLE"];
        PlayLists = playLists;
        SelectedPlayList = playLists.Count > 0 ? playLists[0] : null;
    }

    #endregion CONSTRUCTORS

    #region ACTIONS

    private void Complete(InternalDialogResult result)
    {
        _resultSource.TrySetResult(result);
        RequestClose();
    }

    protected override void OnClosed()
    {
        _resultSource.TrySetResult(InternalDialogResult.None);
    }

    #endregion ACTIONS

    #region BUTTON ACTIONS

    private void CancelButton_Click(object? sender, RoutedEventArgs e)
    {
        Complete(InternalDialogResult.Cancel);
    }

    private void OkButton_Click(object? sender, RoutedEventArgs e)
    {
        Complete(InternalDialogResult.Ok);
    }

    #endregion BUTTON ACTIONS

    #region TEMPLATE

    protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
    {
        DetachHandlers();
        base.OnApplyTemplate(e);
        _cancelButton = e.NameScope.Find<Button>("PART_CancelButton");
        _okButton = e.NameScope.Find<Button>("PART_OkButton");
        if (_cancelButton is not null) _cancelButton.Click += CancelButton_Click;
        if (_okButton is not null) _okButton.Click += OkButton_Click;
    }

    private void DetachHandlers()
    {
        if (_cancelButton is not null) _cancelButton.Click -= CancelButton_Click;
        if (_okButton is not null) _okButton.Click -= OkButton_Click;
        _cancelButton = null;
        _okButton = null;
    }

    #endregion TEMPLATE
}
