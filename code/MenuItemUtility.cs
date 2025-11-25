using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace Writer2
{
    internal static class MenuItemUtility
    {
        public static MenuItem CreateMenuItem(string header, Action<object, RoutedEventArgs> handler = null, string iconUri = null, string tag = null)
        {
            var menuItem = new MenuItem();

            menuItem.Header = header;

            if (!string.IsNullOrEmpty(iconUri))
            {
                var bitmapImage = new BitmapImage();

                bitmapImage.BeginInit();
                bitmapImage.UriSource = new Uri(iconUri);
                bitmapImage.EndInit();

                var menuIcon = new Image();

                menuIcon.Source = bitmapImage;
                menuIcon.Width = 16;
                menuIcon.Height = 16;

                menuItem.Icon = menuIcon;
            }

            if (handler != null)
            {
                menuItem.Click += new RoutedEventHandler(handler);
            }

            if (!string.IsNullOrEmpty(tag))
            {
                menuItem.Tag = tag;
            }

            return menuItem;
        }
    }
}