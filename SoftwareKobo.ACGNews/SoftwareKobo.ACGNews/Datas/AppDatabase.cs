using SoftwareKobo.ACGNews.Models;
using SQLite.Net;
using SQLite.Net.Platform.WinRT;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Windows.Storage;

namespace SoftwareKobo.ACGNews.Datas
{
    public static class AppDatabase
    {
        private static readonly string DbPath = Path.Combine(ApplicationData.Current.LocalFolder.Path, "db.db");

        public static IEnumerable<T> GetFeeds<T>() where T : FeedBase
        {
            using (var conn = GetDbConnection<T>())
            {
                var feeds = conn.Table<T>().OrderByDescending(temp => temp.SortId).ToList();
                if (feeds.Count > 100)
                {
                    conn.RunInTransaction(() =>
                    {
                        foreach (var deleteFeed in feeds.Skip(100))
                        {
                            // ReSharper disable once AccessToDisposedClosure
                            conn.Delete(deleteFeed);
                        }
                    });
                }
                return feeds;
            }
        }

        public static void InsertOrUpdateFeeds<T>(IEnumerable<T> feeds) where T : FeedBase
        {
            using (var conn = GetDbConnection<T>())
            {
                conn.InsertOrReplaceAll(feeds, typeof(T));
            }
        }

        private static SQLiteConnection GetDbConnection<T>() where T : FeedBase
        {
            var conn = new SQLiteConnection(new SQLitePlatformWinRT(), DbPath);
            conn.CreateTable(typeof(T));
            return conn;
        }
    }
}