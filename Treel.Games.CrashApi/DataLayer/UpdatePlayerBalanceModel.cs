namespace Treel.Games.CrashApi.DataLayer
{
    public class UpdatePlayerBalanceModel
    {
        public string GameId { get; set; }
        public string UserId { get; set; }
        public double PlayerBalance { get; set; }
    }
}
