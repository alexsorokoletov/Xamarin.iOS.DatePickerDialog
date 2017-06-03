using System;
using System.Linq;
using CoreAnimation;
using CoreGraphics;
using Foundation;
using UIKit;

namespace DT.iOS.DatePickerDialog
{
    public class DatePickerDialog : UIView
    {
        /* Consts */
        private const float kDatePickerDialogDefaultButtonHeight = 50;
        private const float kDatePickerDialogDefaultButtonSpacerHeight = 1;
        private const float kDatePickerDialogCornerRadius = 7;
        private const int kDatePickerDialogCancelButtonTag = 1;
        private const int kDatePickerDialogDoneButtonTag = 2;

        /* Views */
        private UIView _dialogView;
        private UILabel _titleLabel;
        private UIDatePicker _datePicker;
        private UIButton _cancelButton;
        private UIButton _doneButton;

        /* Vars */
        private String _title;
        private String _doneButtonTitle;
        private String _cancelButtonTitle;
        private DateTime _defaultDate;
        private DateTime? _maximumDate;
        private DateTime? _minimumDate;
        private UIDatePickerMode _datePickerMode;
        private Action<DateTime> _callback;


        public DatePickerDialog()
        {
        }

        public void Show(String title, Action<DateTime> callback, DateTime minimumDate, DateTime maximumDate)
        {
            Show(title, doneButtonTitle: "Done", cancelButtonTitle: "Cancel", datePickerMode: UIDatePickerMode.DateAndTime, callback: callback, defaultDate: DateTime.Now, maximumDate: maximumDate, minimumDate: minimumDate);
        }

        public void Show(String title, Action<DateTime> callback, UIDatePickerMode datePickerMode = UIDatePickerMode.DateAndTime)
        {
            Show(title, doneButtonTitle: "Done", cancelButtonTitle: "Cancel", datePickerMode: datePickerMode, callback: callback, defaultDate: DateTime.Now);
        }

        public void Show(string title, string doneButtonTitle, string cancelButtonTitle, UIDatePickerMode datePickerMode, Action<DateTime> callback, DateTime defaultDate, DateTime? maximumDate=null, DateTime? minimumDate= null)
        {

            _title = title;
            _doneButtonTitle = doneButtonTitle;
            _cancelButtonTitle = cancelButtonTitle;
            _datePickerMode = datePickerMode;
            _callback = callback;
            _defaultDate = defaultDate;
            _maximumDate = maximumDate;
            _minimumDate = minimumDate;

            _dialogView = createContainerView();

            _dialogView.Layer.ShouldRasterize = true;
            _dialogView.Layer.RasterizationScale = UIScreen.MainScreen.Scale;

            Layer.ShouldRasterize = true;
            Layer.RasterizationScale = UIScreen.MainScreen.Scale;

            _dialogView.Layer.Opacity = 0.5f;
            _dialogView.Layer.Transform = CATransform3D.MakeScale(1.3f, 1.3f, 1);

            BackgroundColor = UIColor.FromRGBA(0, 0, 0, 0);

            AddSubview(_dialogView);

            /* Attached to the top most window (make sure we are using the right orientation) */
            var interfaceOrientation = UIApplication.SharedApplication.StatusBarOrientation;

            switch (interfaceOrientation)
            {
                case UIInterfaceOrientation.LandscapeLeft:
                    var rotationLeft = Math.PI * 270 / 180;
                    Transform = CGAffineTransform.MakeRotation((nfloat)rotationLeft);
                    break;

                case UIInterfaceOrientation.LandscapeRight:
                    var rotationRight = Math.PI * 90 / 180;
                    Transform = CGAffineTransform.MakeRotation((nfloat)rotationRight);
                    break;

                case UIInterfaceOrientation.PortraitUpsideDown:
                    var rotationUpsideDown = Math.PI * 180 / 180;
                    Transform = CGAffineTransform.MakeRotation((nfloat)rotationUpsideDown);
                    break;
                default:
                    break;
            }

            Frame = new CGRect(0, 0, Frame.Width, Frame.Size.Height);
            UIApplication.SharedApplication.Windows[0].AddSubview(this);

            UIView.Animate(0.2, 0d, UIViewAnimationOptions.CurveEaseInOut, () =>
                {
                    BackgroundColor = UIColor.Black.ColorWithAlpha(0.4f);
                    _dialogView.Layer.Opacity = 1;
                    _dialogView.Layer.Transform = CATransform3D.MakeScale(1, 1, 1);
                }, () =>
                {
                });

        }

