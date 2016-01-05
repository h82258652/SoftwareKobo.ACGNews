using SoftwareKobo.ACGNews.Models;
using Windows.Networking.Connectivity;

namespace SoftwareKobo.ACGNews.Utils
{
    public class NetworkManager : BindableBase
    {
        public NetworkManager()
        {
            NetworkInformation.NetworkStatusChanged += delegate
            {
                RaisePropertyChanged(string.Empty);
            };
        }

        public bool HasNetwork => NetworkState != NetworkState.None;

        public bool Is2G => NetworkState == NetworkState._2G;

        public bool Is3G => NetworkState == NetworkState._3G;

        public bool Is4G => NetworkState == NetworkState._4G;

        public bool IsWifi => NetworkState == NetworkState.Wifi;

        public NetworkState NetworkState
        {
            get
            {
                var profile = NetworkInformation.GetInternetConnectionProfile();
                if (profile == null)
                {
                    return NetworkState.None;
                }
                if (profile.IsWlanConnectionProfile)
                {
                    return NetworkState.Wifi;
                }
                if (profile.IsWwanConnectionProfile)
                {
                    switch (profile.WwanConnectionProfileDetails.GetCurrentDataClass())
                    {
                        case WwanDataClass.None:
                            return NetworkState.None;

                        case WwanDataClass.Gprs:
                        case WwanDataClass.Edge:
                            return NetworkState._2G;

                        case WwanDataClass.Umts:
                        case WwanDataClass.Hsdpa:
                        case WwanDataClass.Hsupa:
                        case WwanDataClass.Cdma1xRtt:
                        case WwanDataClass.Cdma1xEvdo:
                        case WwanDataClass.Cdma1xEvdoRevA:
                        case WwanDataClass.Cdma1xEvdv:
                        case WwanDataClass.Cdma3xRtt:
                        case WwanDataClass.Cdma1xEvdoRevB:
                        case WwanDataClass.CdmaUmb:
                            return NetworkState._3G;

                        case WwanDataClass.LteAdvanced:
                            return NetworkState._4G;

                        default:
                            return NetworkState.Unknown;
                    }
                }
                return NetworkState.Unknown;
            }
        }
    }
}