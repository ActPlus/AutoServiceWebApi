using System;

namespace WebService.Models
{
    /// <summary>
    /// Interface defines Attribute of Order
    /// </summary>
    public interface IOrder
    {

        Guid Id { get; set; }
        Guid? CustomerId { get; set; }
        OrderStateEnum? State { get; set; }
        string Description { get; set; }
        
        
    }
}
