using SoftwareKobo.ACGNews.Datas;
using SoftwareKobo.ACGNews.Models;

namespace SoftwareKobo.ACGNews.ViewModels
{
    public class SettingViewModel : BindableBase
    {
        public bool NavigateBackBySlideToRight
        {
            get
            {
                return AppSetting.NavigateBackBySlideToRight;
            }
            set
            {
                AppSetting.NavigateBackBySlideToRight = value;
                RaisePropertyChanged();
            }
        }
    }
}