using OrderService.Grpc.Gateway.ExceptionHandlers;
using OrderService.Grpc.Gateway.Extensions;
using System.Reflection;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

builder.Services.AddGrpcClients();
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(o =>
{
    string xmlPath = Path.Combine(AppContext.BaseDirectory, $"{Assembly.GetExecutingAssembly().GetName().Name}.xml");
    o.IncludeXmlComments(xmlPath);
    o.CustomSchemaIds(type => type.ToString());
});
builder.Services.AddGrpcSwagger();
builder.Services.AddExceptionHandler<GrpcExceptionHandler>();
builder.Services.AddProblemDetails();

WebApplication app = builder.Build();

app.MapControllers();
app.UseSwagger();
app.UseSwaggerUI();
app.UseExceptionHandler();

await app.RunAsync();