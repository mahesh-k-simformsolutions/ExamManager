using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.AspNetCore.Components;
using Radzen;
using static ExamManagementSystem.Shared.MainLayout;

namespace ExamManagementSystem.Hubs.Connection
{

    public class SignalRBlazorHubConnection
    {
        private bool _isInitialized = false;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public SignalRBlazorHubConnection(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task Initialize()
        {
            if (!_isInitialized)
            {
                var httpContext = _httpContextAccessor.HttpContext;
                if (httpContext != null && httpContext.User != null && httpContext.User.Identity.IsAuthenticated)
                {
                    var authToken = httpContext.Request.Cookies[".AspNetCore.Identity.Application"];
                    var url = $"{httpContext.Request.Scheme}://{httpContext.Request.Host}/notificationhub";
                    HubConnection = new HubConnectionBuilder()
                        .WithUrl(url, options =>
                        {
                            options.Cookies.Add(new System.Net.Cookie(".AspNetCore.Identity.Application", authToken, "/", "localhost"));

                        })
                        .Build();

                    await HubConnection.StartAsync();
                    _isInitialized = true;
                }
            }
        }
        public HubConnection HubConnection { get; private set; }
    }

}

