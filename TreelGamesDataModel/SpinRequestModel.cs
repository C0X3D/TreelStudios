using Treel.DataModel;

namespace TreelGamesDataModel
{
    public class CrashPlaceBetModel : SpinRequestModel
    {
        public string? PlayerName { get; set; } = string.Empty;
    }

    public class SpinRequestModel : RequestModelBase
    {
        public string? SessionToken { get; set; } = string.Empty;
        public string? PlayerId { get; set; } = string.Empty;
        public string? GameId { get; set; } = string.Empty;
        public decimal Amount { get; set; } = 0m;
    }
}
