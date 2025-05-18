using System.Net;
using System.Threading.Tasks;
using Discount.Application.Commands;
using Discount.Application.Queries;
using Discount.Grpc.Protos;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Discount.API.Controllers
{
    [ApiController]
    [Route("api/discount")]
    public class DiscountController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly ILogger<DiscountController> _logger;

        public DiscountController(IMediator mediator, ILogger<DiscountController> logger)
        {
            _mediator = mediator;
            _logger = logger;
        }

        /// <summary>
        /// Get discount by product name
        /// </summary>
        /// <param name="productName">Name of the product</param>
        /// <returns>Discount information</returns>
        [HttpGet("{productName}", Name = "GetDiscount")]
        [ProducesResponseType(typeof(CouponModel), (int)HttpStatusCode.OK)]
        public async Task<ActionResult<CouponModel>> GetDiscount(string productName)
        {
            var query = new GetDiscountQuery(productName);
            var coupon = await _mediator.Send(query);
            _logger.LogInformation($"Discount retrieved for product {productName}, amount: {coupon.Amount}");
            return Ok(coupon);
        }

        /// <summary>
        /// Create a new discount
        /// </summary>
        /// <param name="command">Discount information</param>
        /// <returns>Created discount</returns>
        [HttpPost]
        [ProducesResponseType(typeof(CouponModel), (int)HttpStatusCode.OK)]
        public async Task<ActionResult<CouponModel>> CreateDiscount([FromBody] CreateDiscountCommand command)
        {
            var coupon = await _mediator.Send(command);
            _logger.LogInformation($"Discount created for product {command.ProductName}, amount: {command.Amount}");
            return CreatedAtRoute("GetDiscount", new { productName = command.ProductName }, coupon);
        }

        /// <summary>
        /// Update an existing discount
        /// </summary>
        /// <param name="command">Updated discount information</param>
        /// <returns>Updated discount</returns>
        [HttpPut]
        [ProducesResponseType(typeof(CouponModel), (int)HttpStatusCode.OK)]
        public async Task<ActionResult<CouponModel>> UpdateDiscount([FromBody] UpdateDiscountCommand command)
        {
            var coupon = await _mediator.Send(command);
            _logger.LogInformation($"Discount updated for product {command.ProductName}, amount: {command.Amount}");
            return Ok(coupon);
        }

        /// <summary>
        /// Delete a discount
        /// </summary>
        /// <param name="productName">Name of the product</param>
        /// <returns>Success status</returns>
        [HttpDelete("{productName}")]
        [ProducesResponseType(typeof(bool), (int)HttpStatusCode.OK)]
        public async Task<ActionResult<bool>> DeleteDiscount(string productName)
        {
            var command = new DeleteDiscountCommand(productName);
            var deleted = await _mediator.Send(command);
            _logger.LogInformation($"Discount deletion for product {productName} was {(deleted ? "successful" : "unsuccessful")}");
            return Ok(deleted);
        }
    }
}
