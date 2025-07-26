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
            .AllowAnyOrigin()
            .AllowAnyHeader()
            .AllowAnyMethod()
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

builder.Services.AddScoped<HtmlSanitizer>();
builder.Services.AddHostedService<FileProcessorQueue>();
builder.Services.AddSingleton<CaptchaGenerator>();

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
