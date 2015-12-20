using SoftwareKobo.ACGNews.Models;
using System.Threading.Tasks;

namespace SoftwareKobo.ACGNews.Services
{
    public interface IService
    {
        Task<string> DetailAsync(FeedBase feed);
    }
}