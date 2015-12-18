using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Foundation.Metadata;
using Windows.Phone.UI.Input;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;
using WinRTXamlToolkit.AwaitableUI;

namespace SoftwareKobo.ACGNews.Views
{
    public sealed partial class AppView
    {
        public AppView()
        {
            InitializeComponent();
            Instance = this;
        }

        public static AppView Instance
        {
            get;
            private set;
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            if (ApiInformation.IsTypePresent("Windows.Phone.UI.Input.HardwareButtons"))
            {
                HardwareButtons.BackPressed += HardwareButtons_BackPressed;
            }
        }

        private void HardwareButtons_BackPressed(object sender, BackPressedEventArgs e)
        {
            if (ApiInformation.IsTypePresent("Windows.Phone.UI.Input.BackPressedEventArgs"))
            {
                e.Handled = true;
                if (Detail.Visibility == Visibility.Visible)
                {
                    Detail.Hide();
                }
            }
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            if (ApiInformation.IsTypePresent("Windows.Phone.UI.Input.HardwareButtons"))
            {
                HardwareButtons.BackPressed -= HardwareButtons_BackPressed;
            }
        }

        public async void NavigateToDetail(string html, Point renderTransformOrigin)
        {
            Detail.ContentGrid.RenderTransformOrigin = renderTransformOrigin;

            await Detail.SetContentAsync(html);

            Detail.Show();
        }
    }
}