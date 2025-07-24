using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Treel.DataModel.Enums;

namespace Treel.DataModel.Authentication.Responses
{
    public class TreelStudios_ErrorResponse
    {
        public TreelStudios_ErrorResponse()
        {
        }

        public TreelStudios_ErrorResponse(ResponseStatusEnum rEQUEST_FAILED_OBJECT_NULL)
        {
            this.Status = rEQUEST_FAILED_OBJECT_NULL;
        }

        public ResponseStatusEnum Status { get; set; }
        public string Message { get; set; } = string.Empty;
    }
}
