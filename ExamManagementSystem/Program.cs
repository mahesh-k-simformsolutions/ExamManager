using DinkToPdf;
using DinkToPdf.Contracts;
using ExamManagementSystem.Areas.Identity;
using ExamManagementSystem.Background;
using ExamManagementSystem.Data;
using ExamManagementSystem.Data.DbContext;
using ExamManagementSystem.Enums;
using ExamManagementSystem.Helpers;
using ExamManagementSystem.Service;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;
using Hubs = ExamManagementSystem.Hubs;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

// Add services to the container.
string connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));

builder.Logging.AddFile($"Logs/{Assembly.GetExecutingAssembly().GetName().Name}.log");

builder.Services.AddDatabaseDeveloperPageExceptionFilter();

builder.Services.AddDefaultIdentity<User>(options => options.SignIn.RequireConfirmedAccount = false).AddRoles<IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>();
//builder.Services.AddIdentity<User, IdentityRole>()
//        .AddDefaultTokenProviders()
//        .AddDefaultUI();
//builder.Services.AddTransient<IUserStore<User>, UserStore<User, IdentityRole, ApplicationDbContext, string>>();
//builder.Services.AddTransient<IRoleStore<IdentityRole>, RoleStore<IdentityRole, ApplicationDbContext, string>>();
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("VerifiedUser", policy =>
    {
        policy.RequireAuthenticatedUser();
        policy.RequireClaim("Verified", "True");
    });
    options.AddPolicy("VerifiedStudent", policy =>
    {
        policy.RequireAuthenticatedUser();
        policy.RequireRole(EnumUserRole.Student.ToString());
        policy.RequireClaim("Verified", "True");
    });
    options.AddPolicy("VerifiedTeacher", policy =>
    {
        policy.RequireAuthenticatedUser();
        policy.RequireRole(EnumUserRole.Teacher.ToString());
        policy.RequireClaim("Verified", "True");
    });
    options.AddPolicy("VerifiedTeacherOrAdmin", policy =>
    {
        policy.RequireAuthenticatedUser();
        policy.RequireRole(EnumUserRole.Teacher.ToString(), EnumUserRole.Admin.ToString());
        policy.RequireClaim("Verified", "True");
    });
});

builder.Services.AddRazorPages();

builder.Services.AddServerSideBlazor().AddCircuitOptions(o =>
{
    o.DetailedErrors = builder.Environment.IsDevelopment();
});

builder.Services.AddScoped<AuthenticationStateProvider, RevalidatingIdentityAuthenticationStateProvider<User>>();
builder.Services.AddScoped<IClaimsTransformation, ClaimsTransformation>();

builder.Services.AddScoped<Radzen.DialogService>();
builder.Services.AddScoped<Radzen.NotificationService>();
builder.Services.AddScoped<Radzen.AlertService>();
builder.Services.AddScoped<Radzen.DataStore>();
builder.Services.AddScoped<Radzen.TooltipService>();
builder.Services.AddScoped<Radzen.ContextMenuService>();

builder.Services.AddScoped<ExamService>();
builder.Services.AddScoped<QuestionService>();
builder.Services.AddScoped<CommonService>();
builder.Services.AddScoped<ResultService>();

builder.Services.AddHostedService<UpdateExamStatusToStarted>();
builder.Services.AddHostedService<UpdateExamStatusToCompleted>();

builder.Services.AddSingleton(typeof(IConverter), new SynchronizedConverter(new PdfTools()));

builder.Services.AddSignalR();
builder.Services.AddSingleton<Hubs.NotificationHub>();
builder.Services.AddScoped<Hubs.Connection.SignalRBlazorHubConnection>();

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
    endpoints.MapHub<Hubs.NotificationHub>("/notificationHub");
});

await app.CreateAdmin();

app.Run();
