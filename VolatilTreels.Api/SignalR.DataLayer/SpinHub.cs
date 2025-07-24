using Cox.SignalRDataModel;
using Cox.SignalRDataModel.Interfaces;
using Cox.SignalRHub;
using DatabaseContext;
using Microsoft.AspNetCore.SignalR;
using Treel.DataModel.TransactionModels;
using Treel.Games.DataLayer;
using TreelGamesDataModel;
using VolatilTreels.Api.SignalR.DataLayer.Intefaces;

namespace VolatilTreels.Api.SignalR.DataLayer;

public class SpinHub : GeneralHub<ISpinHub>
{
    private readonly _IDatabaseContextManager _databaseContextManager;
    private readonly IHubsService _hubsService;

    public SpinHub(IClientManagementService? clientManagement, _IDatabaseContextManager databaseContextManager, IHubsService hubsService) : base(clientManagement)
    {
        OnNewClientConnected += NotificationsHub_OnNewClientConnected;
        _databaseContextManager = databaseContextManager;
        _hubsService = hubsService;

        databaseContextManager.OnUserBalanceUpdatedEvent += DatabaseContextManager_OnTransferBalanceEvent;
    }

    private void DatabaseContextManager_OnTransferBalanceEvent(string arg1, TreelStudios_BalanceResponseModel arg2)
    {
        _hubsService.SendBalanceRefreshAsync(arg1, arg2.Balance.ToString());
    }

    private void NotificationsHub_OnNewClientConnected(object? sender, ConnectionEventArgs e)
    {
        Clients.Clients(e.ConnectionId).UserConnectedAsync($"New Connection {e.ConnectionId}.");
    }

    public async Task SpinNowAsync(SpinRequestModel spinRequestModel)
    {
        var dataModel = await _databaseContextManager.GetBusyUser($"{spinRequestModel.PlayerId}_{spinRequestModel.GameId}");
        if (dataModel == null)
        {
            await _hubsService.SendSpinOver($"{spinRequestModel.PlayerId}_{spinRequestModel.GameId}", "spin_done");
        }
        if (dataModel?.DataModel?.BaseSpinDataModel?.Count > 0)
        {
            BaseSpinDataModel curSpin = dataModel?.DataModel?.BaseSpinDataModel?.Dequeue()!;
            await _hubsService.SendSpinResultAsync($"{spinRequestModel.PlayerId}_{spinRequestModel.GameId}", Newtonsoft.Json.JsonConvert.SerializeObject(curSpin));
        }
        else
        {
            if (dataModel?.DataModel?.BonusSpinDataModel?.BonusSpinSteps?.Count > 0)
            {
                await _hubsService.SendSpinOver($"{spinRequestModel.PlayerId}_{spinRequestModel.GameId}", "start_bonus");
            }
        }
        await _databaseContextManager.UpdateBusyUser($"{spinRequestModel.PlayerId}_{spinRequestModel.GameId}", dataModel!);
    }

    public async Task SpinBonusNowAsync(SpinRequestModel spinRequestModel)
    {
        var dataModel = await _databaseContextManager.GetBusyUser($"{spinRequestModel.PlayerId}_{spinRequestModel.GameId}");
        if (dataModel == null)
        {
            await _hubsService.SendSpinOver($"{spinRequestModel.PlayerId}_{spinRequestModel.GameId}", "spin_done");
        }

        if (dataModel?.DataModel?.BaseSpinDataModel?.Count > 0)
        {
            await _hubsService.SendSpinOver($"{spinRequestModel.PlayerId}_{spinRequestModel.GameId}", "wait_base_game_over");
            return;
        }

        if (dataModel?.DataModel?.BonusSpinDataModel?.BonusSpinSteps?.Count > 0)
        {
            BonusSpinStepDataModel curbonusSpin = dataModel?.DataModel?.BonusSpinDataModel?.BonusSpinSteps?.Dequeue()!;
            await _hubsService.SendSpinResultAsync($"{spinRequestModel.PlayerId}_{spinRequestModel.GameId}", Newtonsoft.Json.JsonConvert.SerializeObject(curbonusSpin));
        }

        await _databaseContextManager.UpdateBusyUser($"{spinRequestModel.PlayerId}_{spinRequestModel.GameId}", dataModel!);
    }

    public override string GetEventName()
    {
        return nameof(SpinHub);
    }
}
