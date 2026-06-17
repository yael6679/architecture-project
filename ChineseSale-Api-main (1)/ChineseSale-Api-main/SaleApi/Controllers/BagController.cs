using Microsoft.AspNetCore.Mvc;
using SaleApi.Models;
using SaleApi.Services;
using static SaleApi.Dto.BagDto;

namespace SaleApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BagController : ControllerBase
    {
        private readonly IBagService _bagService;
        private readonly ILogger<BagController> _logger;

        public BagController(IBagService bagService, ILogger<BagController> logger)
        {
            _bagService = bagService;
            _logger = logger;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<GetBagDto>>> GetAllBag()
        {
            var bag = await _bagService.GetAllBag();
            return Ok(bag);
        }

        [HttpPost("add")]
        public async Task<IActionResult> AddToBag([FromBody] CreateBagDto bagDto)
        {
            try
            {
                var result = await _bagService.NewGiftToBag(bagDto);
                if (result == null) return BadRequest();

                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletBag(int id)
        {
            try
            {
                await _bagService.DeleteBag(id);
                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Bag>> GetBagById(int id)
        {
            try
            {
                var bag = await _bagService.GetBagById(id);
                if (bag == null)
                    return NotFound();
                return Ok(bag);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet("user/{id}")]
        public async Task<ActionResult<Bag>> GetBagByUser(int id)
        {
            try
            {
                var bag = await _bagService.GetBagByUser(id);
                if (bag == null)
                    return NotFound();
                return Ok(bag);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet("gift/{id}")]
        public async Task<ActionResult<Bag>> GetBagByGift(int id)
        {
            try
            {
                var bag = await _bagService.GetBagByGift(id);
                if (bag == null)
                    return NotFound();
                return Ok(bag);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpPost("checkout/{userId}")]
        public async Task<IActionResult> Checkout(int userId)
        {
            var result = await _bagService.ProcessCheckout(userId);

            if (result)
            {
                return Ok(new { message = "הרכישה הושלמה בהצלחה, הסל רוקן והכרטיסים עברו להזמנות." });
            }

            return BadRequest("לא ניתן לבצע רכישה. ייתכן והסל ריק.");
        }
    }
}
