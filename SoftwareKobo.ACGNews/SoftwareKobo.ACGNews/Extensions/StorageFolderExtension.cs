using System;
using System.Threading.Tasks;
using Windows.Storage;

namespace SoftwareKobo.ACGNews.Extensions
{
    public static class StorageFolderExtension
    {
        public static async Task<ulong> GetSizeAsync(this StorageFolder folder)
        {
            ulong size = 0;
            var childFolders = await folder.GetFoldersAsync();
            foreach (var childFolder in childFolders)
            {
                size += await childFolder.GetSizeAsync();
            }
            var files = await folder.GetFilesAsync();
            foreach (var file in files)
            {
                size += (await file.GetBasicPropertiesAsync()).Size;
            }
            return size;
        }
    }
}