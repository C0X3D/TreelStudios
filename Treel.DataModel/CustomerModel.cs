using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Treel.DataModel
{
    public class CustomerModel
    {
        public int Id { get; set; }
        public string Customer_Site_Name { get; set; } = string.Empty;
        public string Customer_Secret { get; set; } = string.Empty;
        public string Customer_Api_Key { get; set; } = string.Empty;
        public string Endpoint_User_Authenticate { get; set; } = string.Empty;
        public string Endpoint_User_Balance {  get; set; } = string.Empty;
        public string Customer_Curency_Name { get; set; } = string.Empty;
        public string Endpoint_User_Withdraw_Balance { get; set; } = string.Empty;
        public string Endpoint_User_Deposit_Balance { get; set; } = string.Empty;
        public string Customer_Endpoint { get; set; } = string.Empty;
    }
}
