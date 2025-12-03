using TraderBridge.Models;
namespace TraderBridge.Interfaces;
public interface IOrderRepository
{
    Task SaveAsync(IbkrOrder order);
}
