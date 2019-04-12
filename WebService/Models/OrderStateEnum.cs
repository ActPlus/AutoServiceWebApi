
namespace WebService.Models
{
    /// <summary>
    /// OrderStateEnum Tracks progress on every Order
    /// </summary>
    public enum OrderStateEnum
    {
        /// <summary>
        /// Order has been Recieved and stored
        /// </summary>
        Recieved = 1,
        /// <summary>
        /// Order has been Classified Manualy, by Worker
        /// </summary>
        Classified = 2,
        /// <summary>
        /// Order has been accepted by worker
        /// </summary>
        Accepted = 3,
        /// <summary>
        /// Order has been paid by Customer
        /// </summary>
        Paid = 4,
        /// <summary>
        /// Order is in progress
        /// </summary>
        InProgress = 5,
        /// <summary>
        /// Order has been finished
        /// </summary>
        Finished = 6,
        /// <summary>
        /// Order has not been Accepted
        /// </summary>
        Canceled = 7
    }
}
