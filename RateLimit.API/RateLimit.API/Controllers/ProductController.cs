using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using System.Threading.RateLimiting;

namespace RateLimit.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [EnableRateLimiting("Sliding")] // Hangi politikanın bu controller üzerinde aktif olacağını belirlediğimiz bir attribute
    public class ProductController : ControllerBase
    {
        [HttpGet]
        public IActionResult GetProduct()
        {
            return Ok();
        }
    }
    public class CustomRateLimitPolicy : IRateLimiterPolicy<string>
    {
        public Func<OnRejectedContext, CancellationToken, ValueTask>? OnRejected =>
        (context, cancellationToken) =>
        {
            //Log...
            return new();
        };

        public RateLimitPartition<string> GetPartition(HttpContext httpContext)
        {
            return RateLimitPartition.GetFixedWindowLimiter("", _ => new()
            {
                PermitLimit = 4,
                Window = TimeSpan.FromSeconds(12),
                QueueProcessingOrder = QueueProcessingOrder.OldestFirst,
                QueueLimit = 2
            });
        }

    }
}
