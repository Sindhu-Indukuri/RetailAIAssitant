using Microsoft.AspNetCore.Mvc;
using RetailAIAssitant.Services;

namespace RetailAIAssitant.Controllers
{
    [ApiController]
    [Route("api/recommend")]
    public class RecommendationController
        : ControllerBase
    {
        private readonly RecommendationService _service;

        public RecommendationController(
            RecommendationService service)
        {
            _service = service;
        }

        [HttpGet("{productName}")]
        public async Task<IActionResult>
            Recommend(string productName)
        {
            var result =
                await _service.RecommendAsync(productName);

            return Ok(result);
        }
    }
}