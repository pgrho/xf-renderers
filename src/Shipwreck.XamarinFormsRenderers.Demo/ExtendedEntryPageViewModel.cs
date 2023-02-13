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

    #endregion Text

    #region Placeholder

    private string _Placeholder = "placeholder";

    public string Placeholder
    {
        get => _Placeholder;
        set => SetProperty(ref _Placeholder, value);
    }

    #endregion Placeholder

    #region IsSoftwareKeyboardEnabled

    private bool _IsSoftwareKeyboardEnabled = true;

    public bool IsSoftwareKeyboardEnabled
    {
        get => _IsSoftwareKeyboardEnabled;
        set => SetProperty(ref _IsSoftwareKeyboardEnabled, value);
    }

    #endregion IsSoftwareKeyboardEnabled

    #region SelectAllOnFocus

    private bool _SelectAllOnFocus;

    public bool SelectAllOnFocus
    {
        get => _SelectAllOnFocus;
        set => SetProperty(ref _SelectAllOnFocus, value);
    }

    #endregion SelectAllOnFocus

    #region ReturnType

    private ReturnType _ReturnType;

    public ReturnType ReturnType
    {
        get => _ReturnType;
        set => SetProperty(ref _ReturnType, value);
    }

    public ReturnType[] ReturnTypes { get; } = { ReturnType.Default, ReturnType.Done, ReturnType.Go, ReturnType.Next, ReturnType.Search, ReturnType.Send };

    #endregion SelectAllOnFocus

    private Command _FocusCommand;

    public Command FocusCommand
        => _FocusCommand ??= new(() => (Page as ExtendedEntryPage)?.FocusTarget());
}
