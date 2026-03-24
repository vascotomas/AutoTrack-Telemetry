using Microsoft.AspNetCore.SignalR.Client;

namespace ClientSDK
{
    public class ClientSignalR
    {
        private readonly HubConnection _hubConnection;

        public ClientSignalR(string hubUrl, string token)
        {
            _hubConnection = new HubConnectionBuilder()
                .WithUrl(hubUrl, options =>
                {
                    options.AccessTokenProvider = () => Task.FromResult<string?>(token);
                })
                .WithAutomaticReconnect()
                .Build();
        }

        public void SuscribirEvento<T>(string nombreEvento, Action<T> callback)
        {
            _hubConnection.On<T>(nombreEvento, callback);
        }

        public void SuscribirEvento<T1, T2>(string nombreEvento, Action<T1, T2> callback)
        {
            _hubConnection.On<T1, T2>(nombreEvento, callback);
        }

        public async Task<bool> ConectarAsync()
        {
            try
            {
                await _hubConnection.StartAsync();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task DesconectarAsync()
        {
            if (_hubConnection.State == HubConnectionState.Connected)
            {
                await _hubConnection.StopAsync();
            }
        }
    }
}
