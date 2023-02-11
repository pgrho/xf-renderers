using System.ComponentModel;
using Android.Content;
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
    }

    protected override void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
    {
        switch (e.PropertyName)
        {
            case nameof(Element.SelectAllOnFocus):
                UpdateSelectAllOnFocus();
                break;
        }

        base.OnElementPropertyChanged(sender, e);
    }

    protected virtual void UpdateSelectAllOnFocus()
    {
        if (Element is ExtendedEntry e && Control is CustomEditText c)
        {
            c.SetSelectAllOnFocus(e.SelectAllOnFocus);
        }
    }
}
