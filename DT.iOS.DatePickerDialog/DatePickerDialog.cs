using System;
using System.Linq;
using CoreAnimation;
using CoreGraphics;
using Foundation;
using UIKit;

#nullable enable

namespace DT.iOS.DatePickerDialog
{
    public class DatePickerDialog : UIView
    {
        /* Consts */
        private const string kDone = "Done";
        private const string kCancel = "Cancel";
        private const float kDefaultButtonHeight = 50;
        private const float kDefaultButtonSpacerHeight = 1;
        private const float kCornerRadius = 7;
        private const int kDoneButtonTag = 2;

        /* Views */
        private UIView? _dialogView;
        private UILabel? _titleLabel;
        private UIDatePicker? _datePicker;
        private UIButton? _cancelButton;
        private UIButton? _doneButton;

        /* Vars */
        private DateTime _defaultDate;
        private UIDatePickerMode _datePickerMode;
        private Action<DateTime>? _callback;
        private Action? _cancelCallback;
        private bool _showCancelButton = false;
        private readonly bool _useLocalizedButtons;
        private NSLocale? _locale;
        private UIColor _textColor;
        private UIColor _buttonColor;
        private UIFont _font;
        private CAGradientLayer _gradient = new CAGradientLayer();
        private NSObject? _orientationChangeObserver;

        public DatePickerDialog(UIColor? textColor = null, UIColor? buttonColor = null, UIFont? font = null, NSLocale? locale = null,
            bool showCancelButton = true, bool useLocalizedButtons = false)
        {
            _textColor = textColor ?? Colors.Text();
            _buttonColor = buttonColor ?? Colors.Accent();
            _font = font ?? UIFont.BoldSystemFontOfSize(15);
            _showCancelButton = showCancelButton;
            _useLocalizedButtons = useLocalizedButtons;
            _locale = locale;
            SetupView();
        }

        public void Show(string title, Action<DateTime> callback, DateTime minimumDate, DateTime maximumDate, Action? cancelCallback = null, Func<UIView>? getCurrentView = null)
            => Show(title, kDone, kCancel, UIDatePickerMode.DateAndTime, callback, DateTime.Now, maximumDate, minimumDate, cancelCallback, getCurrentView);

        public void Show(string title, Action<DateTime> callback, UIDatePickerMode datePickerMode = UIDatePickerMode.DateAndTime, Action? cancelCallback = null, Func<UIView>? getCurrentView = null)
            => Show(title, kDone, kCancel, datePickerMode, callback, DateTime.Now, null, null, cancelCallback, getCurrentView);

        public void Show(string title, UIDatePickerMode datePickerMode, Action<DateTime> callback, DateTime defaultDate, DateTime? maximumDate = null, DateTime? minimumDate = null, Action? cancelCallback = null, Func<UIView>? getCurrentView = null)
            => Show(title, kDone, kCancel, datePickerMode, callback, defaultDate, maximumDate, minimumDate, cancelCallback, getCurrentView);

