namespace VolatilTreels.Api.SignalR.DataLayer.Intefaces;

public interface ISpinHub
{
    public Task SpinResultAsync(object data);
    public Task BalanceRefreshAsync(object data);
    public Task UserConnectedAsync(object data);
}
