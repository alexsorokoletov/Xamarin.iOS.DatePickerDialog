using System;
using DT.iOS.DatePickerDialog;
using UIKit;

namespace TestDialogApp
{
    public partial class ViewController : UIViewController
    {
        protected ViewController(IntPtr handle) : base(handle)
        {
            // Note: this .ctor should not contain any initialization logic.
        }

        partial void DatePickerClicked(Foundation.NSObject sender)
        {
            var startingTime = DateTime.Now;
            var dialog = new DatePickerDialog();
            dialog.Show("Choose date", "Done", "Cancel", UIDatePickerMode.Date, (dt) =>
                {
                    DatePickLabel.Text = dt.ToString();
                }, startingTime);
        }

        partial void TimePickerClicked(Foundation.NSObject sender)
        {
            var startingTime = DateTime.Now;
            var dialog = new DatePickerDialog();
            dialog.Show("Choose time", "Done", "Cancel", UIDatePickerMode.Time, (dt) =>
                {
                    TimePickLabel.Text = dt.ToString();
                }, startingTime);
        }

        partial void DatePickerWithDateRange(UIButton sender)
        {
            var startingTime = DateTime.Now;
            var dialog = new DatePickerDialog();
            dialog.Show("Choose time", (dt) =>
                {
                    TimePickLabel.Text = dt.ToString();
            },DateTime.Now.AddDays(10), DateTime.Now.AddDays(-10));
        }
    }
}

