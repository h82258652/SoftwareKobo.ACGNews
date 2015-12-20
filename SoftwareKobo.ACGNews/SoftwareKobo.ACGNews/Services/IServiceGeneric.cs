using SoftwareKobo.ACGNews.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SoftwareKobo.ACGNews.Services
{
    public interface IService<T> : IService where T : FeedBase
    {
        Task<IEnumerable<T>> GetAsync(int page = 0);
    }
}