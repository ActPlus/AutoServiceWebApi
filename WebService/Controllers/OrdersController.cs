using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using RestSharp;
using WebService.Models;


namespace WebService.Controllers
{
    /// <summary>
    /// OrdersController Manages CRUD operation of Orders
    /// Uses LiteDB to store Orders
    /// Uses RestSharp to Access external api with customers
    /// </summary>
    [Produces("application/json")]
    [Route("api/[controller]")]
    [ApiController]
    public class OrdersController : ControllerBase
    {
        /// <summary>
        /// _logger manages Logging Events to Console.
        /// </summary>
        private readonly ILogger _logger;
        /// <summary>
        /// _repostitory handles database CRUD operations
        /// </summary>
        private IOrderRepository<Order> _repository;
        /// <summary>
        /// _configuration store configuration from appSettings.json
        /// required to define Database's path and tableName of Orders
        /// </summary>
        private IConfiguration _configuration;

        /// <summary>
        /// Initialisation of _configuration, _logger, _repository injected from StartUp
        /// </summary>
        /// <param name="configuration">Configuration has reqired domain and tableName of orders of in-memory database</param>
        /// <param name="logger">Configured Logger from Program. logger manages Logging Events to Console.</param>
        /// <param name="repository">Handles definition of CRUD methods on in-memory database</param>
        public OrdersController(IConfiguration configuration, ILogger<OrdersController> logger, IOrderRepository<Order> repository)
        {
            _configuration = configuration;
            _logger = logger;
            _repository = repository;

            // Set up od Database configuration
            _repository.databasePath = configuration["database:connection"];
            _repository.tableName = configuration["database:ordersTable"];
        }

