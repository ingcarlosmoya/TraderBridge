using Microsoft.Extensions.Options;
using System.Text.Json;
using TraderBridge.Config;
using TraderBridge.Interfaces;
using TraderBridge.Models;
using TraderBridge.Workers;

namespace TraderBridge.Services;
public class Mt5FileListener : IMt5Listener
{
    private readonly IBKRSettings _bridgeSettings;
    private readonly IOrderPipeline _pipeline;
    private readonly int _port;
    private readonly FileSystemWatcher _watcher;
    private string _ordersFilePath;
    private readonly ILogger<IbkrWorker> _logger;

    public Mt5FileListener(IOrderPipeline pipeline, IOptions<IBKRSettings> bridgeSettings, ILogger<IbkrWorker> logger)
    {
        _pipeline = pipeline;
        _bridgeSettings = bridgeSettings.Value;
        _logger = logger;
        _ordersFilePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), _bridgeSettings.OrdersFilePath);
        _watcher = new FileSystemWatcher();
    }

    public async Task<bool> StartOrdersReader()
    {
        bool isOrdersFileReady = false;
        while (!isOrdersFileReady)
        {
            isOrdersFileReady = CheckOrdersFileExists();
            if (isOrdersFileReady)
            {
                _logger.LogWarning("The MT5 order's file is ready to be read...");
                break;
            }
            else
                _logger.LogError("The MT5 order's file is not ready yet!");
            await Task.Delay(2000);
        }

        return isOrdersFileReady;
  

    }

    private bool CheckOrdersFileExists()
    {

        if (!File.Exists(_ordersFilePath))
        {
            _logger.LogError("El archivo no existe todavía. Asegúrate que el EA en MT5 está generando logs.");
            return false;
        }

        _watcher.Path = Path.GetDirectoryName(_ordersFilePath)!;
        _watcher.Filter = Path.GetFileName(_ordersFilePath);
        _watcher.NotifyFilter = NotifyFilters.LastWrite;

        _watcher.Changed += OnChanged;
        _watcher.EnableRaisingEvents = true;

        _logger.LogInformation("Esperando eventos...");

        return true;

    }

    private async void OnChanged(object sender, FileSystemEventArgs e)
    {
        try
        {
            var lines = File.ReadAllLines(_ordersFilePath);
            if (lines.Length == 0) return;

            var lastLine = lines[0]; // última línea
            if (!string.IsNullOrWhiteSpace(lastLine))
            {
                var options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                };
                var mt5Order = JsonSerializer.Deserialize<Mt5TradeTransaction>(lastLine, options);
                if (mt5Order != null) { 
                    await _pipeline.ExecuteAsync(mt5Order);
                    if (_pipeline.IsOrderExecuted)
                        _logger.LogWarning($"Order {mt5Order.Ticket} was placed succesfully!");
                    else
                    {
                        _logger.LogError($"Order {mt5Order.Ticket} was not placed due to order pipeline internal error");
                    }

                }
            }
        }
        catch (Exception ex)
        {
            _logger.LogError("Error leyendo evento: " + ex.Message);
        }
    }
}

