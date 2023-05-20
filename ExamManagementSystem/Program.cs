using DinkToPdf;
using DinkToPdf.Contracts;
using ExamManagementSystem.Areas.Identity;
using ExamManagementSystem.Background;
using ExamManagementSystem.Data;
using ExamManagementSystem.Data.DbContext;
using ExamManagementSystem.Helpers;
using ExamManagementSystem.Hubs;
using ExamManagementSystem.Hubs.Connection;
using ExamManagementSystem.Service;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Radzen;
using System.Reflection;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

// Add services to the container.
string connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));

builder.Logging.AddFile($"Logs/{Assembly.GetExecutingAssembly().GetName().Name}.log");

builder.Services.AddDatabaseDeveloperPageExceptionFilter();

builder.Services.AddDefaultIdentity<User>(options => options.SignIn.RequireConfirmedAccount = false).AddRoles<IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>();

builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor().AddCircuitOptions(o =>
{
    o.DetailedErrors = builder.Environment.IsDevelopment();
});

builder.Services.AddScoped<AuthenticationStateProvider, RevalidatingIdentityAuthenticationStateProvider<User>>();
builder.Services.AddScoped<IClaimsTransformation, ClaimsTransformation>();

builder.Services.AddScoped<DialogService>();
builder.Services.AddScoped<NotificationService>();
builder.Services.AddScoped<TooltipService>();
builder.Services.AddScoped<ContextMenuService>();

builder.Services.AddScoped<ExamService>();
builder.Services.AddScoped<QuestionService>();
builder.Services.AddScoped<CommonService>();

builder.Services.AddHostedService<UpdateExamStatusToStarted>();
builder.Services.AddHostedService<UpdateExamStatusToCompleted>();
builder.Services.AddSingleton(typeof(IConverter), new SynchronizedConverter(new PdfTools()));

builder.Services.AddSignalR();
builder.Services.AddSingleton<NotificationHub>();
builder.Services.AddScoped<SignalRBlazorHubConnection>();

WebApplication app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    _ = app.UseMigrationsEndPoint();
}
else
{
    _ = app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    _ = app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.MapBlazorHub();
app.MapFallbackToPage("/_Host");

app.UseEndpoints(endpoints =>
{
    endpoints.MapHub<NotificationHub>("/notificationHub");
});

await app.CreateAdmin();

app.Run();
