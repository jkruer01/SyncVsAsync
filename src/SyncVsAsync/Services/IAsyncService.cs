using System.Collections.Generic;
using System.Threading.Tasks;

namespace SyncVsAsync.Services
{
    public interface IAsyncService
    {
        Task<List<string>> SearchAsync();
    }
}
