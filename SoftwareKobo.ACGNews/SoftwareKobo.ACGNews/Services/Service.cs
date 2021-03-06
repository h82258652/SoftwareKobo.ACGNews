﻿using SoftwareKobo.ACGNews.DataModels;
using SoftwareKobo.ACGNews.Models;
using System;

namespace SoftwareKobo.ACGNews.Services
{
    public static class Service
    {
        public static IService GetService(this Channel channel)
        {
            switch (channel)
            {
                case Channel.Acg17173:
                    return new Acg17173Service();

                case Channel.Acg178:
                    return new Acg178Service();

                case Channel.Acgdoge:
                    return new AcgdogeService();

                case Channel.AcgGamersky:
                    return new AcgGamerskyService();

                case Channel.TencentComic:
                    return new TencentComicService();

                default:
                    throw new ArgumentOutOfRangeException(nameof(channel), channel, "频道错误");
            }
        }

        public static IFeedCollection CreateFeedCollection(this Channel channel)
        {
            switch (channel)
            {
                case Channel.Acg17173:
                    return new FeedCollection<Acg17173Feed>(new Acg17173Service());

                case Channel.Acg178:
                    return new FeedCollection<Acg178Feed>(new Acg178Service());

                case Channel.Acgdoge:
                    return new FeedCollection<AcgdogeFeed>(new AcgdogeService());

                case Channel.AcgGamersky:
                    return new FeedCollection<AcgGamerskyFeed>(new AcgGamerskyService());

                case Channel.Anitama:
                    return new FeedCollection<AnitamaFeed>(new AnitamaService());

                case Channel.TencentComic:
                    return new FeedCollection<TencentComicFeed>(new TencentComicService());

                default:
                    throw new ArgumentOutOfRangeException(nameof(channel), channel, "频道错误");
            }
        }

        public static IService GetService(FeedBase feed)
        {
            if (feed is Acg17173Feed)
            {
                return new Acg17173Service();
            }
            if (feed is Acg178Feed)
            {
                return new Acg178Service();
            }
            if (feed is AcgdogeFeed)
            {
                return new AcgdogeService();
            }
            if (feed is AcgGamerskyFeed)
            {
                return new AcgGamerskyService();
            }
            if (feed is AnitamaFeed)
            {
                return new AnitamaService();
            }
            if (feed is TencentComicFeed)
            {
                return new TencentComicService();
            }

            throw new ArgumentException("无法根据该类型 feed 创建服务", nameof(feed));
        }
    }
}