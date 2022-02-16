using Dapr;
using Dapr.Client;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace TPaperDelivery
{
    [ApiController]
    public class DeliveryController
    {
        private readonly DaprClient _daprClient;

        private readonly ILogger<DeliveryController> _logger;

        public DeliveryController(ILogger<DeliveryController> logger, DaprClient daprClient)
        {
            _logger = logger;
            _daprClient = daprClient ?? throw new ArgumentNullException(nameof(daprClient));
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

            string savedDelivery = await SaveDelivery(newDelivery);

            return new OkObjectResult("");
        }

        [HttpGet]
        [Route("api/deliveries/get")]
        public async Task<IActionResult> Get(CancellationToken cts)
        {
            var registeredDeliveries = await _daprClient.GetStateAsync<string>("blobstore", "delivery_new");

            return new OkObjectResult(registeredDeliveries);
        }

        private async Task<string> SaveDelivery(Delivery delivery)
        {
            string jsonString = JsonSerializer.Serialize(delivery);
            await _daprClient.SaveStateAsync("blobstore", "delivery_new", jsonString);
            return await _daprClient.GetStateAsync<string>("blobstore", "delivery_new");
        }
    }
}
