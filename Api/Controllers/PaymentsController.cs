using Api.Extensions;
using Api.SignalR;
using Core.Entities;
using Core.Entities.OrderAggregate;
using Core.Interfaces;
using Core.Specifications;
using Infrastructure.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Stripe;

namespace Api.Controllers;

public class PaymentsController(
    IPaymentService paymentService, IUnitOfWork unit,
    ILogger<PaymentsController> logger, 
    IHubContext<NotificationHub> hubContext) : ApiController
{   
    private readonly string _whSecret = ""; 
    
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
        return Ok(await unit.Repository<DeliveryMethod>().GetAllAsync());
    }

    public async Task<IActionResult> StripWebHook()
    {
        var json = await new StreamReader(Request.Body).ReadToEndAsync();

        try
        {
            var stripeEvent = ConstructStripeEvent(json);

            if (stripeEvent.Data.Object is not PaymentIntent intent)
            {
                return BadRequest("Invalid event data");
            }
            
            await HandlePaymentIntentSucceeded(intent);
            return Ok();
        }
        catch (StripeException e)
        {
            logger.LogError(e,"Stripe webhook error");
            return StatusCode(StatusCodes.Status500InternalServerError);
        }
        catch (Exception e)
        {
            logger.LogError(e, "An unexpected error occured");
           return StatusCode(StatusCodes.Status500InternalServerError, "webhook error");
        }
    }

    private async Task HandlePaymentIntentSucceeded(PaymentIntent intent)
    {
        if (intent.Status == "succeeded")
        {
            var spec = new OrderSpecification(intent.Id, true);
            var order = await unit.Repository<Order>().GetEntityAsync(spec)
                ?? throw new Exception("Order not found");

            order.Status = (long)order.GetTotal * 100 != intent.Amount 
                ? OrderStatus.PaymentMismatch 
                : OrderStatus.PaymentReceived;

            await unit.CompleteAsync();

            var connectionId = NotificationHub.GetConnectionIdByEmail(order.BuyerEmail);
            if (!string.IsNullOrEmpty(connectionId))
            {
                await hubContext.Clients.Client(connectionId)
                    .SendAsync("OrderCompleteNotification", order.ToDto());
            }
        }
    }

    private Event ConstructStripeEvent(string json)
    {
        try
        {
            return EventUtility.ConstructEvent(json, Request.Headers["Stripe-Signature"], _whSecret);
        }
        catch (Exception e)
        {
            logger.LogError(e, "Failed to construct stripe event");
            throw new StripeException("Invalid signature");
        }
    }
}