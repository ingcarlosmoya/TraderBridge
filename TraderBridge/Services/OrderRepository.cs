using TraderBridge.Interfaces;
using TraderBridge.Models;
namespace TraderBridge.Services;
public class OrderRepository : IOrderRepository
{
    // Simple in-memory store for demo
    private readonly List<IbkrOrder> _store = new();
    public Task SaveAsync(IbkrOrder order)
    {
        _store.Add(order);
        return Task.CompletedTask;
    }
}
