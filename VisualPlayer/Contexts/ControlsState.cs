using System;
using System.ComponentModel;
using VisualPlayer.ViewModels;
using VisualPlayer.ViewModels.Config;

namespace VisualPlayer.Contexts;

public sealed class ControlsState : ViewModelBase, IDisposable
{
    private readonly AppearanceConfig _appearanceConfig;
    private bool _areControlsHiddenByClick;
    private bool _isMiniMode;

    #region PROPERTIES

    public bool AreControlsHiddenByClick
    {
        get => _areControlsHiddenByClick;
        private set
        {
            if (_areControlsHiddenByClick == value)
                return;

            _areControlsHiddenByClick = value;
            NotifyPropertyChanged(nameof(AreControlsHiddenByClick));
            NotifyPropertyChanged(nameof(AreFullControlsVisible));
        }
    }

    public bool AreFullControlsVisible
    {
        get => !IsMiniMode;
    }

    public bool IsInfoBarAutoHideEnabled
    {
        get => _appearanceConfig.AutoHideInfoBarControl;
    }
    
    public bool IsMenuAutoHideEnabled
    {
        get => _appearanceConfig.AutoHideHamburgerMenu;
    }

    public bool IsMiniMode
    {
        get => _isMiniMode;
        private set
        {
            if (_isMiniMode == value)
                return;

            _isMiniMode = value;
            NotifyPropertyChanged(nameof(IsMiniMode));
            NotifyPropertyChanged(nameof(AreFullControlsVisible));
        }
    }

    public bool IsPlayerAutoHideEnabled
    {
        get => _appearanceConfig.AutoHidePlayerControl;
    }

    #endregion PROPERTIES

    #region CONSTRUCTORS

    public ControlsState(AppearanceConfig appearanceConfig)
    {
        _appearanceConfig = appearanceConfig;
        _appearanceConfig.PropertyChanged += AppearanceConfig_PropertyChanged;
    }

    public void Dispose()
    {
        _appearanceConfig.PropertyChanged -= AppearanceConfig_PropertyChanged;
    }

    #endregion CONSTRUCTORS

    public void SetMiniMode(bool isMiniMode)
    {
        IsMiniMode = isMiniMode;

        if (isMiniMode)
            AreControlsHiddenByClick = false;
    }

    public void ShowControls()
    {
        AreControlsHiddenByClick = false;
    }

    public void ToggleControlsHiddenByClick()
    {
        if (!IsMiniMode)
            AreControlsHiddenByClick = !AreControlsHiddenByClick;
    }

    #region PROPERTY CHANGES

    private void AppearanceConfig_PropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(AppearanceConfig.AutoHideInfoBarControl))
            NotifyPropertyChanged(nameof(IsInfoBarAutoHideEnabled));
        else if (e.PropertyName == nameof(AppearanceConfig.AutoHideHamburgerMenu))
            NotifyPropertyChanged(nameof(IsMenuAutoHideEnabled));
        else if (e.PropertyName == nameof(AppearanceConfig.AutoHidePlayerControl))
            NotifyPropertyChanged(nameof(IsPlayerAutoHideEnabled));
    }

    #endregion PROPERTY CHANGES
}