        /// <summary>
        /// Handling GET Requests from api/orders
        /// Finding All Orders from Database
        /// </summary>
        /// <returns>
        /// "application/json" with HTTPCode 200 - Returns Json array of All Orders in Database and Http success response code 200
        /// HTTPCode 404 - Return Http Response Code 404 when no Orders are found.
        /// </returns>
        // GET api/orders
        [Produces("application/json")]
        [HttpGet]
        [ProducesResponseType(typeof(Order), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult<IEnumerable<Order>> Get()
        {

            _logger.LogInformation((int)LoggingCodes.GetOrder, "GET Request recieved");
            using (_logger.BeginScope("Handling Get Request"))
            {

                _logger.LogInformation((int)LoggingCodes.GetOrder, "Getting all orders");
                //Finding all Orders from Databse, referencing object: _repository
                var allOrders = _repository.FindAll();

                //No orders are found
                if (allOrders.Count() == 0)
                {
                    _logger.LogWarning((int)LoggingCodes.GetOrderNotFound, "Get returns no orders. None exist.");
                    return NotFound("No orders have been found");
                }

                //At least 1 Order is found in database
                _logger.LogInformation((int)LoggingCodes.GetOrder, "Return all orders Count: " + allOrders.Count());
                return Ok(allOrders);
            }
        }

        /// <summary>
        /// Handling GET Requests from api/orders/{id}
        /// Finding Orders By Id from Database
        /// </summary>
        /// <param name="id">Uniqe GUID of Order, which sould be returned</param>
        /// <returns>
        /// "application/json" with HTTPCode 200 - Returns Json object of Order with {id} passed to argument and Http success response code 200
        /// HTTPCode 404 - Return Http Response Code 404 when Order with {id}  has not been found
        /// </returns>
        // GET api/orders/5
        [Produces("application/json")]
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(Order), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult<IEnumerable<Order>> Get(Guid id)
        {
            _logger.LogInformation((int)LoggingCodes.GetOrder, "GET Request recieved");
            using (_logger.BeginScope("Handling Get Request with id: " + id))
            {

                _logger.LogInformation((int)LoggingCodes.GetOrder, "Getting order with id: " + id);

                //Find in Repository Order with passed {id}
                var employee = _repository.FindByCondition(a => a.Id.Equals(id));
                //No Order has Id passed in {id}
                if (employee.Count() == 0)
                {
                    _logger.LogWarning((int)LoggingCodes.GetOrderNotFound, "Get returns no order. Order with id: " + id + " doesnt exists.");


                    //Building Error string when order has not be found
                    StringBuilder noResultBuilder = new StringBuilder(50);
                    noResultBuilder.Append("Order with id:");
                    noResultBuilder.Append(id);
                    noResultBuilder.Append("has not been found");
                    return NotFound(noResultBuilder.ToString());
                }

                //Request has been handled successfuly 
                return Ok(employee);
            }
        }

        /// <summary>
        /// Handling POST Requests from api/orders
        /// Storing Order in Database.
        /// </summary>
        /// <param name="newOrder">
        /// Request Body contains Fields of Order, at Least Required
        /// </param>
        /// <returns>
        /// application/json of Id and HTTPCode 201 - Successfuly created Order in Database, returns its ID and HTTP success create response Code
        /// HTTPCode 400 - Not Valid Data entered to Request Body, defined by Data Validation annotations in Model/Order,  Http failed response code
        /// </returns>
        // POST api/orders
        [HttpPost]
        [Produces("application/json")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status503ServiceUnavailable)]
        public ActionResult Post([FromBody] Order newOrder)
        {
            //Check if valid customer Id has been entered
            int customerValidationResult = ValidateCustomer(newOrder.CustomerId);

            //If validation failed
            if (customerValidationResult == StatusCodes.Status404NotFound)
            {
                return NotFound("Customer with id: " + newOrder.CustomerId + " doesn't exist");
            }

            //External customer validation api is not responding route at:  _configuration["CustomerValidation:applicationUrl"] + _configuration["CustomerValidation:route"]
            if (customerValidationResult == StatusCodes.Status503ServiceUnavailable)
            {
                return StatusCode(StatusCodes.Status503ServiceUnavailable, "External Customer Validation Api at: " + _configuration["CustomerValidation:applicationUrl"] + _configuration["CustomerValidation:route"] + " is not responding and return HTTP CODE 503");
            }

            _logger.LogInformation((int)LoggingCodes.CreateOrder, "POST Request recieved for order:" + newOrder);
            //Store Order passed from Request Body toDatabase
            _repository.Create(newOrder);
            _logger.LogInformation((int)LoggingCodes.CreateOrder, "Order Created with Id:" + newOrder.Id);
            //Request has been finished successfuly
            return CreatedAtAction("POST", "Id", newOrder.Id);
        }

        /// <summary>
        /// Update operation - Updates Order in database to modifed instance enterd in Request Body
        /// </summary>
        /// <param name="id">Id of Order in database which will be modified</param>
        /// <param name="updatedOrder">New instance of modified Order, will be stored in existing order by id</param>
        /// <returns>
        /// "application/json" with HTTPCode 200 - Returns Json object of Order with {id} passed to argument and Http success response code 200
        /// HTTPCode 404 - Return Http Response Code 404 when Order with {id} has not been found
        /// HTTPCode 404 - Return Http Response Code 404 when Order with {id} has not been found
        /// </returns>
        // PUT api/orders/5
        [HttpPut("{id}")]
        [Produces("application/json")]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status503ServiceUnavailable)]
        public ActionResult<string> Put(Guid id, [FromBody]  Order updatedOrder)
        {
            Guid oldId = id;
            //Check if customerId is valid
            int customerValidationResult = ValidateCustomer(updatedOrder.CustomerId);

            //If validation failed
            if (customerValidationResult == StatusCodes.Status404NotFound)
            {
                return NotFound("Customer with id: " + updatedOrder.CustomerId + " doesn't exist");
            }

            //External api is not responding
            if(customerValidationResult == StatusCodes.Status503ServiceUnavailable)
            {
                return StatusCode(StatusCodes.Status503ServiceUnavailable, "External Customer Validation Api at: " + _configuration["CustomerValidation:applicationUrl"] + _configuration["CustomerValidation:route"] + " is not responding and return HTTP CODE 503");
            }

            //Find Order in repository database, update it, and return result
            bool updateResult = _repository.Update(id, updatedOrder);

            //if order has been found and updated
            if (updateResult)
            {
                //Successfuly updated in database
                //if Id was updated too, return old and new
                if (oldId != updatedOrder.Id)
                {
                    return Ok("Order Updated, Old Id: " + oldId + ", new Id: " + updatedOrder.Id);
                }

                //If id has not been modified return only old
                return Ok("Order Updated, Id: " + oldId);
            }
            _logger.LogWarning((int)LoggingCodes.UpdateOrderNotFound, "Update not executed, Order with Id: " + id + " not found.");

            //Build Not Found Stack
            StringBuilder noResultBuilder = new StringBuilder(50);
            noResultBuilder.Append("Order with id:");
            noResultBuilder.Append(id);
            noResultBuilder.Append("has not been found");
            //Order has not been found in database
            return NotFound(noResultBuilder.ToString());

        }

