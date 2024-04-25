using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;

namespace VisualPlayer.Utilities
{
    public static class WindowHelper
    {

        //  METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> Adjust window in screen. </summary>
        /// <param name="window"> Window. </param>
        public static void AdjustWindowInScreen(Window window)
        {
            var windowScreen = GetWindowScreen(window);

            if (windowScreen != null)
                AdjustWindowToScreen(window, windowScreen);
            else
                AdjustWindowToPrimaryScreen(window);
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Adjust window in primary screen bounds. </summary>
        /// <param name="window"> Window. </param>
        private static void AdjustWindowToPrimaryScreen(Window window)
        {
            AdjustWindowToScreen(window, Screen.PrimaryScreen);
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Adjust window in screen bounds. </summary>
        /// <param name="window"> Window. </param>
        /// <param name="screen"> Destination screen. </param>
        private static void AdjustWindowToScreen(Window window, Screen screen)
        {
            if (screen == null)
                return;

            double x = window.Left;
            double y = window.Top;
            double height = window.ActualHeight;
            double width = window.ActualWidth;

            //  Check right bound.
            if (x + width > screen.WorkingArea.X + screen.WorkingArea.Width)
                x = screen.WorkingArea.Width - width;

            //  Check left bound.
            if (x < screen.WorkingArea.X)
                x = screen.WorkingArea.X;

            //  Check bottom bound.
            if (y + height > screen.WorkingArea.Y + screen.WorkingArea.Height)
                y = screen.WorkingArea.Height - height;

            //  Check top bound.
            if (y < screen.WorkingArea.Y)
                y = screen.WorkingArea.Y;

            //  Check height.
            if (y > screen.WorkingArea.Height)
                height = screen.WorkingArea.Height;

            //  Check width.
            if (x > screen.WorkingArea.Width)
                width = screen.WorkingArea.Width;

            window.Left = x;
            window.Top = y;
            window.Height = height;
            window.Width = width;
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Get window screen. </summary>
        /// <param name="window"> Window. </param>
        /// <returns> Screen in which window appears. </returns>
        private static Screen GetWindowScreen(Window window)
        {
            double centerX = window.Left + (window.Width / 2);
            double centerY = window.Top + (window.Height / 2);

            foreach (var screen in Screen.AllScreens)
            {
                double rightBound = screen.Bounds.X + screen.Bounds.Width;
                double bottomBound = screen.Bounds.Y + screen.Bounds.Height;

                if (MathHelper.IsInRange(centerX, screen.Bounds.X, rightBound - 1)
                    && MathHelper.IsInRange(centerY, screen.Bounds.Y, bottomBound - 1))
                    return screen;
            }

            return null;
        }

    }
}
