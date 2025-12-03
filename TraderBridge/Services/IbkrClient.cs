using Microsoft.Extensions.Options;
using System.Diagnostics;
using System.Text.Json;
using TraderBridge.Config;
using TraderBridge.Handlers.Response;
using TraderBridge.Interfaces;
using TraderBridge.Models;
using TraderBridge.Workers;

namespace TraderBridge.Services;
public class IbkrClient : IIbkrClient
{
    public bool IsConnected { get; private set; }

    private readonly IBKRSettings _bridgeSettings;
    private readonly IApiMessenger _apiMessenger;
    private readonly ILogger _logger;
    private SuccessfulResponseHandler _successfulResponseHandler;
    private ConfirmationResponseHandler _confirmationResponseHandler;
    private ErrorResponseHandler _errorResponseHandler;
    private CheckPnlResponseHandler _checkPnlResponseHandler;


    public IbkrClient(ILogger<IbkrWorker> logger, ApiMessenger apiMessenger, IOptions<IBKRSettings> bridgeSettings,
        SuccessfulResponseHandler successfulResponseHandler, ConfirmationResponseHandler confirmationResponseHandler, ErrorResponseHandler errorResponseHandler,
        CheckPnlResponseHandler checkPnlResponseHandler)
    {
        _logger = logger;
        _bridgeSettings = bridgeSettings.Value;
        _apiMessenger = apiMessenger;
        _successfulResponseHandler = successfulResponseHandler;
        _confirmationResponseHandler = confirmationResponseHandler;
        _errorResponseHandler = errorResponseHandler;
        _checkPnlResponseHandler = checkPnlResponseHandler;
    }
    public async Task SendOrderAsync(IbkrOrder order)
    {
        try
        {
            var options = new JsonSerializerOptions
            {
                PropertyNamingPolicy = new LowerCaseNamingPolicy(),
            };

            var orders = new IbkrOrder();
            orders.Orders = new List<IbkrOrder>();
            orders.Orders.Add(order);
            var mt5OrderJson = JsonSerializer.Serialize(orders, options);
            var response = await _apiMessenger.PostAsync($"{_bridgeSettings.GatewayV1Api}/iserver/account/{_bridgeSettings.AccountId}/orders", mt5OrderJson);
            IsConnected = response.IsSuccessStatusCode;
            if (!IsConnected)
                LogError("Send order", response);

            var responseJson = await response.Content.ReadAsStringAsync();
            _confirmationResponseHandler._middleReponseHandler = _successfulResponseHandler;
            _successfulResponseHandler.SetNext(_confirmationResponseHandler);
            _confirmationResponseHandler.SetNext(_errorResponseHandler);
            _logger.LogWarning($"[IbkrClient] Sending {order.Action} {order.Quantity} {order.Symbol}");
            await _successfulResponseHandler.Handle(responseJson);

        }
        catch (Exception)
        {
            throw;
        }
    }

    public async Task CheckPnl()
    {
        try
        {
            var response = await _apiMessenger.GetAsync($"{_bridgeSettings.GatewayV1Api}{_bridgeSettings.PnlEndpoint}");
            IsConnected = response.IsSuccessStatusCode;
            if (!IsConnected)
                LogError("Check PNL", response);

            var responseJson = await response.Content.ReadAsStringAsync();
            _checkPnlResponseHandler.SetNext(_errorResponseHandler);
            await _checkPnlResponseHandler.Handle(responseJson);

        }
        catch (Exception)
        {
            throw;
        }
        
    }

    public async Task<bool> StartClientPortal() {
        var processStarted = false;
        try
        {
            while (!processStarted)
            {
                processStarted =  await RunBatCommand();
                if (processStarted) {
                    var response = await _apiMessenger.GetAsync(_bridgeSettings.GatewayUrl);
                    processStarted = response.IsSuccessStatusCode;
                }
            }

            Process.Start("C:\\Program Files\\Mozilla Firefox\\firefox.exe", _bridgeSettings.GatewayUrl);
            return processStarted;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error executing batch file: {ex.Message}");
            return processStarted;
        }
    }

    private async Task<bool> RunBatCommand()
    {
        bool processStarted = false;
        using (Process process = new Process())
        {
            process.StartInfo.FileName = "cmd.exe";
            process.StartInfo.Arguments = _bridgeSettings.CommandArguments; ;
            process.StartInfo.WorkingDirectory = _bridgeSettings.WorkingDirectory;
            process.StartInfo.UseShellExecute = false;
            process.StartInfo.RedirectStandardOutput = true;
            process.StartInfo.CreateNoWindow = true;
            process.StartInfo.RedirectStandardError = true;
            process.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;

            process.ErrorDataReceived += (sender, e) =>
            {
                if (!string.IsNullOrEmpty(e.Data))
                    _logger.LogError($"There are errors by launching the client portal batch. Reason: {e.Data}");

                processStarted = false;
            };

            _logger.LogInformation("Starting client Portal Gateway....");
            await Task.Delay(5000);
            _logger.LogInformation("Wait client Portal Gateway has finished....");
            process.Start();
            process.BeginOutputReadLine();
            process.BeginErrorReadLine();
            _logger.LogInformation("IBKR Client Portal Gateway started.");
            _logger.LogInformation("Press any key to continue...");
            processStarted = true;
            return processStarted;
        }
    }

    public async Task Tickle()
    {
        try
        {
            var response = await _apiMessenger.GetAsync($"{_bridgeSettings.GatewayV1Api}{_bridgeSettings.TickleEndpoint}");
            IsConnected = response.IsSuccessStatusCode;
            if (!IsConnected)
                LogError("Tickle", response);
        }
        catch (Exception)
        {
            throw;
        }
    }

    private void LogError(string callerName, HttpResponseMessage httpResponseMessage) {
        _logger.LogError($"{callerName} process has failed due to Code: {(int)httpResponseMessage.StatusCode}, Reason: {httpResponseMessage.ReasonPhrase}");
    }
}

public class LowerCaseNamingPolicy : JsonNamingPolicy
{
    public override string ConvertName(string name)
        => name.ToLower();
}