        /// <summary>
        /// Deletes Order from repository by passed {id}
        /// </summary>
        /// <param name="id">Id of Order to delete</param>
        /// <returns>
        /// HTTP Code 204 and {id} - Order has not been found in database, returns 204HTTP code and passed id 
        /// HTTP Code 200 and {id} - Order has been deleted successfuly, return Http success response code 200.
        /// </returns>
        // DELETE api/orders/5
        [HttpDelete("{id}")]
        [Produces("application/json")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public ActionResult<string> Delete(Guid id)
        {
            _logger.LogInformation("Attempt to Delete Order with id: " + id);
            //Attempt to Delete order with {id}
            bool deleteResult = _repository.Delete(id);

            //If Order was deleted successfuly
            if (deleteResult)
            {
                _logger.LogInformation("Delete of Order with id: " + id + " was successful");
                //Return 200 Http success code, with id of deleted order
                return Ok("Delete of order was successfull with id:" + id);
            }

            //Build Not found string
            StringBuilder noResultBuilder = new StringBuilder(50);
            noResultBuilder.Append("Order with id:");
            noResultBuilder.Append(id);
            noResultBuilder.Append("has not been found");

            //Return failed response code
            _logger.LogInformation("Delete of Order with id: " + id + " was not successful, Order has not been found");
            return NotFound(noResultBuilder.ToString());
        }


        /// <summary>
        /// Validates if CustomerId exists, from external API
        /// defained in appsettings.json at
        /// CustomerValidation
        /// </summary>
        /// <param name="CustomerId">
        /// Entered Id of customer
        /// </param>
        /// <returns>
        /// HTTP Code 503 - Validation was enabled, but couldn't be executed. External api didn't response, return HTTP response code 503, Service Unavailabe
        /// HTTP Code 404 - Validation failed, external Api did not confirmed existance of Customer Id, returns HTTP response code 404, Not Found
        /// HTTP Code 200 - Validation was disabled or CustomerId has been validated successfuly, returns HTTP success response code 200, Ok
        /// </returns>
        protected int ValidateCustomer(Guid? CustomerId)
        {
            using (_logger.BeginScope("Validating Customer"))
            {

                //Check if Validation is enabled in appsettings.json
                if (_configuration["CustomerValidation:enabled"] == "True")
                {
                    _logger.LogInformation("Validating Customer from " + _configuration["CustomerValidation:route"] + "/" + CustomerId + " API");
                    //define route to external Api from appsettings.json
                    string externalApiRoute = _configuration["CustomerValidation:applicationUrl"] + _configuration["CustomerValidation:route"] + "/" + CustomerId;
                    _logger.LogInformation("Request Route is set to: " + externalApiRoute);
                    //Initialize HttpClient with defined route
                    var client = new RestClient(externalApiRoute);


                    //Build Get Request
                    var request = new RestRequest(Method.GET);
                    //Send request to Api and wait for result
                    IRestResponse response = client.Execute(request);

                    //return response code
                    if (response.StatusCode == System.Net.HttpStatusCode.ServiceUnavailable)
                    {
                        return StatusCodes.Status503ServiceUnavailable;
                    }

                    if (!response.IsSuccessful)
                    {
                        _logger.LogWarning("Customer with Id: " + CustomerId + " doesnt exist.");
                        return StatusCodes.Status404NotFound;
                    }
                }
                else
                {
                    _logger.LogWarning("Customer Validation is not enabled in appsettings.json, under: CustomerValidation:enabled");
                }
            }
            //Customer Id has been validated
            return StatusCodes.Status200OK;
        }
    }
}
