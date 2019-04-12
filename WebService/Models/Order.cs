using System;
using System.ComponentModel.DataAnnotations;

namespace WebService.Models
{
    /// <summary>
    /// Model of Order, definition of Constraints with DataAnnotaions
    /// </summary>
    public class Order : IOrder
    {
        /// <summary>
        /// Unique Id of every Order
        /// </summary>
        public Guid Id { get; set; }
        /// <summary>
        /// Customer who requested Order
        /// Field is required
        /// </summary>
        [Required(ErrorMessage = "CustomerId is Required")]
        public Guid? CustomerId { get; set; }
        /// <summary>
        /// State Tracks progress of every Order
        /// Is updated in run-time
        /// Field is required
        /// </summary>
        [Required(ErrorMessage = "State is Required")]
        public OrderStateEnum? State { get; set; }
        /// <summary>
        /// Destiption is Optional field, defined from Customer
        /// Special request, which couldnt be entered in client.
        /// </summary>
        [StringLength(60, ErrorMessage = "Maximum length of Description is 60characters")]
        public string Description { get; set; }






    }
}
