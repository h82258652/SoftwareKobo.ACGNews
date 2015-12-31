using SoftwareKobo.ACGNews.DatabaseModels;
using SoftwareKobo.ACGNews.Models;
using SQLite.Net;
using SQLite.Net.Async;
using SQLite.Net.Platform.WinRT;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.ComTypes;
using System.Threading.Tasks;
using Windows.Storage;
using WinRTXamlToolkit.IO.Extensions;

namespace SoftwareKobo.ACGNews.Datas
{
    public static class AppDatabase
    {
        private static readonly string DbPath = Path.Combine(ApplicationData.Current.LocalFolder.Path, "db.db");

        public static IEnumerable<T> GetFeeds<T>(int count, long? idLessThan = null) where T : FeedBase
        {
            using (var conn = GetDbConnection<T>())
            {
                var query = conn.Table<T>();
                if (idLessThan.HasValue)
                {
                    query = query.Where(temp => temp.Id < idLessThan.Value);
                }
                return query.OrderByDescending(temp => temp.Id).Take(count).ToList();
            }
        }

        public static void InsertOrUpdateFeeds<T>(IEnumerable<T> feeds) where T : FeedBase
        {
            using (var conn = GetDbConnection<T>())
            {
                conn.InsertOrReplaceAll(feeds, typeof(T));
            }
        }

        public static string LoadArticle(string url)
        {
            using (var conn = GetDbConnection<ArticleEntity>())
            {
                var article = conn.Table<ArticleEntity>().SingleOrDefault(temp => temp.Url == url);
                return article?.Html;
            }
        }

        public static int SaveArticle(string url, string html)
        {
            using (var conn = GetDbConnection<ArticleEntity>())
            {
                return conn.InsertOrReplace(new ArticleEntity
                {
                    Url = url,
                    Html = html
                }, typeof(ArticleEntity));
            }
        }

        public static async Task<int> SaveArticleAsync(string url, string html)
        {
            var conn = await GetDbConnectionAsync<ArticleEntity>();
            return await conn.InsertOrReplaceAsync(new ArticleEntity()
            {
                Url = url,
                Html = html
            });
        }

        public static async Task<string> LoadArticleAsync(string url)
        {
            var conn = await GetDbConnectionAsync<ArticleEntity>();
            var article = await conn.Table<ArticleEntity>().Where(temp => temp.Url == url).FirstOrDefaultAsync();
            return article?.Html;
        }

        private static SQLiteConnection GetDbConnection<T>()
        {
            var conn = new SQLiteConnection(new SQLitePlatformWinRT(), DbPath);
            conn.CreateTable(typeof(T));
            return conn;
        }

        private static async Task<SQLiteAsyncConnection> GetDbConnectionAsync<T>() where T : class
        {
            var connFactory = new Func<SQLiteConnectionWithLock>(() => new SQLiteConnectionWithLock(new SQLitePlatformWinRT(), new SQLiteConnectionString(DbPath, false)));
            var conn = new SQLiteAsyncConnection(connFactory);
            await conn.CreateTableAsync<T>();
            return conn;
        }

        public static async Task<IEnumerable<T>> GetFeedsAsync<T>(int count, long? idLessThan = null) where T : FeedBase
        {
            var conn = await GetDbConnectionAsync<T>();
            var query = conn.Table<T>();
            if (idLessThan.HasValue)
            {
                query = query.Where(temp => temp.Id < idLessThan.Value);
            }
            return await query.OrderByDescending(temp => temp.Id).Take(count).ToListAsync();
        }

        public static async Task InsertOrUpdateFeedAsync<T>(T feed) where T : FeedBase
        {
            var conn = await GetDbConnectionAsync<T>();
            await conn.InsertOrReplaceAsync(feed);
        }

        public static async Task InsertOrUpdateFeedsAsync<T>(IEnumerable<T> feeds) where T : FeedBase
        {
            var conn = await GetDbConnectionAsync<T>();
            await conn.InsertOrReplaceAllAsync(feeds);
        }

        public static async Task<ulong> GetDatabaseSize()
        {
            var database = await StorageFile.GetFileFromPathAsync(DbPath);
            if (database == null)
            {
                return 0;
            }
            return await database.GetSize();
        }
    }
}