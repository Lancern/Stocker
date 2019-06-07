using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Stocker.HBase;
using Stocker.WebAPI.Models;

namespace Stocker.WebAPI.Controllers
{
    [ApiController]
    [Route("stocks")]
    public class StocksController : ControllerBase
    {
        private readonly IHBaseClient _hbaseClient;
        private readonly ILogger<StocksController> _logger;

        public StocksController(IHBaseClient hbaseClient, ILogger<StocksController> logger)
        {
            _hbaseClient = hbaseClient ?? throw new ArgumentNullException(nameof(hbaseClient));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        // GET: /stocks
        [HttpGet]
        public async Task<List<StockListItem>> Get(
            [FromQuery] string search,
            [FromQuery] int? page,
            [FromQuery] int? itemsPerPage)
        {
            throw new NotImplementedException();
        }
    }
}