        public void Show(string title, string doneButtonTitle, string cancelButtonTitle,
            UIDatePickerMode datePickerMode,
            Action<DateTime> callback,
            DateTime defaultDate,
            DateTime? maximumDate = null, DateTime? minimumDate = null,
            Action? cancelCallback = null,
            Func<UIView>? getCurrentView = null)
        {

            _titleLabel!.Text = title;

            if (!_useLocalizedButtons)
            {
                _doneButton!.SetTitle(doneButtonTitle, UIControlState.Normal);
                if (_showCancelButton)
                {
                    _cancelButton!.SetTitle(cancelButtonTitle, UIControlState.Normal);
                }
            }

            _datePickerMode = datePickerMode;
            _callback = callback;
            _cancelCallback = cancelCallback;
            _defaultDate = defaultDate;
            _datePicker!.Mode = _datePickerMode;
            _datePicker.Date = (NSDate)_defaultDate;
            if (maximumDate != null)
            {
                _datePicker.MaximumDate = (NSDate)maximumDate.Value;
            }
            if (minimumDate != null)
            {
                _datePicker.MinimumDate = (NSDate)minimumDate.Value;
            }
            if (_locale != null)
            {
                _datePicker.Locale = _locale;
            }
            if (UIDevice.CurrentDevice.CheckSystemVersion(13, 4))
            {
                _datePicker.PreferredDatePickerStyle = UIDatePickerStyle.Wheels;
            }

            var window = getCurrentView == null
                ? UIApplication.SharedApplication.Windows.Last()
                : getCurrentView.Invoke();

            window.AddSubview(this);
            window.BringSubviewToFront(this);
            window.EndEditing(true);

            _orientationChangeObserver = UIDevice.Notifications.ObserveOrientationDidChange(this.DeviceOrientationDidChange);
            AnimateNotify(0.2, 0d, UIViewAnimationOptions.CurveEaseInOut, () =>
            {
                BackgroundColor = UIColor.Black.ColorWithAlpha(0.4f);
                _dialogView!.Layer.Opacity = 1;
                _dialogView!.Layer.Transform = CATransform3D.MakeScale(1, 1, 1);
            }, (_) => { });

        }

        private CGSize CountScreenSize()
        {
            var screenWidth = UIScreen.MainScreen.Bounds.Size.Width;
            var screenHeight = UIScreen.MainScreen.Bounds.Size.Height;
            return new CGSize(screenWidth, screenHeight);
        }

        private void SetupView()
        {
            _dialogView = CreateContainerView();
            Layer.ShouldRasterize = _dialogView.Layer.ShouldRasterize = true;
            Layer.RasterizationScale = _dialogView.Layer.RasterizationScale = UIScreen.MainScreen.Scale;

            _dialogView.Layer.Opacity = 0.5f;
            _dialogView.Layer.Transform = CATransform3D.MakeScale(1.3f, 1.3f, 1f);
            BackgroundColor = UIColor.FromRGBA(0, 0, 0, 0);
            AddSubview(_dialogView);
        }

        private UIView CreateContainerView()
        {
            var screenSize = CountScreenSize();
            var dialogSize = new CGSize(
                                 300,
                                 230
                                 + kDefaultButtonHeight
                                 + kDefaultButtonSpacerHeight);

            // For the black background
            var maxSize = Math.Max(screenSize.Width, screenSize.Height);
            Frame = new CGRect(0, 0, maxSize, maxSize);

            // This is the dialog's container; we attach the custom content and the buttons to this one
            var dialogContainer = new UIView(new CGRect(CGPoint.Empty, dialogSize));
            dialogContainer.Center = new CGPoint(screenSize.Width / 2, screenSize.Height / 2);
            _gradient.Frame = dialogContainer.Bounds;
            _gradient.Colors = Colors.GradientBackground();
            var cornerRadius = kCornerRadius;
            _gradient.CornerRadius = cornerRadius;
            dialogContainer.Layer.InsertSublayer(_gradient, 0);

            dialogContainer.Layer.CornerRadius = cornerRadius;
            dialogContainer.Layer.BorderColor = Colors.Separator().CGColor;
            dialogContainer.Layer.BorderWidth = 1;
            dialogContainer.Layer.ShadowRadius = cornerRadius + 5;
            dialogContainer.Layer.ShadowOpacity = 0.1f;
            dialogContainer.Layer.ShadowOffset = new CGSize(0 - (cornerRadius + 5) / 2, 0 - (cornerRadius + 5) / 2);
            dialogContainer.Layer.ShadowColor = UIColor.Black.CGColor;
            dialogContainer.Layer.ShadowPath = UIBezierPath.FromRoundedRect(dialogContainer.Bounds, dialogContainer.Layer.CornerRadius).CGPath;

            // There is a line above the button
            var lineView = new UIView(new CGRect(0, dialogContainer.Bounds.Size.Height - kDefaultButtonHeight - kDefaultButtonSpacerHeight, dialogContainer.Bounds.Size.Width, kDefaultButtonSpacerHeight))
            {
                BackgroundColor = Colors.Separator()
            };
            dialogContainer.AddSubview(lineView);

            //Title
            _titleLabel = new UILabel(new CGRect(10, 10, 280, 30))
            {
                TextAlignment = UITextAlignment.Center,
                TextColor = _textColor,
                Font = _font.WithSize(17)
            };
            dialogContainer.AddSubview(_titleLabel);
            _datePicker = ConfigureDatePicker();
            dialogContainer.AddSubview(_datePicker);
            AddButtonsToView(dialogContainer);
            return dialogContainer;
        }