        /* Helper function: count and return the screen's size */
        private CGSize CountScreenSize()
        {
            var screenWidth = UIScreen.MainScreen.Bounds.Size.Width;
            var screenHeight = UIScreen.MainScreen.Bounds.Size.Height;
            var interfaceOrientaion = UIApplication.SharedApplication.StatusBarOrientation;
            if (UIInterfaceOrientation.LandscapeLeft == interfaceOrientaion || UIInterfaceOrientation.LandscapeRight == interfaceOrientaion)
            {
                return new CGSize(screenHeight, screenWidth);
            }
            else
            {
                return new CGSize(screenWidth, screenHeight);
            }
        }

        /* Creates the container view here: create the dialog, then add the custom content and buttons */
        private UIView createContainerView()
        {
            var screenSize = CountScreenSize();
            var dialogSize = new CGSize(
                                 300,
                                 230
                                 + kDatePickerDialogDefaultButtonHeight
                                 + kDatePickerDialogDefaultButtonSpacerHeight);

            // For the black background
            Frame = new CGRect(0, 0, screenSize.Width, screenSize.Height);

            // This is the dialog's container; we attach the custom content and the buttons to this one
            var dialogContainer = new UIView(new CGRect((screenSize.Width - dialogSize.Width) / 2, (screenSize.Height - dialogSize.Height) / 2, dialogSize.Width, dialogSize.Height));

            // First, we style the dialog to match the iOS8 UIAlertView >>>
            var gradient = new CAGradientLayer();

            gradient.Frame = dialogContainer.Bounds;
            gradient.Colors = new[]
            {
                UIColor.FromRGB(218, 218, 218).CGColor,
                UIColor.FromRGB(233, 233, 233).CGColor,
                UIColor.FromRGB(218, 218, 218).CGColor
            };

            var cornerRadius = kDatePickerDialogCornerRadius;
            gradient.CornerRadius = cornerRadius;
            dialogContainer.Layer.InsertSublayer(gradient, 0);

            dialogContainer.Layer.CornerRadius = cornerRadius;
            dialogContainer.Layer.BorderColor = UIColor.FromRGB(198, 198, 198).CGColor;
            dialogContainer.Layer.BorderWidth = 1;
            dialogContainer.Layer.ShadowRadius = cornerRadius + 5;
            dialogContainer.Layer.ShadowOpacity = 0.1f;
            dialogContainer.Layer.ShadowOffset = new CGSize(0 - (cornerRadius + 5) / 2, 0 - (cornerRadius + 5) / 2);
            dialogContainer.Layer.ShadowColor = UIColor.Black.CGColor;
            dialogContainer.Layer.ShadowPath = UIBezierPath.FromRoundedRect(dialogContainer.Bounds, dialogContainer.Layer.CornerRadius).CGPath;

            // There is a line above the button
            var lineView = new UIView(new CGRect(0, dialogContainer.Bounds.Size.Height - kDatePickerDialogDefaultButtonHeight - kDatePickerDialogDefaultButtonSpacerHeight, dialogContainer.Bounds.Size.Width, kDatePickerDialogDefaultButtonSpacerHeight));
            lineView.BackgroundColor = UIColor.FromRGB(198, 198, 198);
            dialogContainer.AddSubview(lineView);
            // ˆˆˆ
            //Title
            _titleLabel = new UILabel(new CGRect(10, 10, 280, 30));
            _titleLabel.TextAlignment = UITextAlignment.Center;
            _titleLabel.Font = UIFont.BoldSystemFontOfSize(17);
            _titleLabel.Text = _title;
            dialogContainer.AddSubview(_titleLabel);


            _datePicker = new UIDatePicker(new CGRect(0, 30, 0, 0));
            _datePicker.AutoresizingMask = UIViewAutoresizing.FlexibleRightMargin;
            _datePicker.Frame = new CGRect(_datePicker.Frame.Location, new CGSize(300, _datePicker.Frame.Size.Height));
            _datePicker.Mode = _datePickerMode;
            _datePicker.Date = (NSDate)_defaultDate;

            if (_maximumDate.HasValue)
                _datePicker.MaximumDate = (NSDate)_maximumDate.Value;

            if (_minimumDate.HasValue)
                _datePicker.MinimumDate = (NSDate)_minimumDate.Value;

            dialogContainer.AddSubview(_datePicker);
            AddButtonsToView(dialogContainer);
            return dialogContainer;
        }

