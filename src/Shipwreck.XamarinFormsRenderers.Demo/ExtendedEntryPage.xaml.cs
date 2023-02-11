using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Shipwreck.XamarinFormsRenderers.Demo;

[XamlCompilation(XamlCompilationOptions.Compile)]
public partial class ExtendedEntryPage : ContentPage
{
    public ExtendedEntryPage()
    {
        InitializeComponent();
        BindingContext = new ExtendedEntryPageViewModel(this);
    }

    internal void FocusTarget()
        => target.Focus();
}
