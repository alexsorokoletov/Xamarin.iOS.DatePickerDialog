using DT.iOS.DatePickerDialog;
using System;
using UIKit;
using Xamarin.Essentials;

namespace TestDialogApp
{
    public partial class ViewController : UIViewController
    {
        public ViewController(IntPtr handle) : base(handle)
        {
        }

        partial void DatePickerClicked(Foundation.NSObject sender)
        {
            var startingTime = DateTime.Now;
            var dialog = new DatePickerDialog(useLocalizedButtons: true);
            dialog.Show("Choose date", "Done", "Cancel", UIDatePickerMode.Date, (dt) =>
            {
                TimePickerText.Text = $"Value selected: ${dt}";
            },
            startingTime,
            getCurrentView: () => Platform.GetCurrentUIViewController().View);
        }

        partial void DateWithRangeClicked(Foundation.NSObject sender)
        {
            var startingTime = DateTime.Now;
            var dialog = new DatePickerDialog();
            dialog.Show("Choose date this week", (dt) =>
            {
                TimePickerText.Text = $"Value selected: ${dt}";
            }, DateTime.Now, DateTime.Now.AddDays(7));
        }

        partial void TimePickerClicked(Foundation.NSObject sender)
        {
            var startingTime = DateTime.Now;
            var dialog = new DatePickerDialog();
            dialog.Show("Choose time", "Done", "Cancel", UIDatePickerMode.Time, (dt) =>
            {
                TimePickerText.Text = $"Value selected: ${dt}";
            }, startingTime);
        }
    }
}