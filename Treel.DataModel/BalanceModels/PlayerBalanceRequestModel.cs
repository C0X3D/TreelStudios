namespace Treel.DataModel.BalanceModels
{
    public class PlayerBalanceRequestModel : RequestModelBase
    {
        public string SessionToken { get; set; } = string.Empty;
        public string PlayerId { get; set; } = string.Empty;
        public string GameName { get; set; } = string.Empty;
    }
}
