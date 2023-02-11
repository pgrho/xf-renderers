namespace Shipwreck.XamarinFormsRenderers;

public class ExtendedEntry : Entry
{
    #region SelectAllOnFocus

    public static readonly BindableProperty SelectAllOnFocusProperty
        = BindableProperty.Create(
            nameof(SelectAllOnFocus), typeof(bool), typeof(ExtendedEntry),
            defaultValue: false);

    public bool SelectAllOnFocus
    {
        get => (bool)GetValue(SelectAllOnFocusProperty);
        set => SetValue(SelectAllOnFocusProperty, value);
    }

    #endregion SelectAllOnFocus

    #region IsSoftwareKeyboardEnabled

    public static readonly BindableProperty IsSoftwareKeyboardEnabledProperty
        = BindableProperty.Create(
            nameof(IsSoftwareKeyboardEnabled), typeof(bool), typeof(ExtendedEntry),
            defaultValue: true);

    public bool IsSoftwareKeyboardEnabled
    {
        get => (bool)GetValue(IsSoftwareKeyboardEnabledProperty);
        set => SetValue(IsSoftwareKeyboardEnabledProperty, value);
    }

    #endregion IsSoftwareKeyboardEnabled
}
