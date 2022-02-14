using Microsoft.AspNetCore.Mvc;
using System.Threading;
using System.Threading.Tasks;

namespace TPaperDelivery
{
    [ApiController]
    [Route("api/[controller]")]
    public class DeliveryController
    {
        [HttpGet]
        [Route("create/{clientId}/{ediOrderId}/{productCode}/{number}")]
        public async Task<IActionResult> ProcessEdiOrder(
            int clientId,
            int ediOrderId,
            int productCode,
            int number,
            CancellationToken cts)
        {
            Product product = new Product { Id = 1, ExternalCode = 2 };

            var newDelivery = new Delivery
            {
                Id = 0,
                ClientId = clientId,
                EdiOrderId = ediOrderId,
                Number = number,
                ProductId = product.Id,
                ProductCode = product.ExternalCode,
                Notes = "Prepared for shipment"
            };

            return new OkObjectResult(newDelivery);
        }

        [HttpGet]
        [Route("health")]
        public async Task<IActionResult> Get(CancellationToken cts)
        {
            return new OkObjectResult("Started");
        }
    }
}
