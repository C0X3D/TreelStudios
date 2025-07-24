using Treel.DataModel;


namespace DatabaseContext
{
    public class CustomerSettings
    {
        public CustomerModel Customer = new CustomerModel()
        {
            Id = 0,
            Customer_Site_Name = "coxinocazino",
            //Customer_Endpoint = "http://localhost:5001",
            Customer_Endpoint = "https://oauth2.coxichat.ro",
            Endpoint_User_Authenticate = "/api/TreelAuthentication/authenticate",
            Endpoint_User_Balance = "/api/TreelTransactions/TreelBalanceGet",
            Endpoint_User_Withdraw_Balance = "/api/TreelTransactions/TreelTransactionWithdraw",
            Endpoint_User_Deposit_Balance = "/api/TreelTransactions/TreelTransactionDeposit",
            Customer_Curency_Name = "COX",
            Customer_Secret = "8Lq9H2s244Z5MwhKK8f1TuMz5yuHuS1U",
            Customer_Api_Key = "YQR7LrbCm16NFmsDQT4yCzUZkChCKwrM",
        };

        public CustomerSettings()
        {
        }

        private static CustomerSettings instance;
        public static CustomerSettings InstanceSettings
        {
            get
            {
                if (instance == null)
                {
                    instance = new CustomerSettings();
                }
                return instance;
            }
        }
    }
}
