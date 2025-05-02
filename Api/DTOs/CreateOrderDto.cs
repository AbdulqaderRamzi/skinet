using System.ComponentModel.DataAnnotations;
using Core.Entities.OrderAggregate;

namespace Api.DTOs;

public class CreateOrderDto
{
    [Required]
    public string CartId { get; set; } = null!;
    [Required]
    public Guid DeliveryMethodId { get; set; }
    [Required]
    public ShippingAddress ShippingAddress { get; set; } = null!;
    [Required]
    public PaymentSummary PaymentSummary { get; set; } = null!;
}