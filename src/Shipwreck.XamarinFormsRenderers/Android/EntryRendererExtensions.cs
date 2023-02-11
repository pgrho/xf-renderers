using Android.Views.InputMethods;
using ImeFlags = Xamarin.Forms.PlatformConfiguration.AndroidSpecific.ImeFlags;

namespace Shipwreck.XamarinFormsRenderers.Android;

internal static class EntryRendererExtensions
{
    internal static ImeAction ToAndroidImeAction(this ReturnType returnType)
    {
        switch (returnType)
        {
            case ReturnType.Go:
                return ImeAction.Go;

            case ReturnType.Next:
                return ImeAction.Next;

            case ReturnType.Send:
                return ImeAction.Send;

            case ReturnType.Search:
                return ImeAction.Search;

            case ReturnType.Done:
                return ImeAction.Done;

            case ReturnType.Default:
                return ImeAction.Done;

            default:
                throw new System.NotImplementedException($"ReturnType {returnType} not supported");
        }
    }

    public static ImeAction ToAndroidImeOptions(this ImeFlags flags)
    {
        switch (flags)
        {
            case ImeFlags.Previous:
                return ImeAction.Previous;

            case ImeFlags.Next:
                return ImeAction.Next;

            case ImeFlags.Search:
                return ImeAction.Search;

            case ImeFlags.Send:
                return ImeAction.Send;

            case ImeFlags.Go:
                return ImeAction.Go;

            case ImeFlags.None:
                return ImeAction.None;

            case ImeFlags.ImeMaskAction:
                return ImeAction.ImeMaskAction;

            case ImeFlags.NoPersonalizedLearning:
                return (ImeAction)ImeFlags.NoPersonalizedLearning;

            case ImeFlags.NoExtractUi:
                return (ImeAction)ImeFlags.NoExtractUi;

            case ImeFlags.NoAccessoryAction:
                return (ImeAction)ImeFlags.NoAccessoryAction;

            case ImeFlags.NoFullscreen:
                return (ImeAction)ImeFlags.NoFullscreen;

            case ImeFlags.Default:
            case ImeFlags.Done:
            default:
                return ImeAction.Done;
        }
    }
}
