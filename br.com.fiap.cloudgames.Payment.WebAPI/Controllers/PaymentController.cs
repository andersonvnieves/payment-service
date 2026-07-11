using br.com.fiap.cloudgames.Payment.Application.UseCases.ApprovePayment;
using br.com.fiap.cloudgames.Payment.Application.UseCases.DeclinePayment;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace br.com.fiap.cloudgames.Payment.WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PaymentController : ControllerBase
    {
        private readonly ApprovePaymentUseCase _approvePaymentUseCase;
        private readonly DeclinePaymentUseCase _declineOrderUseCase;

        public PaymentController(ApprovePaymentUseCase approvePaymentUseCase, DeclinePaymentUseCase declineOrderUseCase)
        {
            _approvePaymentUseCase = approvePaymentUseCase;
            _declineOrderUseCase = declineOrderUseCase;
        }
        
        [Authorize]
        [HttpPost("approve")]
        public async Task<IActionResult> Approve([FromQuery] Guid orderId)
        {
            await _approvePaymentUseCase.ExecuteAsync(orderId);
            return Ok();
        }

        [Authorize]
        [HttpPost("decline")]
        public async Task<IActionResult> Decline([FromQuery] Guid orderId)
        {
            await _declineOrderUseCase.ExecuteAsync(orderId);
            return Ok();
        }
    }
}
