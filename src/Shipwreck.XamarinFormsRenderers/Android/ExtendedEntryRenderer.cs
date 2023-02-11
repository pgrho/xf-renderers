using System.ComponentModel;
using Android.Content;
using Android.Widget;
using Shipwreck.XamarinFormsRenderers;
using Shipwreck.XamarinFormsRenderers.Android;

[assembly: ExportRenderer(typeof(ExtendedEntry), typeof(ExtendedEntryRenderer))]
namespace Shipwreck.XamarinFormsRenderers.Android;

public class ExtendedEntryRenderer : EntryRenderer
{
    public ExtendedEntryRenderer(Context context)
        : base(context)
    {
    }

    public new ExtendedEntry Element => (ExtendedEntry)base.Element;

    protected override void OnElementChanged(Xamarin.Forms.Platform.Android.ElementChangedEventArgs<Entry> e)
    {
        base.OnElementChanged(e);

        UpdateSelectAllOnFocus();
        UpdateIsSoftwareKeyboardEnabled();
    }

    protected override void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
    {
        switch (e.PropertyName)
        {
            case nameof(Element.SelectAllOnFocus):
                UpdateSelectAllOnFocus();
                break;

            case nameof(Element.IsSoftwareKeyboardEnabled):
                UpdateIsSoftwareKeyboardEnabled();
                break;
        }

        base.OnElementPropertyChanged(sender, e);
    }

    protected override void OnIsFocusedChanged()
    {
        base.OnIsFocusedChanged();

        if (Element.IsFocused && !Element.IsSoftwareKeyboardEnabled)
        {
            EditText?.HideKeyboard();
        }
    }

    protected virtual void UpdateSelectAllOnFocus()
    {
        if (Element is ExtendedEntry e && Control is CustomEditText c)
        {
            c.SetSelectAllOnFocus(e.SelectAllOnFocus);
        }
    }

    protected virtual void UpdateIsSoftwareKeyboardEnabled()
    {
        if (Element is ExtendedEntry e && Control is CustomEditText c)
        {
            c.ShowSoftInputOnFocus = e.IsSoftwareKeyboardEnabled;

            if (c.IsFocused)
            {
                if (e.IsSoftwareKeyboardEnabled)
                {
                    c.ShowKeyboard();
                }
                else
                {
                    c.HideKeyboard();
                }
            }
        }
    }

    #region Keyboard

    protected override void ShowKeyboard(EditText editText)
    {
        if (Element.IsSoftwareKeyboardEnabled)
        {
            base.ShowKeyboard(editText);
        }
    }

    protected override void PostShowKeyboard(EditText editText)
    {
        if (Element.IsSoftwareKeyboardEnabled)
        {
            base.PostShowKeyboard(editText);
        }
    }

    #endregion Keyboard
}
