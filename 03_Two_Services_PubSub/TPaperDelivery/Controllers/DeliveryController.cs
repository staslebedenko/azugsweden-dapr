using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Dapr;
using Microsoft.Extensions.Logging;

namespace TPaperDelivery
{
    [ApiController]
    public class DeliveryController
    {

        private readonly ILogger<DeliveryController> _logger;

        public DeliveryController(ILogger<DeliveryController> logger)
        {
            _logger = logger;
        }

        [Topic("delivery", "create")]
        [HttpPost]
        [Route("createdelivery")]
        public async Task<IActionResult> ProcessEdiOrder(Delivery delivery)
        {
            _logger.LogWarning("Triggered method");

            var product = new Product { Id = 0, ExternalCode = 5, Name = "Default" };

            var newDelivery = new Delivery
            {
                Id = 0,
                ClientId = delivery.ClientId,
                EdiOrderId = delivery.EdiOrderId,
                Number = delivery.Number,
                ProductId = product.Id,
                ProductCode = product.ExternalCode,
                Notes = "Prepared for shipment"
            };

            _logger.LogWarning("Saved delivery");

            return new OkObjectResult("");
        }

        [HttpGet]
        [Route("api/deliveries/get")]
        public async Task<IActionResult> Get(CancellationToken cts)
        {
            var registeredDeliveries = new List<Delivery>();

            return new OkObjectResult(registeredDeliveries);
        }
    }
}
