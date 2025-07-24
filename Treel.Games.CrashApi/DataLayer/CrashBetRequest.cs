namespace Treel.Games.CrashApi.DataLayer
{
    public class CrashBetRequest
    {
        public string UserId { get; set; }
        public string UserName { get; set; }
        public double Amount { get; set; }
    }


    public enum CrashBetResponseStatus
    {
        Failed = 0,
        Success,
        RoundInProgress,
        DuplicateBet
    }
    public class CrashBetResponse
    {
       public CrashBetResponseStatus Success { get; set; }
    }
}
