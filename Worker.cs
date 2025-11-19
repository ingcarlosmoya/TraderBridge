using IBApi;
using IBKR_Service.Config;
using IBKR_Service.Handlers;
using Microsoft.Extensions.FileSystemGlobbing;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Net;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices.Marshalling;
using System.Security.Principal;
using System.Text;
using System.Threading;
using System.Threading.Tasks;


namespace IBKR_Service
{
    public class Symbol
    {

        public int conid { get; set; }
        public string ticket { get; set; }
    }
    public class Worker : BackgroundService
    {


        private static string eventsFile = Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
        @"MetaQuotes\Terminal\Common\Files\mt5_ibkr_events.jsonl");

        private readonly string baseUrl = "https://localhost:5000/v1/api";
        const string tickle_Endpoint = "/tickle";
        const string pnl_Endpoint = "/iserver/account/pnl/partitioned";
        const string batchFilePath = @"C:\Users\ingca\clientportal.gw\bin\run.bat"; // Replace with your batch file path
        const string yamlFilePath = @"C:\Users\ingca\clientportal.gw\root\conf.yaml"; // Replace with your batch file path

        // Tabla de equivalencias MT5 -> IBKR
        private static Dictionary<string, Symbol> symbolMap = new Dictionary<string, Symbol>
        {
            { "NAS100", new Symbol{ticket = "MNQ", conid=730283094 } },       // NAS100 CFD -> Micro NASDAQ
            { "US30",   new Symbol{ticket = "MYM", conid=362688015 } },       // US30 CFD   -> Mini Dow
            { "XAUUSD", new Symbol{ticket = "XAUUSD", conid=58430358 } },    // Oro CFD -> Oro spot
            { "EURUSD", new Symbol{ticket = "EUR.USD", conid=12087792 } },    // Forex
            { "USDJPY", new Symbol{ticket = "USD.JPY", conid=15016059 } },    // Forex
            { "GBPUSD", new Symbol{ticket = "GBP.USD", conid=12087797 } }    // Forex
            //{ "ETHUSD", "ETHUSDRR" }    // Forex
        };

        private readonly ILogger<Worker> _logger;
        private readonly ILogger<ResponseHandler> _loggerHandler;
        private readonly ApiMessenger _apiMessenger;

        private readonly SuccessfulResponseHanlder _successfulResponseHanlder;
        private readonly ConfirmationResponseHanlder _confirmationResponseHanlder;
        private readonly ErrorResponseHanlder _errorResponseHanlder;
        private readonly CheckPnlResponseHanlder _checkPnlHanlder;


        private HttpClient _httpClient;
        private readonly IbkrSettings _ibkr;
        private FileSystemWatcher _watcher;

        public Worker(ILogger<Worker> logger, ILogger<ResponseHandler> loggerHandler, ApiMessenger apiMessenger)
        {
            _logger = logger;
            _loggerHandler = loggerHandler;
            _apiMessenger = apiMessenger;
            _successfulResponseHanlder = new SuccessfulResponseHanlder(_loggerHandler, apiMessenger);
            _confirmationResponseHanlder = new ConfirmationResponseHanlder(_loggerHandler, apiMessenger);
            _errorResponseHanlder = new ErrorResponseHanlder(_loggerHandler, apiMessenger);
            _checkPnlHanlder = new CheckPnlResponseHanlder(_loggerHandler, apiMessenger);

            _successfulResponseHanlder.SetNext(_confirmationResponseHanlder);
            _confirmationResponseHanlder.SetNext(_errorResponseHanlder);
            _checkPnlHanlder.SetNext(_errorResponseHanlder);
        }
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("MT5 -> IBKR Bridge started...");
            _logger.LogInformation("Montoring file: " + eventsFile);


            _logger.LogInformation("Testing IBKR connection...");
            StartCLientPortal();
            Check();


