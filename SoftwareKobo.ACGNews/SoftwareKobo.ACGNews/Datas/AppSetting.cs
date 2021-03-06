﻿using SoftwareKobo.ACGNews.Models;
using SoftwareKobo.ACGNews.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using Windows.Storage;

namespace SoftwareKobo.ACGNews.Datas
{
    public class AppSetting : BindableBase
    {
        private static readonly List<AppSetting> Instances = new List<AppSetting>();

        public AppSetting()
        {
            Instances.Add(this);
        }

        public event EventHandler<Channel> CurrentChannelChanged;

        public Channel CurrentChannel
        {
            get
            {
                var value = ApplicationData.Current.LocalSettings.Values[nameof(CurrentChannel)];
                if (value is int)
                {
                    return (Channel)value;
                }
                var array = Enum.GetValues(typeof(Channel)).Cast<Channel>().ToArray();
                var channel = array[App.GlobalRand.Next(array.Length)];
                CurrentChannel = channel;
                return channel;
            }
            set
            {
                ApplicationData.Current.LocalSettings.Values[nameof(CurrentChannel)] = (int)value;
                foreach (var instance in Instances)
                {
                    instance.RaisePropertyChanged(nameof(CurrentChannel));
                }
                CurrentChannelChanged?.Invoke(this, value);
            }
        }

        public static AppSetting Instance => Instances.FirstOrDefault() ?? new AppSetting();

        public bool NavigateBackBySlideToRight
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
                foreach (var instance in Instances)
                {
                    instance.RaisePropertyChanged(nameof(NavigateBackBySlideToRight));
                }
            }
        }
    }
}