using Dapr.Client;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace TPaperOrders
{
    [ApiController]
    [Route("api/[controller]")]
    public class OrderController
    {
        private readonly ILogger<OrderController> _logger;

        private readonly DaprClient _daprClient;

        public OrderController(
            ILogger<OrderController> logger,
            IHttpClientFactory clientFactory,
            DaprClient daprClient)
        {
            _logger = logger;
            _daprClient = daprClient ?? throw new ArgumentNullException(nameof(daprClient));
        }

        [HttpGet]
        [Route("create/{quantity}")]
        public async Task<IActionResult> ProcessEdiOrder(decimal quantity, CancellationToken cts)
        {
            _logger.LogInformation("Processed a request.");

            var order = new EdiOrder
            {
                ClientId = 1,
                DeliveryId = 1,
                Notes = "Test order",
                ProductCode = 1,
                Quantity = quantity
            };

            DeliveryModel savedDelivery = await CreateDeliveryForOrder(order, cts);

            string responseMessage = $"Accepted EDI message {order.Id} and created delivery {savedDelivery?.Id}";

            return new OkObjectResult(responseMessage);
        }

        private async Task<DeliveryModel> CreateDeliveryForOrder(EdiOrder savedOrder, CancellationToken cts)
        {

            var route = $"api/delivery/create/{savedOrder.ClientId}/{savedOrder.Id}/{savedOrder.ProductCode}/{savedOrder.Quantity}";

            DeliveryModel savedDelivery = await _daprClient.InvokeMethodAsync<DeliveryModel>(
                HttpMethod.Get, "tpaperdelivery", route, cts);

            return savedDelivery;
        }
    }
}
