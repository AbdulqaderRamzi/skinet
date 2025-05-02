using Core.Entities.OrderAggregate;

namespace Core.Specifications;

public class OrderSpecification : BaseSpecification<Order>
{
    public OrderSpecification(string email)
    {
        AddCriteria(x => x.BuyerEmail == email);
        AddInclude(x => x.OrderItems);
        AddInclude(x => x.DeliveryMethod);
        AddOrderByDescending(x => x.OrderDate);
        
    }
    
    public OrderSpecification(string email, Guid id)
    {
        AddCriteria(x => x.BuyerEmail == email && x.Id == id);
        // just an example to demo include by passing string
        // if you need to use then include you can use a period
        // ex. OrderItems.Something
        AddInclude("OrderItems");
        AddInclude("DeliveryMethod");
    }
}