        private UIDatePicker ConfigureDatePicker()
        {
            var picker = new UIDatePicker(new CGRect(0, 30, 0, 0));
            picker.SetValueForKey(this._textColor, (NSString)"textColor");
            picker.AutoresizingMask = UIViewAutoresizing.FlexibleRightMargin;
            picker.Frame = new CGRect(picker.Frame.Location, new CGSize(300, 216));
            return picker;
        }

        private void ButtonTapped(object sender, EventArgs e)
        {
            nint tag = 0;
            if (sender is UIButton uiButton)
                tag = uiButton.Tag;
            else if (sender is UIBarButtonItem barButton)
                tag = barButton.Tag;

            if (tag == kDoneButtonTag)
            {
                var utcDate = (DateTime)_datePicker!.Date;
                var local = new DateTime(utcDate.Ticks, DateTimeKind.Utc).ToLocalTime();
                _callback?.Invoke(local);
            }
            else
            {
                _cancelCallback?.Invoke();
            }
            Close();
        }

        private void AddButtonsToView(UIView container)
        {
            if(_useLocalizedButtons)
            {
                AddBarButtonsToView(container);
                return;
            }

            var buttonWidth = container.Bounds.Size.Width / 2;

            var leftButtonFrame = new CGRect(
                0,
                container.Bounds.Size.Height - kDefaultButtonHeight,
                buttonWidth,
                kDefaultButtonHeight);

            var rightButtonFrame = new CGRect(
                buttonWidth,
                container.Bounds.Size.Height - kDefaultButtonHeight,
                buttonWidth,
                kDefaultButtonHeight);

            if (!_showCancelButton)
            {
                buttonWidth = container.Bounds.Size.Width;
                leftButtonFrame = CGRect.Empty;
                rightButtonFrame = new CGRect(
                    0,
                    container.Bounds.Size.Height - kDefaultButtonHeight,
                    buttonWidth,
                    kDefaultButtonHeight);

            }

            var interfaceLayoutDirection = UIApplication.SharedApplication.UserInterfaceLayoutDirection;
            var isLTR = interfaceLayoutDirection == UIUserInterfaceLayoutDirection.LeftToRight;

            if (_showCancelButton)
            {
                _cancelButton =  new UIButton(UIButtonType.System)
                {
                    Frame = isLTR ? leftButtonFrame : rightButtonFrame
                };
                _cancelButton.SetTitleColor(_buttonColor, UIControlState.Normal);
                _cancelButton.SetTitleColor(_buttonColor, UIControlState.Highlighted);
                if (_cancelButton.TitleLabel != null)
                {
                    _cancelButton.TitleLabel.Font = _font.WithSize(14);
                }
                _cancelButton.Layer.CornerRadius = kCornerRadius;
                _cancelButton.TouchUpInside += ButtonTapped;
                container.AddSubview(_cancelButton);
            }
            _doneButton = new UIButton(UIButtonType.System)
            {
                Frame = isLTR ? rightButtonFrame : leftButtonFrame,
                Tag = kDoneButtonTag
            };
            _doneButton.SetTitleColor(_buttonColor, UIControlState.Normal);
            _doneButton.SetTitleColor(_buttonColor, UIControlState.Highlighted);
            if (_doneButton.TitleLabel != null)
            {
                _doneButton.TitleLabel.Font = _font.WithSize(14);
            }
            _doneButton.Layer.CornerRadius = kCornerRadius;
            _doneButton.TouchUpInside += ButtonTapped;
            container.AddSubview(_doneButton);
        }


