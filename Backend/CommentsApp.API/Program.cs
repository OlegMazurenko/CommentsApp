using CommentsApp.API.Data;
using CommentsApp.API.Hubs;
using CommentsApp.API.Services;

using Microsoft.EntityFrameworkCore;

using HtmlSanitizer = CommentsApp.API.Services.HtmlSanitizer;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy
            .WithOrigins("http://localhost:4200")
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials()
            .WithExposedHeaders("Content-Disposition");
    });
});

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddMemoryCache();
builder.Services.AddSignalR();

builder.Services.AddScoped<IHtmlSanitizer, HtmlSanitizer>();
builder.Services.AddSingleton<FileProcessorQueue>();
builder.Services.AddHostedService(sp => sp.GetRequiredService<FileProcessorQueue>());
builder.Services.AddSingleton<ICaptchaGenerator, CaptchaGenerator>();

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();
app.UseCors();
app.UseHttpsRedirection();
app.UseAuthorization();

app.MapControllers();
app.MapHub<CommentsHub>("/hubs/comments");

using var scope = app.Services.CreateScope();
var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
dbContext.Database.Migrate();

app.Run();
