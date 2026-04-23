using Microsoft.UI.Xaml;
using Microsoft.UI.Windowing;
using Windows.Graphics;

namespace CloudSpritzers1.Src.Helpers
{
    public static class WindowHelper
    {
        public static void MaximizeWindow(Window window)
        {
            var windowHandle = WinRT.Interop.WindowNative.GetWindowHandle(window);
            var windowId = Microsoft.UI.Win32Interop.GetWindowIdFromWindow(windowHandle);
            var appWindow = AppWindow.GetFromWindowId(windowId);

            if (appWindow is not null)
            {
                // Set to overlapped to keep title bar and system buttons
                appWindow.SetPresenter(AppWindowPresenterKind.Overlapped);

                // Get the display area for the window
                var displayArea = DisplayArea.GetFromWindowId(windowId, DisplayAreaFallback.Primary);

                // Resize the window to fill the work area (excluding taskbar)
                appWindow.Resize(new SizeInt32(
                    displayArea.WorkArea.Width,
                    displayArea.WorkArea.Height));

                // Move the window to the top-left of the work area
                appWindow.Move(new PointInt32(
                    displayArea.WorkArea.X,
                    displayArea.WorkArea.Y));
            }
        }
    }
}
