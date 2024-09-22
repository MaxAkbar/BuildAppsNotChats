using Microsoft.ML.OnnxRuntimeGenAI;
using Microsoft.OpenApi.Models;
using Phi35CudaServer.Models.Swagger;
using PhiMiniLib;
using Swashbuckle.AspNetCore.Filters;

var builder = WebApplication.CreateBuilder(args);
var binDirectory = AppDomain.CurrentDomain.BaseDirectory;
var solutionDirectory = Directory.GetParent(binDirectory)!.Parent!.Parent!.Parent!.Parent!.FullName;
var modelPath = Path.Combine(solutionDirectory, "Models", "Phi-3.5-mini-Instruct-onnx-cuda-fp32");

if (!Directory.Exists(modelPath))
{
    Console.WriteLine($"Directory '{modelPath}' does not exist!");
    return;
}

// Responsible for cleaning up the library during shutdown
using OgaHandle ogaHandle = new();
using Model model = new(modelPath);
using Tokenizer tokenizer = new(model);

IPhiProcessor phiProcessor = new PhiProcessor(model, tokenizer);

builder.Services.AddSingleton<IPhiProcessor>(phiProcessor);
builder.Services.AddSingleton<IPhiHelper>(new PhiHelper(phiProcessor));

// Add logging services
builder.Logging.ClearProviders();
builder.Logging.AddConsole();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "PhiService", Version = "v1" });
    c.ExampleFilters();
});
builder.Services.AddSwaggerExamplesFromAssemblyOf<ModelResponseExample>();
builder.Services.AddSwaggerExamplesFromAssemblyOf<ChatRequestExample>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();

// Add logging middleware to log the raw request body
app.Use(async (context, next) =>
{
    var logger = app.Services.GetRequiredService<ILogger<Program>>();
    context.Request.EnableBuffering();
    var body = await new StreamReader(context.Request.Body).ReadToEndAsync();
    context.Request.Body.Position = 0;
    
    logger.LogInformation("Request path: {Path}", context.Request.Path);
    logger.LogInformation("Request query string: {QueryString}", context.Request.QueryString);
    logger.LogInformation("Raw request body: {Body}", body);
    await next.Invoke();
});

app.MapControllers();
app.Run();