            while (!stoppingToken.IsCancellationRequested)
            {
                if (_logger.IsEnabled(LogLevel.Information))
                {
                    SingleEndpointCaller(tickle_Endpoint, stoppingToken);
                    CheckPnL(stoppingToken);
                }
                await Task.Delay(500, stoppingToken);
            }
        }

        private void StartCLientPortal()
        {
            try
            {
                using (Process process = new Process())
                {
                    //ProcessStartInfo startInfo = new ProcessStartInfo();
                    process.StartInfo.FileName = "cmd.exe"; // The executable to run the batch file
                    process.StartInfo.Arguments = "/c bin\\run.bat root\\conf.yaml"; ; // /C executes the command and then terminates
                    process.StartInfo.WorkingDirectory = @"C:\Users\ingca\clientportal.gw"; // 🔹 adjust to your real path
                    process.StartInfo.UseShellExecute = false; // Do not use the OS shell to start the process
                    process.StartInfo.RedirectStandardOutput = true; // Redirect output to capture it in C#
                    process.StartInfo.CreateNoWindow = true; // Do not create a new window for the command prompt
                    process.StartInfo.RedirectStandardError = true;

                    process.OutputDataReceived += (sender, e) =>
                    {
                        if (!string.IsNullOrEmpty(e.Data))
                            Console.WriteLine($"[IBKR] {e.Data}");
                    };

                    process.ErrorDataReceived += (sender, e) =>
                    {
                        if (!string.IsNullOrEmpty(e.Data))
                            Console.WriteLine($"[IBKR ERROR] {e.Data}");
                    };

                    process.Start();
                    process.BeginOutputReadLine();
                    process.BeginErrorReadLine();
                    

                    _logger.LogInformation("IBKR Client Portal Gateway started.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error executing batch file: {ex.Message}");
            }
        }

        private async void SingleEndpointCaller(string endpoint, CancellationToken stoppingToken)
        {
            try
            {
                var response = await _httpClient.GetAsync($"{baseUrl}{tickle_Endpoint}", stoppingToken);
                var content = await response.Content.ReadAsStringAsync();
                var responseLog = ($"Response for {tickle_Endpoint}: {response.StatusCode}");
                if (!response.IsSuccessStatusCode)
                    _logger.LogError(responseLog);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "HTTP call failed");
            }
        }

        private async void CheckPnL(CancellationToken stoppingToken)
        {
            try
            {
                var response = await _apiMessenger.GetAsync($"{baseUrl}{pnl_Endpoint}");
                _checkPnlHanlder.Handle(response);
 
            }
            catch (Exception)
            {
                throw;
            }
        }

        private void Check()
        {

            if (!File.Exists(eventsFile))
            {
                _logger.LogWarning("El archivo no existe todavía. Asegúrate que el EA en MT5 está generando logs.");
            }


            _watcher = new FileSystemWatcher();

            _watcher.Path = Path.GetDirectoryName(eventsFile)!;
            _watcher.Filter = Path.GetFileName(eventsFile);
            _watcher.NotifyFilter = NotifyFilters.LastWrite;

            _watcher.Changed += OnChanged;
            _watcher.EnableRaisingEvents = true;

            _logger.LogInformation("Esperando eventos...");

        }

        private async void OnChanged(object sender, FileSystemEventArgs e)
        {
            try
            {
                var lines = File.ReadAllLines(eventsFile);
                if (lines.Length == 0) return;

                var lastLine = lines[0]; // última línea
                var evt = JsonConvert.DeserializeObject<JObject>(lastLine);

                var signal = ConvertToSignal(evt);
                string command = signal.Command;
                if (!string.IsNullOrEmpty(command))
                {
                    _logger.LogInformation($"[MT5] => {command}");
                    _logger.LogInformation($"[Bridge] => {signal.Type.ToUpper()} {signal.AdjustedSymbol} {signal.AdjustedLots}\n");

                    _watcher.EnableRaisingEvents = false;
                    await CreateOrder(signal);

                    File.WriteAllText(e.FullPath, string.Empty);
                    _watcher.EnableRaisingEvents = true;

                }
            }
            catch (Exception ex)
            {
                _logger.LogError("Error leyendo evento: " + ex.Message);
            }
        }

        private async Task CreateOrder(Signal signal)
        {
            ErrorResponse postResponse = null;
            var createOrder = new CreateOrder
            {
                orders = new List<Order>(){  new Order()
            {
                acctId = "DUN296642",
                conid = signal.AcctId,
                orderType = "MKT",
                quantity = signal.AdjustedLots,
                side = signal.Type,
                ticker = signal.AdjustedSymbol,
                tif = "GTC"

            } }
            };
            var jsonRequest = JsonConvert.SerializeObject(createOrder);
            var jsonResponse = await _apiMessenger.PostAsync($"{baseUrl}/iserver/account/DUN296642/orders", jsonRequest);
            _successfulResponseHanlder.Handle(jsonResponse);

        }

        private static Signal ConvertToSignal(JObject evt)
        {
            var signal = new Signal
            {
                Action = evt["action"]?.ToString(),
                Symbol = evt["symbol"]?.ToString(),
                Lots = evt["lots"]?.ToObject<double>() ?? 0,
                Type = evt["type"]?.ToString(),
                Price = evt["price"]?.ToObject<double>() ?? 0,
                StopLoss = evt["sl"]?.ToObject<double>() ?? 0,
                TakeProfit = evt["tp"]?.ToObject<double>() ?? 0,
                IsPending = true
            };

            if (!string.IsNullOrWhiteSpace(signal.Symbol))
            {
                AdjustLotsSize(signal);
                switch (signal.Action)
                {
                    case "DEAL":
                        signal.Command = $"{signal.Type} {signal.Symbol} Lots={signal.Lots} Price={signal.Price} SL={signal.StopLoss} TP={signal.TakeProfit}";
                        return signal;
                    case "CLOSE":
                        signal.Command = $"CLOSE {signal.Symbol} {signal.Lots}";
                        return signal;
                    case "MODIFY":
                        signal.Command = $"MODIFY {signal.Symbol} {signal.Lots} SL={signal.StopLoss} TP={signal.TakeProfit}";
                        return signal;
                    default:
                        return signal;
                }
            }
            return signal;
        }

        private static void AdjustLotsSize(Signal signal)
        {
            signal.AdjustedLots = signal.Lots < 1 ? 1 : signal.Lots;
            var symbol = symbolMap[signal.Symbol];
            switch (signal.Symbol)
            {
                case ("NAS100"):
                    signal.AdjustedSymbol = symbol.ticket;
                    signal.AcctId = symbol.conid;
                    signal.AdjustedLots = signal.Lots < 1 ? 1 : signal.Lots;
                    return;
                case "US30":
                    signal.AdjustedSymbol = symbol.ticket;
                    signal.AcctId = symbol.conid;
                    signal.AdjustedLots = signal.Lots < 1 ? 1 : signal.Lots;
                    return;
                case "EURUSD":
                    signal.AdjustedSymbol = symbol.ticket;
                    signal.AcctId = symbol.conid;
                    signal.AdjustedLots = signal.Lots < 1 ? 20000 : signal.Lots*20000;
                    return;
                case "USDJPY":
                    signal.AdjustedSymbol = symbol.ticket;
                    signal.AcctId = symbol.conid;
                    signal.AdjustedLots = signal.Lots < 1 ? 25000 : signal.Lots * 25000;
                    return;
                case "GBPUSD":
                    signal.AdjustedSymbol = symbol.ticket;
                    signal.AcctId = symbol.conid;
                    signal.AdjustedLots = signal.Lots < 1 ? 20000 : signal.Lots * 20000;
                    return;
                default:
                    signal.AdjustedSymbol = symbol.ticket;
                    signal.AcctId = symbol.conid;
                    break;
            }
        }



    }
}

