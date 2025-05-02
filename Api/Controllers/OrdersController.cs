using Api.DTOs;
using Api.Extensions;
using Core.Entities;
using Core.Entities.OrderAggregate;
using Core.Interfaces;
using Core.Specifications;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

[Authorize]
public class OrdersController(ICartService cartService, IUnitOfWork unit) : ApiController
{
    [HttpPost]
    public async Task<IActionResult> CreateOrder(CreateOrderDto orderDto)
    {
        var email = User.GetEmail();
        var cart = await cartService.GetAsync(orderDto.CartId);

        if (cart is null) return BadRequest("Cart not found");
        
        if (cart.PaymentIntentId is null) 
            return BadRequest("No payment intent for this order");
        
        var items = new List<OrderItem>();

        foreach (var item in cart.Items)
        {
            var productItem = await unit.Repository<Product>().GetByIdAsync(item.ProductId);
            
            if (productItem is null) return BadRequest("Product not found");

            var itemOrdered = new ProductItemOrdered
            {
                ProductId = item.ProductId,
                ProductName = item.ProductName,
                PictureUrl = item.PictureUrl
            };

            var orderItem = new OrderItem
            {
                ItemOrdered = itemOrdered,
                Price = productItem.Price,
                Quantity = item.Quantity
            };

            items.Add(orderItem);
        }
        
        var deliveryMethod = await unit.Repository<DeliveryMethod>().GetByIdAsync(orderDto.DeliveryMethodId);
        if (deliveryMethod is null) return BadRequest("No delivery method selected");

        var order = new Order
        {
            OrderItems = items,
            DeliveryMethod = deliveryMethod,
            ShippingAddress = orderDto.ShippingAddress,
            Subtotal = items.Sum(x => x.Price * x.Quantity),
            PaymentSummary = orderDto.PaymentSummary,
            PaymentIntentId = cart.PaymentIntentId,
            BuyerEmail = email
        };

        await unit.Repository<Order>().AddAsync(order);

        return await unit.CompleteAsync() ? Ok(order) : BadRequest("Problem creating order");
    }

    [HttpGet]
    public async Task<IActionResult> GetOrdersForUser()
    {
        var spec = new OrderSpecification(User.GetEmail());
        var orders = await unit.Repository<Order>().GetAllAsync(spec);
        var ordersToReturn = orders.Select(o => o.ToDto()).ToList();
       
        return Ok(ordersToReturn);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetOrder(Guid id)
    {
        var spec = new OrderSpecification(User.GetEmail(), id);
        var order = await unit.Repository<Order>().GetEntityAsync(spec);
    
        if (order is null) return NotFound();
        
        return Ok(order.ToDto());
    }
 }