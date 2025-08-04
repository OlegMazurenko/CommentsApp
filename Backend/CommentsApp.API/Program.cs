using CommentsApp.API.Data;
using CommentsApp.API.Hubs;
using CommentsApp.API.Models.Options;
using CommentsApp.API.Services;
using CommentsApp.API.Services.Interfaces;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

using HtmlSanitizer = CommentsApp.API.Services.HtmlSanitizer;

var builder = WebApplication.CreateBuilder(args);

builder.Services.Configure<FrontendOptions>(builder.Configuration.GetSection(FrontendOptions.SectionName));

builder.Services.AddCors();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddMemoryCache();
builder.Services.AddSignalR();

builder.Services.AddSingleton<FileProcessorQueue>();
builder.Services.AddScoped<ICaptchaService, CaptchaService>();
builder.Services.AddScoped<IHtmlSanitizer, HtmlSanitizer>();
builder.Services.AddScoped<ICommentService, CommentService>();
builder.Services.AddScoped<ICommentValidator, CommentValidator>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IFileService, FileService>();
builder.Services.AddScoped<ICacheService, CacheService>();
builder.Services.AddHostedService(sp => sp.GetRequiredService<FileProcessorQueue>());

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

var frontendOptions = app.Services.GetRequiredService<IOptions<FrontendOptions>>().Value;

app.UseCors(policy =>
{
    policy
        .WithOrigins(frontendOptions.Url)
        .AllowAnyHeader()
        .AllowAnyMethod()
        .AllowCredentials()
        .WithExposedHeaders("Content-Disposition");
});

app.UseHttpsRedirection();
app.UseAuthorization();

app.MapControllers();
app.MapHub<CommentsHub>("/hubs/comments");

using var scope = app.Services.CreateScope();
var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
dbContext.Database.Migrate();

app.Run();
