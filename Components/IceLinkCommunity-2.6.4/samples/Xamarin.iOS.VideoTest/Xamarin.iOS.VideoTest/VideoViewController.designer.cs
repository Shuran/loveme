//
// This file has been generated automatically by Visual Studio to store outlets and
// actions made in the Xcode designer. If it is removed, they will be lost.
// Manual changes to this file may not be handled correctly.
//
using Foundation;
using UIKit;

namespace Xamarin.iOS.VideoTest
{
    [Register("VideoViewController")]
    partial class VideoViewController
    {
        [Outlet]
        UIView ContainerLocal { get; set; }

        [Outlet]
        UIView ContainerRemote { get; set; }
        
        void ReleaseDesignerOutlets()
        {
            if (ContainerLocal != null)
            {
                ContainerLocal.Dispose();
                ContainerLocal = null;
            }

            if (ContainerRemote != null)
            {
                ContainerRemote.Dispose();
                ContainerRemote = null;
            }
        }
    }
}