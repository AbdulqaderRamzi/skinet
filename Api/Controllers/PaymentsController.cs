using Core.Entities;
using Core.Interfaces;
using Infrastructure.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

public class PaymentsController(
    IPaymentService paymentService,
    IGenericRepository<DeliveryMethod> dmRepo) : ApiController
{
    [Authorize]
    [HttpPost("{cartId}")]
    public async Task<IActionResult> CreateOrUpdatePaymentIntent(string cartId)
    {
        var cart = await paymentService.CreateOrUpdatePaymentIntent(cartId);

        if (cart is null) return BadRequest("Problem with your cart");

        return Ok(cart);
    }

    [HttpGet("delivery-methods")]
    public async Task<IActionResult> GetDeliveryMethods()
    {
        return Ok(await dmRepo.GetAllAsync());
    }
}