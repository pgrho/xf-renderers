using System.Threading.Tasks;
using Xamarin.Forms;

namespace Shipwreck.XamarinFormsRenderers.Demo;

public abstract class PageViewModel : ObservableObject
{
    protected PageViewModel(Page page)
    {
        Page = page;
    }

    public Page Page { get; }

    public Task NavigateAsync(Page newPage)
        => Page.Navigation.PushAsync(newPage);
}
