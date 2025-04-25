using Core.Entities;

namespace Infrastructure.Data;

public interface IPaymentService
{
    Task<ShoppingCart?> CreateOrUpdatePaymentIntent(string cartId);
}