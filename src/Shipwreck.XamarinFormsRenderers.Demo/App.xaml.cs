using Xamarin.Forms;

namespace Shipwreck.XamarinFormsRenderers.Demo;

public partial class App : Application
{
    public App()
    {
        InitializeComponent();

        MainPage = new FlyoutPage()
        {
            Flyout = new ContentPage { Title = "Dummy" },
            Detail = new NavigationPage(new MainPage())
        };
    }

    protected override void OnStart()
    {
    }

    protected override void OnSleep()
    {
    }

    protected override void OnResume()
    {
    }
}
