
namespace WebService.Models
{

    /// <summary>
    /// Logging codes of Events
    /// </summary>
    public enum LoggingCodes
    {

        CreateOrder = 1000,
        ListItems = 1001,
        GetOrder = 1002,
        PutOrder = 1003,
        UpdateItem = 1004,
        DeleteItem = 1005,

        GetOrderNotFound = 4000,
        UpdateOrderNotFound = 4001
    }

    
}
