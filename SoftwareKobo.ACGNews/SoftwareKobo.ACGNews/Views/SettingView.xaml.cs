using SoftwareKobo.ACGNews.Converters;
using SoftwareKobo.ACGNews.Datas;
using SoftwareKobo.ACGNews.Web;
using System;
using System.Threading.Tasks;
using Windows.UI.Xaml;

namespace SoftwareKobo.ACGNews.Views
{
    public partial class SettingView
    {
        public SettingView()
        {
            InitializeComponent();
        }

        private void CacheSizeTextBlock_Loaded(object sender, RoutedEventArgs e)
        {
            var timer = new DispatcherTimer()
            {
                Interval = TimeSpan.FromSeconds(10)
            };
            timer.Tick += delegate
            {
                if (AppView.Instance.CurrentView == this)
                {
                    SetCacheSizeTextBlockText();
                }
            };
            timer.Start();
            SetCacheSizeTextBlockText();
        }

        private async Task<ulong> GetAllCacheSizeAsync()
        {
            var task1 = ImageCacheConverter.GetCachedImagesSizeAsync();
            var task2 = UriToStreamResolver.GetCachedImagesSizeAsync();
            var task3 = AppDatabase.GetDatabaseSize();
            await Task.WhenAll(task1, task2, task3);
            var r1 = task1.Result;
            var r2 = task2.Result;
            var r3 = task3.Result;
            return r1 + r2 + r3;
        }

        public async void SetCacheSizeTextBlockText()
        {
            var b = await GetAllCacheSizeAsync();
            if (b < 1024)
            {
                CacheSizeTextBlock.Text = b + "b";
                return;
            }
            var kb = b / 1024.0d;
            if (kb < 1024)
            {
                CacheSizeTextBlock.Text = string.Format("{0:F2}kb", kb);
                return;
            }
            var mb = kb / 1024.0d;
            if (mb < 1024)
            {
                CacheSizeTextBlock.Text = string.Format("{0:F2}mb", mb);
                return;
            }
            var gb = mb / 1024.0d;
            CacheSizeTextBlock.Text = string.Format("{0:F2}gb", gb);
        }

        private async void Btn_Click(object sender, RoutedEventArgs e)
        {
            await Task.WhenAll(ImageCacheConverter.CleanUpCacheAsync(), UriToStreamResolver.CleanUpCacheAsync());
            AppDatabase.DeleteDatabase();
            SetCacheSizeTextBlockText();
            NotificationView.ShowToastMessage("已成功清空大部分缓存");
        }
    }
}