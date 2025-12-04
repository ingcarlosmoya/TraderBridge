using TraderBridge.Adapters;
using TraderBridge.Config;
using TraderBridge.Handlers;
using TraderBridge.Handlers.Response;
using TraderBridge.Interfaces;
using TraderBridge.Services;
using TraderBridge.Workers;

var builder = Host.CreateApplicationBuilder(args);
builder.Services.AddHttpClient();

builder.Services.AddSingleton<ApiMessenger>();
builder.Services.Configure<IBKRSettings>(builder.Configuration.GetSection("IBKRSettings"));
builder.Services.Configure<Mt5Settings>(builder.Configuration.GetSection("Mt5Settings"));

// Core registrations
builder.Services.AddSingleton<IIbkrClient, IbkrClient>();
builder.Services.AddSingleton<IOrderPipeline, OrderPipeline>();
builder.Services.AddSingleton<IOrderAdapter, CfdToFutureAdapter>();
builder.Services.AddSingleton<IOrderRepository, OrderRepository>();
builder.Services.AddSingleton<IMt5Listener, Mt5FileListener>();
builder.Services.AddSingleton<ApiMessenger>();

// Pipeline handlers (order matters)
builder.Services.AddTransient<IOrderHandler, SymbolValidationHandler>();
builder.Services.AddTransient<IOrderHandler, SymbolMappingHandler>();
builder.Services.AddTransient<IOrderHandler, RiskCheckHandler>();
builder.Services.AddTransient<IOrderHandler, BuildIbkrOrderHandler>();
builder.Services.AddTransient<IOrderHandler, SendToIbkrHandler>();
builder.Services.AddTransient<IOrderHandler, LoggingHandler>();

builder.Services.AddTransient<SuccessfulResponseHandler>();
builder.Services.AddTransient<ConfirmationResponseHandler>();
builder.Services.AddTransient<ErrorResponseHandler>();
builder.Services.AddTransient<CheckPnlResponseHandler>();

builder.Services.AddSingleton<IIbkrWorker, IbkrWorker>();
builder.Services.AddHostedService<ManagerWorker>();


var host = builder.Build();
host.Run();
