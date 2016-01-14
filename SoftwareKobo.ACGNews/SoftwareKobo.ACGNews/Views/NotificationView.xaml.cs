using SoftwareKobo.ACGNews.Controls;
using System;
using Windows.UI.Xaml;
using WinRTXamlToolkit.AwaitableUI;

namespace SoftwareKobo.ACGNews.Views
{
    public sealed partial class NotificationView
    {
        public static NotificationView Instance
        {
            get;
            private set;
        }

        public NotificationView()
        {
            InitializeComponent();
            Instance = this;
        }

        public static void ShowLoading()
        {
            Instance.LoadingProgressBar.Visibility = Visibility.Visible;
        }

        public static void HideLoading()
        {
            Instance.LoadingProgressBar.Visibility = Visibility.Collapsed;
        }

        public static async void ShowToastMessage(string message)
        {
            var toastPrompt = new ToastPrompt
            {
                Content = message,
                SlideInDirection = SlideInDirection.Right,
                HorizontalAlignment = HorizontalAlignment.Right,
                VerticalAlignment = VerticalAlignment.Top,
                Margin = new Thickness(0, 32, 0, 0),
                Padding = new Thickness(8, 8, 32, 8)
            };
            Instance.ToastPromptContainer.Children.Add(toastPrompt);
            await toastPrompt.WaitForLoadedAsync();
            await toastPrompt.ShowAsync();
            Instance.ToastPromptContainer.Children.Remove(toastPrompt);
        }
    }
}