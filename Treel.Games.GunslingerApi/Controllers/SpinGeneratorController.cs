using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Treel.Games.DataLayer;

namespace Treel.Games.GunslingerApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SpinGeneratorController : ControllerBase
    {
        public BaseGameLogic BaseGameLogic { get; }
        public SpinGeneratorController(BaseGameLogic baseGameLogic)
        {
            BaseGameLogic = baseGameLogic;
        }

        [HttpPost("GenerateSpin")]
        public async Task<ActionResult<object>> GenerateSpin()
        {
            return await Task.Run(BaseGameLogic.BaseSpinLogic);
        }

        [HttpPost("GenerateNormalBonus")]
        public async Task<ActionResult<object>> GenerateNormalBonus()
        {
            return await Task.Run(BaseGameLogic.GenerateNormalBonus);
        }

        [HttpPost("GenerateSuperBonus")]
        public async Task<ActionResult<object>> GenerateSuperBonus()
        {
            return await Task.Run(BaseGameLogic.GenerateSuperBonus);
        }

        [HttpPost("GenerateLeetBonus")]
        public async Task<ActionResult<object>> GenerateLeetBonus()
        {
            return await Task.Run(BaseGameLogic.GenerateLeetBonus);
        }
    }
}
