using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using POSBackend.Hubs; // your hub namespace

namespace POSBackend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TestNotifyController : ControllerBase
    {
        private readonly IHubContext<OrderHub> _hubContext;

        public TestNotifyController(IHubContext<OrderHub> hubContext)
        {
            _hubContext = hubContext;
        }

        // GET api/testnotify?orderId=123
        [HttpGet]
        public async Task<IActionResult> Get(string orderId)
        {
            await _hubContext.Clients.All.SendAsync("OrderPlaced", orderId);
            return Ok(new { message = $"Notification sent for order {orderId}" });
        }
    }
}
