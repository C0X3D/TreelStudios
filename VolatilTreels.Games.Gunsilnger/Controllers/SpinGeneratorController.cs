using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Treel.Games.DataLayer;

namespace VolatilTreels.Games.Gunsilnger.Controllers
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
            return BaseGameLogic.BaseSpinLogic();
        }

        [HttpPost("GenerateNormalBonus")]
        public async Task<ActionResult<object>> GenerateNormalBonus()
        {
            return BaseGameLogic.GenerateNormalBonus();
        }

        [HttpPost("GenerateSuperBonus")]
        public async Task<ActionResult<object>> GenerateSuperBonus()
        {
            return BaseGameLogic.GenerateSuperBonus();
        }

        [HttpPost("GenerateLeetBonus")]
        public async Task<ActionResult<object>> GenerateLeetBonus()
        {
            return BaseGameLogic.GenerateLeetBonus();
        }
    }
}
