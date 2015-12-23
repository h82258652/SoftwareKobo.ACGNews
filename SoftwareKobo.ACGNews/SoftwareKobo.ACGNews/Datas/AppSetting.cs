﻿using SoftwareKobo.ACGNews.Services;
using System;
using System.Linq;
using System.Runtime.InteropServices.ComTypes;
using Windows.Storage;

namespace SoftwareKobo.ACGNews.Datas
{
    public static class AppSetting
    {
        public static bool NavigateBackBySlideToRight
        {
            get
            {
                var value = ApplicationData.Current.LocalSettings.Values[nameof(NavigateBackBySlideToRight)];
                if (value is bool)
                {
                    return (bool)value;
                }
                return true;
            }
            set
            {
                ApplicationData.Current.LocalSettings.Values[nameof(NavigateBackBySlideToRight)] = value;
            }
        }

        public static Channel CurrentChannel
        {
            get
            {
                return Channel.TencentComic;

                var value = ApplicationData.Current.LocalSettings.Values[nameof(CurrentChannel)];
                if (value is int)
                {
                    return (Channel)value;
                }
                var array = Enum.GetValues(typeof(Channel)).Cast<Channel>().ToArray();
                return array[App.GlobalRand.Next(array.Length)];
            }
            set
            {
                ApplicationData.Current.LocalSettings.Values[nameof(CurrentChannel)] = (int)value;
            }
        }
    }
}