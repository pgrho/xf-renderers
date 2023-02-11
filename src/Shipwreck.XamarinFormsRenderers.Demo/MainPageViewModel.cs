using Xamarin.Forms;

namespace Shipwreck.XamarinFormsRenderers.Demo;

public sealed class MainPageViewModel : PageViewModel
{
    public MainPageViewModel(Page page)
        : base(page)
    {
        ExtendedEntryCommand = new Command(() => NavigateAsync(new ExtendedEntryPage()));
    }

    public Command ExtendedEntryCommand { get; }
}
