using DatabaseContext;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Treel.DataModel.Authentication;
using Treel.DataModel.Authentication.Responses;
using Treel.DataModel.Enums;
using Treel.SendEndpointRequests;
using VolatilTreels.Api.SignalR.DataLayer.Intefaces;

namespace VolatilTreels.Api.Controllers
{
    public class GameController(_IDatabaseContextManager databaseContextManager, IHubsService hubsService) : ControllerBase
    {
        private readonly _IDatabaseContextManager DatabaseContextManager = databaseContextManager;
        private readonly IHubsService _hubsService = hubsService;

        private CustomerSettings databaseContextManagerX = new CustomerSettings();

        [HttpPost("AuthenticateUser")]
        public async Task<ActionResult<AuthenticationModel>> AuthencticateUserAsync([FromBody] AuthenticationModel authenticationModel)
        {
            if (authenticationModel == null)
            {
                return BadRequest(new TreelStudios_ErrorResponse()
                {
                    Status = ResponseStatusEnum.REQUEST_FAILED_OBJECT_NULL,
                });
            }

            if (string.IsNullOrEmpty(authenticationModel.PlayerId) || string.IsNullOrEmpty(authenticationModel.SessionToken))
                return BadRequest(new TreelStudios_ErrorResponse(ResponseStatusEnum.REQUEST_FAILED_OBJECT_NULL));

            CustomerRequestSender sender = new();

            AuthenticationModel authenticationResponseModel = await sender.SendUserAuthenticationRequest(databaseContextManagerX.Customer, authenticationModel);

            return Ok(authenticationResponseModel);
        }
    }
}
