// WARNING
//
// This file has been generated automatically by Visual Studio from the outlets and
// actions declared in your storyboard file.
// Manual changes to this file will not be maintained.
//
using Foundation;
using System;
using System.CodeDom.Compiler;

namespace ecomms_ios
{
    [Register ("InstrumentViewController")]
    partial class InstrumentViewController
    {
        [Outlet]
        UIKit.UILabel _status { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UILabel _location { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UILabel _locationLabel { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UILabel _name { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UILabel _nameLabel { get; set; }

        void ReleaseDesignerOutlets ()
        {
            if (_location != null) {
                _location.Dispose ();
                _location = null;
            }

            if (_locationLabel != null) {
                _locationLabel.Dispose ();
                _locationLabel = null;
            }

            if (_name != null) {
                _name.Dispose ();
                _name = null;
            }

            if (_nameLabel != null) {
                _nameLabel.Dispose ();
                _nameLabel = null;
            }
        }
    }
}