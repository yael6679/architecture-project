using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using SaleApi.Models;
using SaleApi.Services;
using static SaleApi.Dto.DonerDto;
using static SaleApi.Dto.GiftDto;

namespace SaleApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DonerController : ControllerBase
    {
        private readonly IDonerService  _donerService;
        private readonly ILogger<DonerController> _logger;

        public DonerController(IDonerService donerService, ILogger<DonerController> logger)
        {
            _donerService = donerService;
            _logger = logger;
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]

        public async Task<ActionResult<IEnumerable<UpdateDonerDto>>> GetAllDoner()
        {
            var doners = await _donerService.GetAllDoner();
            return Ok(doners);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<UpdateDonerDto>> NewDoner([FromBody] CreateDonerDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var created = await _donerService.NewDoner(dto);
                if (created == null)
                    return BadRequest("Failed to create doner.");

                return Ok(created);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }


        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteDoner(int id)
        {
            try
            {
                await _donerService.DeleteDoner(id);
                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet("{id}")]
        [Authorize(Roles = "Admin")]

        public async Task<ActionResult<Doner>> GetDonerById(int id)
        {
            try
            {
                var doner = await _donerService.GetDonerById(id);
                if (doner == null)
                    return NotFound();
                return Ok(doner);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Internal server error");
            }
        }


        [HttpPut]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<Doner>> UpdateDoner([FromBody] UpdateDonerDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            try
            {
                var updated = await _donerService.UpdateDoner(dto);
                if (updated == null)
                    return BadRequest("Failed to update doner.");
                return Ok(updated);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
        [HttpGet ("withGifts")]
        [Authorize(Roles = "Admin")]

        public async Task<ActionResult<IEnumerable<GetDonerDtoWithGift>>> GetAllDonerWithGift()
        {
            var doners = await _donerService.GetAllDonerWithGift();
            return Ok(doners);
        }


        [HttpGet("withGifts/{id}")]
        [Authorize(Roles = "Admin")]

        public async Task<ActionResult<GetDonerDtoWithGift>> GetDonerByIdWithGigt(int id)
        {
            try
            {
                var doner = await _donerService.GetDonerByIdWithGift(id);
                if (doner == null)
                    return NotFound();
                return Ok(doner);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet("doner/name")]
        [Authorize(Roles = "Admin")]

        public async Task<ActionResult<Doner>> GetDonerByName([FromQuery] string name)
        {
            try
            {
                var doner = await _donerService.GetDonerByName(name);
                if (doner == null)
                    return NotFound();
                return Ok(doner);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet("doner/email")]
        [Authorize(Roles = "Admin")]

        public async Task<ActionResult<Doner>> GetDonerByMail([FromQuery] string email)
        {
            try
            {
                var doner = await _donerService.GetDonerByMail(email);
                if (doner == null)
                    return NotFound();
                return Ok(doner);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet("doner/gift")]
        [Authorize(Roles = "Admin")]

        public async Task<ActionResult<Doner>> GetDonerByGift([FromQuery] string giftName)
        {
            try
            {
                var doner = await _donerService.GetDonerByGift(giftName);
                if (doner == null)
                    return NotFound();
                return Ok(doner);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Internal server error");
            }
        }
    }
}
