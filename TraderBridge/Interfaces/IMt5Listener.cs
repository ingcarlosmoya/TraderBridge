namespace TraderBridge.Interfaces;
public interface IMt5Listener
{
    Task<bool> StartOrdersReader();
}
