using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using SyncVsAsync.Services;

namespace SyncVsAsync.Controllers
{
    [Route("api/[controller]")]
    public class SearchBingSyncController : Controller
    {
        private readonly ISyncService _syncService;

        public SearchBingSyncController()
        {
            _syncService = new SyncService();
        }

        // GET api/values
        [HttpGet]
        public List<string> Get()
        {
            return _syncService.Search();
        }
    }
}
