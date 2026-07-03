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
        private const string BASIC_ROLE = "user";

        public PaymentController(ApprovePaymentUseCase approvePaymentUseCase, DeclinePaymentUseCase declineOrderUseCase)
        {
            _approvePaymentUseCase = approvePaymentUseCase;
            _declineOrderUseCase = declineOrderUseCase;
        }
        
        [Authorize(Roles = BASIC_ROLE)]
        [HttpPost("/{orderId}/approve")]
        public async Task<IActionResult> Approve([FromRoute] Guid orderId)
        {
            await _approvePaymentUseCase.ExecuteAsync(orderId, orderId);
            return Ok();
        }
        
        [Authorize(Roles = BASIC_ROLE)]
        [HttpPost("/{orderId}/decline")]
        public async Task<IActionResult> Decline([FromRoute] Guid orderId)
        {
            await _declineOrderUseCase.ExecuteAsync(orderId, orderId);
            return Ok();
        }
    }
}
