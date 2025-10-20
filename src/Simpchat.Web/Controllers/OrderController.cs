using Microsoft.AspNetCore.Mvc;
using Simpchat.Application.Common.Interfaces.External.Cashing;
using Simpchat.Application.Common.Models.Orders;

namespace Simpchat.Web.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderController : ControllerBase
    {
        private readonly IRabbitMQProducer _producer;
        private readonly ILogger<OrderController> _logger;

        public OrderController(IRabbitMQProducer producer, ILogger<OrderController> logger)
        {
            _producer = producer;
            _logger = logger;
        }

        [HttpPost("send")]
        public IActionResult Send([FromBody] OrderCreatedDto order)
        {
            try
            {
                _producer.SendMessage(order);
                return Ok("Xabar RabbitMQ ga yuborildi.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Xabar yuborishda xatolik");
                return StatusCode(500, "Xatolik yuz berdi");
            }
        }
    }
}
