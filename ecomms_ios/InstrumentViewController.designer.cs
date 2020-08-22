// WARNING
//
// This file has been generated automatically by Visual Studio to store outlets and
// actions made in the UI designer. If it is removed, they will be lost.
// Manual changes to this file may not be handled correctly.
//
using Foundation;
using System.CodeDom.Compiler;

namespace ecomms_ios
{
	[Register ("InstrumentViewController")]
	partial class InstrumentViewController
	{
		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		UIKit.UILabel _location { get; set; }

		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		UIKit.UILabel _name { get; set; }

		[Outlet]
		UIKit.UILabel _status { get; set; }
		
		void ReleaseDesignerOutlets ()
		{
			if (_location != null) {
				_location.Dispose ();
				_location = null;
			}

			if (_name != null) {
				_name.Dispose ();
				_name = null;
			}

			if (_status != null) {
				_status.Dispose ();
				_status = null;
			}
		}
	}
}
