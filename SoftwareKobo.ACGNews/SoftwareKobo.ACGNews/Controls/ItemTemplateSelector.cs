using SoftwareKobo.ACGNews.Models;
using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Markup;

namespace SoftwareKobo.ACGNews.Controls
{
    public class ItemTemplateSelector : DataTemplateSelector
    {
        private static readonly DataTemplate Acg17173Template = (DataTemplate)XamlReader.Load("<DataTemplate xmlns=\"http://schemas.microsoft.com/winfx/2006/xaml/presentation\" xmlns:controls=\"using:SoftwareKobo.ACGNews.Controls\"><controls:Acg17173Item /></DataTemplate>");

        private static readonly DataTemplate Acg178Template = (DataTemplate)XamlReader.Load("<DataTemplate xmlns=\"http://schemas.microsoft.com/winfx/2006/xaml/presentation\" xmlns:controls=\"using:SoftwareKobo.ACGNews.Controls\"><controls:Acg178Item /></DataTemplate>");

        private static readonly DataTemplate AcgdogeTemplate = (DataTemplate)XamlReader.Load("<DataTemplate xmlns=\"http://schemas.microsoft.com/winfx/2006/xaml/presentation\" xmlns:controls=\"using:SoftwareKobo.ACGNews.Controls\"><controls:AcgdogeItem /></DataTemplate>");

        private static readonly DataTemplate AcgGamerskyTemplate = (DataTemplate)XamlReader.Load("<DataTemplate xmlns=\"http://schemas.microsoft.com/winfx/2006/xaml/presentation\" xmlns:controls=\"using:SoftwareKobo.ACGNews.Controls\"><controls:AcgGamerskyItem /></DataTemplate>");

        private static readonly DataTemplate TencentComicTemplate = (DataTemplate)XamlReader.Load("<DataTemplate xmlns=\"http://schemas.microsoft.com/winfx/2006/xaml/presentation\" xmlns:controls=\"using:SoftwareKobo.ACGNews.Controls\"><controls:TencentComicItem /></DataTemplate>");

        protected override DataTemplate SelectTemplateCore(object item)
        {
            if (item is Acg17173Feed)
            {
                return Acg17173Template;
            }
            if (item is Acg178Feed)
            {
                return Acg178Template;
            }
            if (item is AcgdogeFeed)
            {
                return AcgdogeTemplate;
            }
            if (item is AcgGamerskyFeed)
            {
                return AcgGamerskyTemplate;
            }
            if (item is TencentComicFeed)
            {
                return TencentComicTemplate;
            }
            if (item == null)
            {
                return base.SelectTemplateCore(null);
            }

            throw new InvalidOperationException("未定义控件模板");
        }
    }
}