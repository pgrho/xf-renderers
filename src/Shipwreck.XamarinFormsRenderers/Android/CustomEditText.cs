using Android.Content;
using Android.Views;
using Xamarin.Forms.Platform.Android;

namespace Shipwreck.XamarinFormsRenderers.Android;

public class CustomEditText : FormsEditTextBase
{
    public CustomEditText(Context context)
        : base(context)
    {
    }

    public override bool OnKeyPreIme(Keycode keyCode, KeyEvent e)
    {
        if (keyCode != Keycode.Back || e.Action != KeyEventActions.Down)
        {
            return base.OnKeyPreIme(keyCode, e);
        }

        this.HideKeyboard();

        OnKeyboardBackPressed?.Invoke(this, EventArgs.Empty);
        return true;
    }

    protected override void OnSelectionChanged(int selStart, int selEnd)
    {
        base.OnSelectionChanged(selStart, selEnd);
        SelectionChanged?.Invoke(this, new(selStart, selEnd));
    }

    internal event EventHandler OnKeyboardBackPressed;

    internal event EventHandler<Xamarin.Forms.Platform.Android.SelectionChangedEventArgs> SelectionChanged;
}
