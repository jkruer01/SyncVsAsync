using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SyncVsAsync.Services;

namespace SyncVsAsync.Controllers
{
    [Route("api/[controller]")]
    public class SearchBingAsyncController : Controller
    {
        private readonly AsyncService _syncService;

        public SearchBingAsyncController()
        {
            _syncService = new AsyncService();
        }

        // GET api/values
        [HttpGet]
        public async Task<List<string>> GetAsync()
        {
            return await _syncService.SearchAsync();
        }
    }
}
