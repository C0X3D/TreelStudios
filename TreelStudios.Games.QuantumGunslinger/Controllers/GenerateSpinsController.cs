using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Treel.Games.DataLayer;

namespace TreelStudios.Games.QuantumGunslinger.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GenerateSpinsController : ControllerBase
    {
        public GenerateSpinsController(BaseGameLogic baseGameLogic)
        {
            BaseGameLogic = baseGameLogic;
        }

        public BaseGameLogic BaseGameLogic { get; }

        [HttpPost("GenerateSpin")]
        public async Task<ActionResult<object>> GenerateSpin()
        {
            return BaseGameLogic.BaseSpinLogic();
        }
    }
}
