using TraderBridge.Handlers.Response;

namespace TraderBridge.Interfaces
{
    public interface IResponseHandler
    {
        Task Handle(string jsonResponse, ResponseHandler? middleWorkHandler = null);
        void SetNext(IResponseHandler handler);
    }
}