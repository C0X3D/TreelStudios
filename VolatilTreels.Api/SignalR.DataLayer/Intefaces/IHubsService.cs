namespace VolatilTreels.Api.SignalR.DataLayer.Intefaces;

public interface IHubsService
{
    Task SendBalanceRefreshAsync(string userId, string balance);
    Task SendSpinOver(string userId, object message);
    Task SendSpinResultAsync(string userId, object message);
    Task UpdateNotificationsAsync(string userId, object message);
}
