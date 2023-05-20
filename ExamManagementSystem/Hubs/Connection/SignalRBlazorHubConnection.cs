using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.AspNetCore.Components;

namespace ExamManagementSystem.Hubs.Connection
{

    public class SignalRBlazorHubConnection
    {
        private bool _isInitialized = false;
        private readonly NavigationManager _navigationManager;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public SignalRBlazorHubConnection(NavigationManager navigationManager, IHttpContextAccessor httpContextAccessor)
        {
            _navigationManager = navigationManager;
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

                    HubConnection = new HubConnectionBuilder()
                        .WithUrl(_navigationManager.ToAbsoluteUri("/notificationhub"), options =>
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

