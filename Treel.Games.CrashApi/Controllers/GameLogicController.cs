using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using System.Collections.Concurrent;
using Treel.Games.CrashApi.DataLayer;
using Treel.Games.CrashApi.GameManager.SignalR;
using Treel.Games.CrashApi.GameManager.SignalR.Interfaces;
using TreelGamesDataModel;

namespace Treel.Games.CrashApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GameLogicController : ControllerBase
    {
        public GameLogicController(ICrashHubService crashHub)
        {
            CrashHub = crashHub;
        }

        public ICrashHubService CrashHub { get; }

        [HttpGet("StartServer")]
        public async Task<ActionResult<object>> StartServerAsync()
        {
            await CrashHub.StartGame();
            return Ok();
        }

        [HttpPost("place-bet")]
        public async Task<ActionResult<CrashBetResponse>> PlaceBetAsync([FromBody] CrashBetRequest betRequest)
        {
            return Ok(await CrashHub.PlaceBetAsync(betRequest));
        }

        [HttpPost("cash-out")]
        public async Task<ActionResult<CrashCashOutResponse>> CashOutBetAsync([FromBody] CrashCashOutRequest cashOutRequest)
        {
            return Ok(await CrashHub.CashOutAsync(cashOutRequest));
        }

        [HttpGet("provably-fair")]
        public ActionResult<object> GetProvablyFair([FromQuery]string serverSeed,[FromQuery] int roundNumber)
        {
            return Ok(CrashHub.ProvablyFair(serverSeed, roundNumber));
        }

        //todo
        /*
         * move bets[] into hubservice
         * at the end of round remove all
         * fix place bet  & cash-out to allow more than 1 bet per user
         * finish the client -> server -> game -> server -> client <- game
         */
    }
}
