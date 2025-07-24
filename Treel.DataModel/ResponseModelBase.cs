using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Treel.DataModel
{
    public class ResponseModelBase
    {
        public ResponseModelBase()
        {
        }

        public string Customer_Curency_Name { get; set; } = string.Empty;
        public string Customer_Site_Name {  get; set; } = string.Empty;
    }

    public class RequestModelBase
    {
        public RequestModelBase()
        {
        }

        public string Customer_Curency_Name { get; set; } = string.Empty;
        public string Customer_Site_Name { get; set;} = string.Empty;
    }
}
