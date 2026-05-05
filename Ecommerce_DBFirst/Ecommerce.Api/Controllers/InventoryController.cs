using Ecommerce.Api.DTOs;
using Ecommerce.Api.Services;
using Microsoft.AspNetCore.Mvc;

namespace Ecommerce.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class InventoryController : ControllerBase
    {
        private readonly IInventoryService _inventoryService;
        private readonly ILogger<InventoryController> _logger;

        public InventoryController(IInventoryService inventoryService, ILogger<InventoryController> logger)
        {
            _inventoryService = inventoryService;
            _logger = logger;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<InventoryDto>>> GetAll()
        {
            var data = await _inventoryService.GetAllAsync();
            return Ok(data);
        }

        [HttpGet("{productId:int}")]
        public async Task<ActionResult<InventoryDto>> GetByProductId(int productId)
        {
            var data = await _inventoryService.GetByProductIdAsync(productId);
            if (data == null)
            {
                return NotFound(new { message = "Inventory record not found." });
            }

            return Ok(data);
        }

        [HttpPost]
        public async Task<ActionResult<InventoryDto>> Create([FromBody] CreateInventoryDto input)
        {
            if (!ModelState.IsValid)
            {
                return ValidationProblem(ModelState);
            }

            var result = await _inventoryService.CreateAsync(input);
            if (!result.IsSuccess)
            {
                return BadRequest(new { message = result.Error });
            }

            return CreatedAtAction(nameof(GetByProductId), new { productId = result.Data!.ProductId }, result.Data);
        }

        [HttpPut("{productId:int}")]
        public async Task<IActionResult> UpdateStock(int productId, [FromBody] UpdateInventoryStockDto input)
        {
            if (!ModelState.IsValid)
            {
                return ValidationProblem(ModelState);
            }

            var result = await _inventoryService.UpdateStockAsync(productId, input);
            if (!result.IsSuccess)
            {
                return NotFound(new { message = result.Error });
            }

            return NoContent();
        }

        [HttpDelete("{productId:int}")]
        public async Task<IActionResult> DeleteByProductId(int productId)
        {
            var result = await _inventoryService.DeleteByProductIdAsync(productId);
            if (!result.IsSuccess)
            {
                return NotFound(new { message = result.Error });
            }

            return NoContent();
        }
    }
}
