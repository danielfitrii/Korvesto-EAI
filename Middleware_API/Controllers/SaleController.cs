using Microsoft.AspNetCore.Mvc;
using Middleware_API.Models;
using Middleware_API.Services;

namespace Middleware_API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")]
    public class SaleController : ControllerBase
    {
        private readonly SaleProcessingService _saleProcessingService;
        private readonly ILogger<SaleController> _logger;

        public SaleController(
            SaleProcessingService saleProcessingService,
            ILogger<SaleController> logger)
        {
            _saleProcessingService = saleProcessingService;
            _logger = logger;
        }

        /// <summary>
        /// Process a new sale transaction
        /// </summary>
        /// <param name="request">The sale request details</param>
        /// <returns>The result of the sale processing</returns>
        /// <response code="200">Returns the processed sale details</response>
        /// <response code="400">If the request is invalid</response>
        /// <response code="500">If there was an internal error</response>
        [HttpPost("process-sale")]
        [ProducesResponseType(typeof(SaleResponseDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(SaleResponseDTO), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(SaleResponseDTO), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<SaleResponseDTO>> ProcessSale([FromBody] SaleRequestDTO request)
        {
            try
            {
                _logger.LogInformation($"Processing sale request for customer {request.CustomerId}, product {request.ProductId}");
                
                var response = await _saleProcessingService.ProcessSaleAsync(request);
                
                if (!response.Success)
                {
                    _logger.LogWarning($"Sale processing failed: {string.Join(", ", response.Errors)}");
                    return BadRequest(response);
                }

                _logger.LogInformation($"Sale processed successfully. Transaction ID: {response.TransactionId}");
                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error processing sale");
                return StatusCode(500, new SaleResponseDTO
                {
                    Success = false,
                    Message = "An unexpected error occurred",
                    Errors = new List<string> { ex.Message }
                });
            }
        }

        /// <summary>
        /// Get detailed information about a specific sale
        /// </summary>
        /// <param name="id">The ID of the sale</param>
        /// <returns>Detailed sale information including customer and loyalty details</returns>
        /// <response code="200">Returns the sale details</response>
        /// <response code="404">If the sale is not found</response>
        /// <response code="500">If there was an internal error</response>
        [HttpGet("{id}/details")]
        [ProducesResponseType(typeof(SaleDetailsResponseDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<SaleDetailsResponseDTO>> GetSaleDetails(string id)
        {
            try
            {
                _logger.LogInformation($"Getting sale details for sale {id}");
                
                var details = await _saleProcessingService.GetSaleDetailsAsync(id);
                if (details == null)
                {
                    return NotFound($"Sale {id} not found");
                }

                return Ok(details);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error getting sale details for {id}");
                return StatusCode(500, new { error = "An error occurred while retrieving sale details" });
            }
        }

        /// <summary>
        /// Get transaction logs for a specific sale
        /// </summary>
        /// <param name="id">The ID of the sale</param>
        /// <returns>List of transaction logs</returns>
        /// <response code="200">Returns the transaction logs</response>
        /// <response code="404">If the sale is not found</response>
        [HttpGet("{id}/logs")]
        [ProducesResponseType(typeof(IEnumerable<TransactionLog>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult<IEnumerable<TransactionLog>> GetTransactionLogs(string id)
        {
            var logs = _saleProcessingService.GetTransactionLogs(id);
            if (!logs.Any())
            {
                return NotFound($"No logs found for sale {id}");
            }

            return Ok(logs);
        }
    }
} 