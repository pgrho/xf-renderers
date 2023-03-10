using System.ComponentModel;
using Android.Content;
using Android.Graphics.Drawables;
using Android.Text;
using Android.Text.Method;
using Android.Util;
using Android.Views;
using Android.Views.InputMethods;
using Android.Widget;
using AndroidX.Core.Content;
using Java.Lang;
using Xamarin.Forms.Platform.Android;
using Xamarin.Forms.PlatformConfiguration.AndroidSpecific;
using Color = Xamarin.Forms.Color;
using Entry = Xamarin.Forms.Entry;
using VisualElement = Xamarin.Forms.VisualElement;

namespace Shipwreck.XamarinFormsRenderers.Android;

public abstract partial class EntryRendererBase<TControl> : ViewRenderer<Entry, TControl>, ITextWatcher, TextView.IOnEditorActionListener
    where TControl : global::Android.Views.View
{
    bool _disposed;
    ImeAction _currentInputImeFlag;
    IElementController ElementController => Element as IElementController;

    bool _cursorPositionChangePending;
    bool _selectionLengthChangePending;
    bool _nativeSelectionIsUpdating;

    protected abstract EditText EditText { get; }

    public EntryRendererBase(Context context) : base(context)
    {
        AutoPackage = false;
    }

    [Obsolete("This constructor is obsolete as of version 2.5. Please use EntryRenderer(Context) instead.")]
    [EditorBrowsable(EditorBrowsableState.Never)]
    internal EntryRendererBase()
    {
        AutoPackage = false;
    }

    bool TextView.IOnEditorActionListener.OnEditorAction(TextView v, ImeAction actionId, KeyEvent e)
        => OnEditorAction(v, actionId, e, _currentInputImeFlag);

    protected virtual bool OnEditorAction(TextView v, ImeAction actionId, KeyEvent e, ImeAction imeAction)
    {
        // Fire Completed and dismiss keyboard for hardware / physical keyboards
        if (actionId == ImeAction.Done || actionId == imeAction || (actionId == ImeAction.ImeNull && e.KeyCode == Keycode.Enter && e.Action == KeyEventActions.Up))
        {
            var nextFocus = SearchNextFocus(v, imeAction);

            if (nextFocus != v)
            {
                if (nextFocus != null)
                {
                    FocusNext(v, nextFocus);
                }
                else
                {
                    EditText.ClearFocus();
                    v.HideKeyboard();
                }
            }

            ((IEntryController)Element).SendCompleted();
        }

        return true;
    }


    protected virtual global::Android.Views.View SearchNextFocus(TextView v, ImeAction imeAction)
    {
        if (imeAction == ImeAction.Next)
        {
            return FocusSearch(v, FocusSearchDirection.Forward);
        }

        return null;
    }
    protected virtual void FocusNext(TextView currentView, global::Android.Views.View nextFocus)
    {
        nextFocus.RequestFocus();
        if (!nextFocus.OnCheckIsTextEditor())
        {
            currentView.HideKeyboard();
        }
    }

    void ITextWatcher.AfterTextChanged(IEditable s)
    {
        OnAfterTextChanged(s);
    }

    void ITextWatcher.BeforeTextChanged(ICharSequence s, int start, int count, int after)
    {
    }

    void ITextWatcher.OnTextChanged(ICharSequence s, int start, int before, int count)
    {
        TextTransformUtilites.SetPlainText(Element, s?.ToString());
    }

    protected override void OnFocusChangeRequested(object sender, VisualElement.FocusRequestArgs e)
    {
        if (!e.Focus)
        {
            EditText.HideKeyboard();
        }

        base.OnFocusChangeRequested(sender, e);

        if (e.Focus)
        {
            // Post this to the main looper queue so it doesn't happen until the other focus stuff has resolved
            // Otherwise, ShowKeyboard will be called before this control is truly focused, and we will potentially
            // be displaying the wrong keyboard
            PostShowKeyboard(EditText);
        }
    }

    #region Keyboard

    protected virtual void ShowKeyboard(EditText editText)
        => editText?.ShowKeyboard();

    protected virtual void PostShowKeyboard(EditText editText)
        => editText?.PostShowKeyboard();

    #endregion Keyboard

    protected override void OnElementChanged(ElementChangedEventArgs<Entry> e)
    {
        base.OnElementChanged(e);

        if (e.OldElement == null)
        {
            SetNativeControl(CreateNativeControl());

            EditText.AddTextChangedListener(this);
            EditText.SetOnEditorActionListener(this);

            // IFormsEditText
            if (EditText is CustomEditText formsEditText)
            {
                formsEditText.OnKeyboardBackPressed += OnKeyboardBackPressed;
                formsEditText.SelectionChanged += SelectionChanged;
            }
        }

        // When we set the control text, it triggers the SelectionChanged event, which updates CursorPosition and SelectionLength;
        // These one-time-use variables will let us initialize a CursorPosition and SelectionLength via ctor/xaml/etc.
        _cursorPositionChangePending = Element.IsSet(Entry.CursorPositionProperty);
        _selectionLengthChangePending = Element.IsSet(Entry.SelectionLengthProperty);

        UpdatePlaceHolderText();
        UpdateText();
        UpdateInputType();
        UpdateColor();
        UpdateCharacterSpacing();
        UpdateHorizontalTextAlignment();
        UpdateVerticalTextAlignment();
        UpdateFont();
        UpdatePlaceholderColor();
        UpdateMaxLength();
        UpdateImeOptions();
        UpdateReturnType();
        UpdateIsReadOnly();

        if (_cursorPositionChangePending || _selectionLengthChangePending)
        {
            UpdateCursorSelection();
        }

        UpdateClearBtnOnElementChanged();
    }

    protected override void Dispose(bool disposing)
    {
        if (_disposed)
        {
            return;
        }

        _disposed = true;

        if (disposing)
        {
            if (EditText != null)
            {
                EditText.RemoveTextChangedListener(this);
                EditText.SetOnEditorActionListener(null);

                // IFormsEditText
                if (EditText is CustomEditText formsEditContext)
                {
                    formsEditContext.OnKeyboardBackPressed -= OnKeyboardBackPressed;
                    formsEditContext.SelectionChanged -= SelectionChanged;

                    ListenForCloseBtnTouch(false);
                }
            }

            _clearBtn = null;
        }

        base.Dispose(disposing);
    }

    protected virtual void UpdatePlaceHolderText()
    {
        if (EditText.Hint == Element.Placeholder)
        {
            return;
        }

        EditText.Hint = Element.Placeholder;
        if (EditText.IsFocused)
        {
            ShowKeyboard(EditText);
        }
    }

    protected override void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
    {
        if (this.IsDisposed())
        {
            return;
        }

        switch (e.PropertyName)
        {
            case nameof(Element.Placeholder):
                UpdatePlaceHolderText();
                break;

            case nameof(Element.Keyboard):
            case nameof(Element.IsPassword):
            case nameof(Element.IsSpellCheckEnabled):
            case nameof(Element.IsTextPredictionEnabled):
                UpdateInputType();
                break;

            case nameof(Element.Text):
            case nameof(Element.TextTransform):
                UpdateText();
                break;

            case nameof(Element.TextColor):
                UpdateColor();
                break;

            case nameof(Element.HorizontalTextAlignment):
            case nameof(Element.FlowDirection):
                UpdateHorizontalTextAlignment();
                break;

            case nameof(Element.VerticalTextAlignment):
                UpdateVerticalTextAlignment();
                break;

            case nameof(Element.CharacterSpacing):
                UpdateCharacterSpacing();
                break;

            case nameof(Element.FontAttributes):
            case nameof(Element.FontFamily):
            case nameof(Element.FontSize):
                UpdateFont();
                break;

            case nameof(Element.PlaceholderColor):
                UpdatePlaceholderColor();
                break;

            case nameof(Element.MaxLength):
                UpdateMaxLength();
                break;

            case nameof(Element.ReturnType):
                UpdateReturnType();
                break;

            case nameof(Element.SelectionLength):
            case nameof(Element.CursorPosition):
                UpdateCursorSelection();
                break;

            case nameof(Element.IsReadOnly):
                UpdateIsReadOnly();
                break;

            case nameof(Element.ClearButtonVisibility):
                UpdateClearBtnOnPropertyChanged();
                break;

            case nameof(Xamarin.Forms.PlatformConfiguration.AndroidSpecific.Entry.ImeOptions):
                UpdateImeOptions();
                break;

            case nameof(Element.IsFocused):
                OnIsFocusedChanged();
                break;
        }

        base.OnElementPropertyChanged(sender, e);
    }

    protected virtual void OnIsFocusedChanged()
    {
        UpdateClearBtnOnFocusChanged(Element.IsFocused);
    }

    protected virtual NumberKeyListener GetDigitsKeyListener(InputTypes inputTypes)
    {
        // Override this in a custom renderer to use a different NumberKeyListener
        // or to filter out input types you don't want to allow
        // (e.g., inputTypes &= ~InputTypes.NumberFlagSigned to disallow the sign)
        return LocalizedDigitsKeyListener.Create(inputTypes);
    }

    protected virtual void UpdateImeOptions()
    {
        if (Element == null || Control == null)
        {
            return;
        }

        var imeOptions = Element.OnThisPlatform().ImeOptions();
        _currentInputImeFlag = imeOptions.ToAndroidImeOptions();
        EditText.ImeOptions = _currentInputImeFlag;
    }

    void UpdateHorizontalTextAlignment()
    {
        EditText.UpdateTextAlignment(Element.HorizontalTextAlignment, Element.VerticalTextAlignment);
    }

    void UpdateVerticalTextAlignment()
    {
        EditText.UpdateTextAlignment(Element.HorizontalTextAlignment, Element.VerticalTextAlignment);
    }

    protected abstract void UpdateColor();
    protected abstract void UpdateTextColor(Color color);

    protected virtual void UpdateFont()
    {
        EditText.Typeface = Element.ToTypeface();
        EditText.SetTextSize(ComplexUnitType.Sp, (float)Element.FontSize);
    }

    void UpdateInputType()
    {
        Entry model = Element;
        var keyboard = model.Keyboard;

        EditText.InputType = keyboard.ToInputType();
        if (!(keyboard is CustomKeyboard))
        {
            if (model.IsSet(InputView.IsSpellCheckEnabledProperty))
            {
                if ((EditText.InputType & InputTypes.TextFlagNoSuggestions) != InputTypes.TextFlagNoSuggestions)
                {
                    if (!model.IsSpellCheckEnabled)
                    {
                        EditText.InputType = EditText.InputType | InputTypes.TextFlagNoSuggestions;
                    }
                }
            }
            if (model.IsSet(Entry.IsTextPredictionEnabledProperty))
            {
                if ((EditText.InputType & InputTypes.TextFlagNoSuggestions) != InputTypes.TextFlagNoSuggestions)
                {
                    if (!model.IsTextPredictionEnabled)
                    {
                        EditText.InputType = EditText.InputType | InputTypes.TextFlagNoSuggestions;
                    }
                }
            }
        }

        if (keyboard == Keyboard.Numeric)
        {
            EditText.KeyListener = GetDigitsKeyListener(EditText.InputType);
        }

        if (model.IsPassword && ((EditText.InputType & InputTypes.ClassText) == InputTypes.ClassText))
        {
            EditText.InputType = EditText.InputType | InputTypes.TextVariationPassword;
        }

        if (model.IsPassword && ((EditText.InputType & InputTypes.ClassNumber) == InputTypes.ClassNumber))
        {
            EditText.InputType = EditText.InputType | InputTypes.NumberVariationPassword;
        }

        UpdateFont();
    }

    abstract protected void UpdatePlaceholderColor();

    void OnKeyboardBackPressed(object sender, EventArgs eventArgs)
    {
        Control?.ClearFocus();
    }

    void UpdateMaxLength()
    {
        var currentFilters = new List<IInputFilter>(EditText?.GetFilters() ?? new IInputFilter[0]);

        for (var i = 0; i < currentFilters.Count; i++)
        {
            if (currentFilters[i] is InputFilterLengthFilter)
            {
                currentFilters.RemoveAt(i);
                break;
            }
        }

        currentFilters.Add(new InputFilterLengthFilter(Element.MaxLength));

        EditText?.SetFilters(currentFilters.ToArray());

        var currentControlText = EditText?.Text;

        if (currentControlText.Length > Element.MaxLength)
        {
            EditText.Text = currentControlText.Substring(0, Element.MaxLength);
        }
    }

    void UpdateCharacterSpacing()
    {
        if (global::Android.OS.Build.VERSION.SdkInt >= global::Android.OS.BuildVersionCodes.Lollipop)
        {
            EditText.LetterSpacing = Element.CharacterSpacing.ToEm();
        }
    }

    void UpdateReturnType()
    {
        if (Control == null || Element == null)
        {
            return;
        }

        EditText.ImeOptions = Element.ReturnType.ToAndroidImeAction();
        _currentInputImeFlag = EditText.ImeOptions;
    }

    void SelectionChanged(object sender, Xamarin.Forms.Platform.Android.SelectionChangedEventArgs e)
    {
        if (_nativeSelectionIsUpdating || Control == null || Element == null)
        {
            return;
        }

        int cursorPosition = Element.CursorPosition;
        int selectionStart = EditText.SelectionStart;

        if (!_cursorPositionChangePending)
        {
            var start = cursorPosition;

            if (selectionStart != start)
            {
                SetCursorPositionFromRenderer(selectionStart);
            }
        }

        if (!_selectionLengthChangePending)
        {
            int elementSelectionLength = System.Math.Min(EditText.Text.Length - cursorPosition, Element.SelectionLength);

            var controlSelectionLength = EditText.SelectionEnd - selectionStart;
            if (controlSelectionLength != elementSelectionLength)
            {
                SetSelectionLengthFromRenderer(controlSelectionLength);
            }
        }
    }

    void UpdateCursorSelection()
    {
        if (_nativeSelectionIsUpdating || Control == null || Element == null || EditText == null)
        {
            return;
        }

        if (!Element.IsReadOnly && EditText.RequestFocus())
        {
            try
            {
                int start = GetSelectionStart();
                int end = GetSelectionEnd(start);

                EditText.SetSelection(start, end);
            }
            catch (System.Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Failed to set Control.Selection from CursorPosition/SelectionLength: {ex}");
            }
            finally
            {
                _cursorPositionChangePending = _selectionLengthChangePending = false;
            }
        }
    }

    int GetSelectionEnd(int start)
    {
        int end = start;
        int selectionLength = Element.SelectionLength;

        if (Element.IsSet(Entry.SelectionLengthProperty))
        {
            end = System.Math.Max(start, System.Math.Min(EditText.Length(), start + selectionLength));
        }

        int newSelectionLength = System.Math.Max(0, end - start);
        if (newSelectionLength != selectionLength)
        {
            SetSelectionLengthFromRenderer(newSelectionLength);
        }

        return end;
    }

    int GetSelectionStart()
    {
        int start = EditText.Length();
        int cursorPosition = Element.CursorPosition;

        if (Element.IsSet(Entry.CursorPositionProperty))
        {
            start = System.Math.Min(EditText.Text.Length, cursorPosition);
        }

        if (start != cursorPosition)
        {
            SetCursorPositionFromRenderer(start);
        }

        return start;
    }

    void SetCursorPositionFromRenderer(int start)
    {
        try
        {
            _nativeSelectionIsUpdating = true;
            ElementController?.SetValueFromRenderer(Entry.CursorPositionProperty, start);
        }
        catch (System.Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Failed to set CursorPosition from renderer: {ex}");
        }
        finally
        {
            _nativeSelectionIsUpdating = false;
        }
    }

    void SetSelectionLengthFromRenderer(int selectionLength)
    {
        try
        {
            _nativeSelectionIsUpdating = true;
            ElementController?.SetValueFromRenderer(Entry.SelectionLengthProperty, selectionLength);
        }
        catch (System.Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Failed to set SelectionLength from renderer: {ex}");
        }
        finally
        {
            _nativeSelectionIsUpdating = false;
        }
    }

    protected virtual void UpdateIsReadOnly()
    {
        bool isReadOnly = !Element.IsReadOnly;

        EditText.FocusableInTouchMode = isReadOnly;
        EditText.Focusable = isReadOnly;
    }

    void UpdateText()
    {
        if (EditText == null || Element == null)
        {
            return;
        }

        var text = Element.UpdateFormsText(Element.Text, Element.TextTransform);

        if (EditText.Text == text)
        {
            return;
        }

        EditText.Text = text;
        if (EditText.IsFocused)
        {
            EditText.SetSelection(EditText.Text.Length);
            ShowKeyboard(EditText);
        }
    }

    Drawable _clearBtn;

    // HACK removed OnNativeFocusChanged due to internal.
    //internal override void OnNativeFocusChanged(bool hasFocus)
    //{
    //    base.OnNativeFocusChanged(hasFocus);
    //    UpdateClearBtnOnFocusChanged(hasFocus);
    //}

    void OnAfterTextChanged(IEditable s)
    {
        if (Control.IsFocused)
        {
            UpdateClearBtnOnTyping();
        }
    }

    void EditTextTouched(object sender, TouchEventArgs e)
    {
        e.Handled = false;
        var me = e.Event;

        var rBounds = _clearBtn?.Bounds;
        if (rBounds != null)
        {
            var x = me.GetX();
            var y = me.GetY();
            if (me.Action == MotionEventActions.Up
                && ((x >= (EditText.Right - rBounds.Width())
                && x <= (EditText.Right - EditText.PaddingRight)
                && y >= EditText.PaddingTop
                && y <= (EditText.Height - EditText.PaddingBottom)
                && (Element as IVisualElementController).EffectiveFlowDirection.IsLeftToRight())
                || (x >= (EditText.Left + EditText.PaddingLeft)
                && x <= (EditText.Left + rBounds.Width())
                && y >= EditText.PaddingTop
                && y <= (EditText.Height - EditText.PaddingBottom)
                && (Element as IVisualElementController).EffectiveFlowDirection.IsRightToLeft())))
            {
                EditText.Text = null;
                e.Handled = true;
            }
        }
    }

    void UpdateClearBtnOnElementChanged()
    {
        bool showClearBtn = Element.ClearButtonVisibility == ClearButtonVisibility.WhileEditing;
        if (showClearBtn && Element.IsFocused)
        {
            UpdateClearBtn(true);
            ListenForCloseBtnTouch(true);
        }
    }

    void UpdateClearBtnOnPropertyChanged()
    {
        bool isFocused = Element.IsFocused;
        if (isFocused)
        {
            bool showClearBtn = Element.ClearButtonVisibility == ClearButtonVisibility.WhileEditing;
            UpdateClearBtn(showClearBtn);
            ListenForCloseBtnTouch(showClearBtn);
        }
    }

    void UpdateClearBtnOnFocusChanged(bool isFocused)
    {
        if (Element.ClearButtonVisibility == ClearButtonVisibility.WhileEditing)
        {
            UpdateClearBtn(isFocused);
            ListenForCloseBtnTouch(isFocused);
        }
    }

    void UpdateClearBtnOnTyping()
    {
        if (Element.ClearButtonVisibility == ClearButtonVisibility.WhileEditing)
        {
            UpdateClearBtn(true);
        }
    }

    void UpdateClearBtn(bool showClearButton)
    {
        Drawable d = showClearButton && (Element.Text?.Length > 0) ? GetCloseButtonDrawable() : null;

        if (!Element.TextColor.IsDefault)
        {
#pragma warning disable CS0618 // 型またはメンバーが旧型式です
            d?.SetColorFilter(Element.TextColor.ToAndroid(), global::Android.Graphics.PorterDuff.Mode.SrcIn);
        }
#pragma warning restore CS0618 // 型またはメンバーが旧型式です
        else
        {
            d?.ClearColorFilter();
        }

        if ((Element as IVisualElementController).EffectiveFlowDirection.IsRightToLeft())
        {
            EditText.SetCompoundDrawablesWithIntrinsicBounds(d, null, null, null);
        }
        else
        {
            EditText.SetCompoundDrawablesWithIntrinsicBounds(null, null, d, null);
        }

        _clearBtn = d;
    }

    protected virtual Drawable GetCloseButtonDrawable()
        => ContextCompat.GetDrawable(Context, Resource.Drawable.abc_ic_clear_material);

    void ListenForCloseBtnTouch(bool listen)
    {
        if (listen)
        {
            EditText.Touch += EditTextTouched;
        }
        else
        {
            EditText.Touch -= EditTextTouched;
        }
    }
}
