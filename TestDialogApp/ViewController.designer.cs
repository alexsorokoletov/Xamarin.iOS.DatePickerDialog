// WARNING
//
// This file has been generated automatically by Xamarin Studio to store outlets and
// actions made in the UI designer. If it is removed, they will be lost.
// Manual changes to this file may not be handled correctly.
//
using Foundation;
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
		
		void ReleaseDesignerOutlets ()
		{
			if (TimePickLabel != null) {
				TimePickLabel.Dispose ();
				TimePickLabel = null;
			}

			if (DatePickLabel != null) {
				DatePickLabel.Dispose ();
				DatePickLabel = null;
			}
		}
	}
}
