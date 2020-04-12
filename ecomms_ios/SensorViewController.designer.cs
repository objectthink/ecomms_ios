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
    [Register ("SensorViewController")]
    partial class SensorViewController
    {
        [Outlet]
        UIKit.UITextField _humidity { get; set; }


        [Outlet]
        UIKit.UITextField _location { get; set; }


        [Outlet]
        UIKit.UITextField _name { get; set; }


        [Outlet]
        UIKit.UITextField _temperature { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIButton setLocationButton { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIButton setNameButton { get; set; }

        [Action ("SetLocationButton_TouchUpInside:")]
        [GeneratedCode ("iOS Designer", "1.0")]
        partial void SetLocationButton_TouchUpInside (UIKit.UIButton sender);

        [Action ("SetNameButton_TouchUpInside:")]
        [GeneratedCode ("iOS Designer", "1.0")]
        partial void SetNameButton_TouchUpInside (UIKit.UIButton sender);

        void ReleaseDesignerOutlets ()
        {
            if (setLocationButton != null) {
                setLocationButton.Dispose ();
                setLocationButton = null;
            }

            if (setNameButton != null) {
                setNameButton.Dispose ();
                setNameButton = null;
            }
        }
    }
}