using Xamarin.Forms;

namespace Shipwreck.XamarinFormsRenderers.Demo;

public sealed class ExtendedEntryPageViewModel : PageViewModel
{
    public ExtendedEntryPageViewModel(Page page)
        : base(page)
    {
    }
    #region Text

    private string _Text = "abcdefg";

    public string Text
    {
        get => _Text;
        set => SetProperty(ref _Text, value);
    }

    #endregion
    #region Placeholder

    private string _Placeholder = "placeholder";

    public string Placeholder
    {
        get => _Placeholder;
        set => SetProperty(ref _Placeholder, value);
    }

    #endregion

    #region IsSoftwareKeyboardEnabled

    private bool _IsSoftwareKeyboardEnabled = true;

    public bool IsSoftwareKeyboardEnabled
    {
        get => _IsSoftwareKeyboardEnabled;
        set => SetProperty(ref _IsSoftwareKeyboardEnabled, value);
    }

    #endregion

    #region SelectAllOnFocus

    private bool _SelectAllOnFocus;

    public bool SelectAllOnFocus
    {
        get => _SelectAllOnFocus;
        set => SetProperty(ref _SelectAllOnFocus, value);
    }

    #endregion

    private Command _FocusCommand;
    public Command FocusCommand
        => _FocusCommand ??= new(() => (Page as ExtendedEntryPage)?.FocusTarget());


}
