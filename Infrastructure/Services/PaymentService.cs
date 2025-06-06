﻿using Core.Entities;
using Core.Interfaces;
using Infrastructure.Data;
using Microsoft.Extensions.Configuration;
using Stripe;
using Product = Core.Entities.Product;

namespace Infrastructure.Services;

public class PaymentService(
    IConfiguration config, ICartService cartService, IUnitOfWork unit) : IPaymentService
{
    public async Task<ShoppingCart?> CreateOrUpdatePaymentIntent(string cartId)
    {
        StripeConfiguration.ApiKey = config["StripeSettings:SecretKey"];
       
        var cart = await cartService.GetAsync(cartId);
        
        if (cart is null) return null;

        var shippingPrice = 0m;

        if (cart.DeliveryMethodId.HasValue)
        {
            var deliveryMethod = await unit.Repository<DeliveryMethod>().GetByIdAsync((Guid)cart.DeliveryMethodId);
            
            if (deliveryMethod is null) return null;
            
            shippingPrice = deliveryMethod.Price;
        }

        foreach (var item in cart.Items)
        {
            var product = await unit.Repository<Product>().GetByIdAsync(item.ProductId);
            
            if (product is null) return null;

            if (item.Price != product.Price)
            {
                item.Price = product.Price;
            }
        }

        var service = new PaymentIntentService();

        if (string.IsNullOrWhiteSpace(cart.PaymentIntentId))
        {
            var options = new PaymentIntentCreateOptions
            {
                Amount = (long)cart.Items.Sum(x => x.Quantity * x.Price * 1000) + (long)shippingPrice * 100,
                Currency = "usd",
                PaymentMethodTypes = ["card"]
            };
            PaymentIntent? intent = await service.CreateAsync(options);
            cart.PaymentIntentId = intent.Id;
            cart.ClientSecret = intent.ClientSecret;
        }
        else
        {
            var options = new PaymentIntentUpdateOptions
            {
                Amount = (long)cart.Items.Sum(x => x.Quantity * x.Price * 100) + (long)shippingPrice * 100
            };
            await service.UpdateAsync(cart.PaymentIntentId, options);
        }
        
        await cartService.SetAsync(cart);
        return cart;
    }
}