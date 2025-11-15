using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;

namespace POSBackend.Hubs
{
    public class OrderHub : Hub
    {
        // Called by mobile when a new order is placed
        public async Task NotifyOrderPlaced(string orderId)
        {
            // Send to all connected clients except the sender
            await Clients.All.SendAsync("OrderPlaced", orderId);
        }

        // Called by desktop when an order is confirmed
        public async Task NotifyOrderConfirmed(string orderId)
        {
            await Clients.All.SendAsync("OrderConfirmed", orderId);
        }
    }
}