        private void AddBarButtonsToView(UIView container)
        {
            var width = container.Bounds.Size.Width;

            var toolbar = new UIToolbar(new CGRect(
                0,
                container.Bounds.Size.Height - kDefaultButtonHeight,
                width,
                kDefaultButtonHeight))
            {
                Translucent = true,
                Opaque = false,
                BackgroundColor = UIColor.Clear,
                
            };
            toolbar.SetBackgroundImage(new UIImage(), UIToolbarPosition.Any, UIBarMetrics.Default);
            toolbar.Layer.CornerRadius = kCornerRadius;
            toolbar.Layer.MasksToBounds = true;

            var doneButton = new UIBarButtonItem(UIBarButtonSystemItem.Done, ButtonTapped)
            {
                Tag = kDoneButtonTag
            };
            var cancelButton = _showCancelButton ? new UIBarButtonItem(UIBarButtonSystemItem.Cancel, ButtonTapped) : null;
            var spacer = new UIBarButtonItem(UIBarButtonSystemItem.FlexibleSpace);

            var interfaceLayoutDirection = UIApplication.SharedApplication.UserInterfaceLayoutDirection;
            var isLTR = interfaceLayoutDirection == UIUserInterfaceLayoutDirection.LeftToRight;

            var tolbarItems = isLTR
                ? _showCancelButton ? new[] { spacer, cancelButton, spacer, spacer, doneButton, spacer } : new[] { spacer, doneButton, spacer }
                : _showCancelButton ? new[] { spacer, doneButton, spacer, spacer, cancelButton, spacer } : new[] { spacer, doneButton, spacer };

            toolbar.SetItems(tolbarItems, false);

            container.AddSubview(toolbar);
        }

        private void Close()
        {
            var currentTransform = _dialogView!.Layer.Transform;
            var startRotation = (this.ValueForKeyPath(new NSString("layer.transform.rotation.z")) as NSNumber);
            var startRotationAngle = startRotation?.DoubleValue ?? 0d;
            var rotation = CATransform3D.MakeRotation((nfloat)(-startRotationAngle + Math.PI * 270 / 180d), 0f, 0f, 0f);
            _dialogView.Layer.Transform = rotation.Concat(CATransform3D.MakeScale(1, 1, 1));
            _dialogView.Layer.Opacity = 1;
            AnimateNotify(0.2, 0d, UIViewAnimationOptions.TransitionNone,
                () =>
                {
                    BackgroundColor = UIColor.FromRGBA(0, 0, 0, 0);
                    _dialogView.Layer.Transform = currentTransform.Concat(CATransform3D.MakeScale(0.6f, 0.6f, 1f));
                    _dialogView.Layer.Opacity = 0;
                },
                (_) =>
                {
                    Subviews.ToList().ForEach(v => v.RemoveFromSuperview());
                });
            RemoveFromSuperview();
            if (_orientationChangeObserver != null)
            {
                _orientationChangeObserver.Dispose();
            }
            SetupView();
        }

        public override void TraitCollectionDidChange(UITraitCollection? previousTraitCollection)
        {
            base.TraitCollectionDidChange(previousTraitCollection);
            if (_dialogView != null)
            {
                _dialogView.Layer.BorderColor = Colors.Separator().CGColor;
            }
            _gradient.Colors = Colors.GradientBackground();
        }

        private void DeviceOrientationDidChange(object sender, Foundation.NSNotificationEventArgs args)
        {
            BeginInvokeOnMainThread(() =>
            {
                if (_dialogView != null)
                {
                    var screenSize = CountScreenSize();
                    _dialogView.Center = new CGPoint(screenSize.Width / 2, screenSize.Height / 2);
                }
            });

        }
    }
}

