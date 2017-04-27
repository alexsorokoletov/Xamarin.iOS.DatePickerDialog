// WARNING
//
// This file has been generated automatically by Xamarin Studio from the outlets and
// actions declared in your storyboard file.
// Manual changes to this file will not be maintained.
//
using Foundation;
using System;
using System.CodeDom.Compiler;

namespace TestDialogApp
{
    [Register ("ViewController")]
    partial class ViewController
    {
        [Outlet]
        UIKit.UILabel DatePickLabel { get; set; }


        [Outlet]
        UIKit.UILabel TimePickLabel { get; set; }


        [Action ("DatePickerClicked:")]
        partial void DatePickerClicked (Foundation.NSObject sender);


        [Action ("TimePickerClicked:")]
        partial void TimePickerClicked (Foundation.NSObject sender);

        [Action ("DatePickerWithDateRange:")]
        [GeneratedCode ("iOS Designer", "1.0")]
        partial void DatePickerWithDateRange (UIKit.UIButton sender);

        void ReleaseDesignerOutlets ()
        {
            if (DatePickLabel != null) {
                DatePickLabel.Dispose ();
                DatePickLabel = null;
            }

            if (TimePickLabel != null) {
                TimePickLabel.Dispose ();
                TimePickLabel = null;
            }
        }
    }
}