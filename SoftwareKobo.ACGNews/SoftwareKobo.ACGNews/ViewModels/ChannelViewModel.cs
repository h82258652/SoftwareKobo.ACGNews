﻿using SoftwareKobo.ACGNews.Models;
using SoftwareKobo.ACGNews.Services;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using Windows.UI.Xaml.Controls;

namespace SoftwareKobo.ACGNews.ViewModels
{
    public class ChannelViewModel : BindableBase
    {
        public ChannelViewModel()
        {
            Channels = new ObservableCollection<Channel>(Enum.GetValues(typeof(Channel)).Cast<Channel>());
        }

        public ObservableCollection<Channel> Channels
        {
            get;
            private set;
        }
    }
}