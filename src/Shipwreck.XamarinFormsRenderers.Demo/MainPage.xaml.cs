using Xamarin.Forms;

namespace Shipwreck.XamarinFormsRenderers.Demo;

public partial class MainPage : ContentPage
{
    public MainPage()
    {
        InitializeComponent();
        BindingContext = new MainPageViewModel(this);
    }
}
