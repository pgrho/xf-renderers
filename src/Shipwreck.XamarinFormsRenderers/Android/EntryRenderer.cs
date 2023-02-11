using System.ComponentModel;
using Android.Content;
using Android.Widget;
using Color = Xamarin.Forms.Color;

namespace Shipwreck.XamarinFormsRenderers.Android;

public class EntryRenderer : EntryRendererBase<CustomEditText>
{
    TextColorSwitcher _hintColorSwitcher;
    TextColorSwitcher _textColorSwitcher;

    public EntryRenderer(Context context) : base(context)
    {
    }

    [Obsolete("This constructor is obsolete as of version 2.5. Please use EntryRenderer(Context) instead.")]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public EntryRenderer()
    {
        AutoPackage = false;
    }

    protected override CustomEditText CreateNativeControl()
    {
        return new CustomEditText(Context);
    }

    protected override EditText EditText => Control;

    protected override void UpdateIsReadOnly()
    {
        base.UpdateIsReadOnly();
        bool isReadOnly = !Element.IsReadOnly;
        EditText.SetCursorVisible(isReadOnly);
    }

    protected override void UpdatePlaceholderColor()
    {
        _hintColorSwitcher = _hintColorSwitcher ?? new TextColorSwitcher(EditText.HintTextColors, Element.UseLegacyColorManagement());
        _hintColorSwitcher.UpdateTextColor(EditText, Element.PlaceholderColor, EditText.SetHintTextColor);
    }

    protected override void UpdateColor()
    {
        UpdateTextColor(Element.TextColor);
    }

    protected override void UpdateTextColor(Color color)
    {
        _textColorSwitcher = _textColorSwitcher ?? new TextColorSwitcher(EditText.TextColors, Element.UseLegacyColorManagement());
        _textColorSwitcher.UpdateTextColor(EditText, color);
    }
}
