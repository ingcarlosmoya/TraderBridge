

using IBKR_Service;
using IBKR_Service.Config;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Net.Http;

//var builder = Host.CreateApplicationBuilder(args);

////var MyAllowSpecificOrigins = "_myAllowSpecificOrigins";


////builder.Services.AddCors(options =>{
////    options.AddPolicy(name: MyAllowSpecificOrigins,
////                  policy =>
////                  {
////                  policy.WithOrigins("",
////                                              "https://www.localhost:5000") // Replace with your client's origin
////                                .AllowAnyHeader()
////                                .AllowAnyMethod();
////                      });
////});

////
////builder.Services.AddHttpClient();
//builder.Services.AddHttpClient("IBKR", client =>
//{
//    client.BaseAddress = new Uri("https://localhost:5000");
//}).ConfigurePrimaryHttpMessageHandler(() =>
//    new HttpClientHandler
//    {
//        ServerCertificateCustomValidationCallback =
//            (sender, cert, chain, sslPolicyErrors) => true
//    });

//builder.Services.AddHostedService<Worker>();

//var host = builder.Build();
//host.Run();

//eyJpZCI6IjYwMzRkNjM5IiwibWFjIjoiMTY6MjE6NjU6RkI6MTQ6MDUifQ


var builder = Host.CreateApplicationBuilder(args);
builder.Services.AddHttpClient();

//// ✅ Define an HttpClient that ignores invalid SSL certs
//builder.Services.AddHttpClient("UnsafeHttpClient", client =>
//{
//    client.BaseAddress = new Uri("https://127.0.0.1:5000"); // or your IBKR URL
//})
//.ConfigurePrimaryHttpMessageHandler(() =>
//    new HttpClientHandler
//    {
//        // 🔥 This line forces .NET to accept invalid/self-signed certs
//        ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator
//    });


// ✅ Define an HttpClient that ignores invalid SSL certs
//builder.Services.AddHttpClient("IBKR", client =>
//{
//    client.BaseAddress = new Uri("https://localhost:5000"); // or your IBKR URL
//    client.DefaultRequestHeaders.Add("Accept", "application/json");
//})

//.ConfigurePrimaryHttpMessageHandler(() =>
// {
//     return new HttpClientHandler
//     {
//         // ⚠️ Solo para entornos locales o certificados autofirmados
//         ServerCertificateCustomValidationCallback = (HttpRequestMessage, cert, chain, errors) => true
//     };
// });
//.ConfigurePrimaryHttpMessageHandler(() =>
//    new HttpClientHandler
//    {
//        // 🔥 This line forces .NET to accept invalid/self-signed certs
//        ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator
//    });

builder.Services.AddSingleton<ApiMessenger>();

// ✅ Register your Worker
builder.Services.Configure<IbkrSettings>(
    builder.Configuration.GetSection("IBKR")
);
builder.Services.AddHostedService<Worker>();
//builder.Services.AddHostedService<ESocketWorker>();

var host = builder.Build();
host.Run();