        private void ButtonTapped(object sender, EventArgs e)
        {
            var button = sender as UIButton;
            if (button?.Tag == kDatePickerDialogDoneButtonTag)
            {
                var utcDate = (DateTime)_datePicker.Date;
                var local = new DateTime(utcDate.Ticks, DateTimeKind.Utc).ToLocalTime();

                _callback?.Invoke(local);
            }
            else if (button?.Tag == kDatePickerDialogCancelButtonTag)
            {
                //There's nothing do to here \o\
            }
            Close();
        }

        /* Add buttons to container */
        private void AddButtonsToView(UIView container)
        {
            var buttonWidth = container.Bounds.Size.Width / 2;

            _cancelButton = new UIButton(UIButtonType.Custom);
            _cancelButton.Frame = new CGRect(
                0,
                container.Bounds.Size.Height - kDatePickerDialogDefaultButtonHeight,
                buttonWidth,
                kDatePickerDialogDefaultButtonHeight
            );
            _cancelButton.Tag = kDatePickerDialogCancelButtonTag;
            _cancelButton.SetTitle(_cancelButtonTitle, UIControlState.Normal);
            _cancelButton.SetTitleColor(UIColor.FromRGB(0f, 0.5f, 1f), UIControlState.Normal);
            _cancelButton.SetTitleColor(UIColor.FromRGBA(0.2f, 0.2f, 0.2f, 0.5f), UIControlState.Highlighted);
            _cancelButton.TitleLabel.Font = UIFont.BoldSystemFontOfSize(14);
            _cancelButton.Layer.CornerRadius = kDatePickerDialogCornerRadius;
            _cancelButton.TouchUpInside += ButtonTapped;
            container.AddSubview(_cancelButton);

            _doneButton = new UIButton(UIButtonType.Custom);
            _doneButton.Frame = new CGRect(
                buttonWidth,
                container.Bounds.Size.Height - kDatePickerDialogDefaultButtonHeight,
                buttonWidth,
                kDatePickerDialogDefaultButtonHeight
            );
            _doneButton.Tag = kDatePickerDialogDoneButtonTag;
            _doneButton.SetTitle(_doneButtonTitle, forState: UIControlState.Normal);
            _doneButton.SetTitleColor(UIColor.FromRGB(0f, 0.5f, 1f), UIControlState.Normal);
            _doneButton.SetTitleColor(UIColor.FromRGBA(0.2f, 0.2f, 0.2f, 0.5f), UIControlState.Highlighted);
            _doneButton.TitleLabel.Font = UIFont.BoldSystemFontOfSize(14);
            _doneButton.Layer.CornerRadius = kDatePickerDialogCornerRadius;
            _doneButton.TouchUpInside += ButtonTapped;
            container.AddSubview(_doneButton);
        }


        /* Dialog close animation then cleaning and removing the view from the parent */
        private void Close()
        {
            var currentTransform = _dialogView.Layer.Transform;
            var startRotation = (this.ValueForKeyPath(new NSString("layer.transform.rotation.z")) as NSNumber);
            var startRotationAngle = startRotation.DoubleValue;
            var rotation = CATransform3D.MakeRotation((nfloat)(-startRotationAngle + Math.PI * 270 / 180d), 0f, 0f, 0f);
            _dialogView.Layer.Transform = rotation.Concat(CATransform3D.MakeScale(1, 1, 1));
            _dialogView.Layer.Opacity = 1;
            UIView.Animate(0.2, 0d, UIViewAnimationOptions.TransitionNone,
                () =>
                {
                    BackgroundColor = UIColor.FromRGBA(0, 0, 0, 0);
                    _dialogView.Layer.Transform = currentTransform.Concat(CATransform3D.MakeScale(0.6f, 0.6f, 1f));
                    _dialogView.Layer.Opacity = 0;
                },
                () =>
                {
                    var subViews = this.Subviews.ToArray();
                    foreach (UIView v in subViews)
                    {
                        v.RemoveFromSuperview();
                    }
                });
            this.RemoveFromSuperview();
        }

    }
}

