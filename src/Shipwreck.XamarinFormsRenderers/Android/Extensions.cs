using Android.Content.Res;
using Android.Views;

namespace Shipwreck.XamarinFormsRenderers.Android;

internal static class Extensions
{
    internal static IMenuItem FindMenuItemByNameOrIcon(this IMenu menu, string menuName, string iconName)
    {
        if (menu.Size() == 1)
        {
            return menu.GetItem(0);
        }

        for (var i = 0; i < menu.Size(); i++)
        {
            IMenuItem menuItem = menu.GetItem(i);
            if (menuItem.TitleFormatted != null && menuName == menuItem.TitleFormatted.ToString())
            {
                return menuItem;
            }

            if (!string.IsNullOrEmpty(iconName))
            {
                // TODO : search by iconName
            }
        }
        return null;
    }

    internal static DeviceOrientation ToDeviceOrientation(this Orientation orientation)
    {
        switch (orientation)
        {
            case Orientation.Landscape:
                return DeviceOrientation.Landscape;
            case Orientation.Portrait:
                return DeviceOrientation.Portrait;
            default:
                return DeviceOrientation.Other;
        }
    }

    internal static bool IsHorizontal(this Button.ButtonContentLayout layout) =>
        layout.Position == Button.ButtonContentLayout.ImagePosition.Left ||
        layout.Position == Button.ButtonContentLayout.ImagePosition.Right;


    internal static float ToEm(this double pt)
    {
        return (float)pt * 0.0624f; //Coefficient for converting Pt to Em
    }
}
