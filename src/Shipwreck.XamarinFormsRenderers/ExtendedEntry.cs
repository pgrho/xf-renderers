using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shipwreck.XamarinFormsRenderers;

public class ExtendedEntry : Entry
{
    #region SelectAllOnFocus

    public static readonly BindableProperty SelectAllOnFocusProperty
        = BindableProperty.Create(
            nameof(SelectAllOnFocus), typeof(bool), typeof(ExtendedEntry),
            defaultValue: true);

    public bool SelectAllOnFocus
    {
        get => (bool)GetValue(SelectAllOnFocusProperty);
        set => SetValue(SelectAllOnFocusProperty, value);
    }

    #endregion SelectAllOnFocus

}
