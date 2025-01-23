using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using OrderService.Grpc.Gateway.ExceptionHandlers;
using OrderService.Grpc.Gateway.Extensions;
using OrderService.Grpc.Gateway.Options;
using System.Reflection;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

builder.Services.AddOptions<GrpcClientOptions>().BindConfiguration(nameof(GrpcClientOptions));

builder.Services.AddGrpcClients();
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(o =>
{
    string xmlPath = Path.Combine(AppContext.BaseDirectory, $"{Assembly.GetExecutingAssembly().GetName().Name}.xml");
    o.IncludeXmlComments(xmlPath);
});
builder.Services.AddGrpcSwagger();

builder.Services.AddExceptionHandler<GrpcExceptionHandler>();
builder.Services.AddProblemDetails();

WebApplication app = builder.Build();

app.MapControllers();
app.UseSwagger();
app.UseSwaggerUI();
app.UseExceptionHandler();

app.Run();