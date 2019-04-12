using LiteDB;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;

/// <summary>
/// Repository of type Orders
/// Implements CRUD methods from IOrderRepositry 
/// </summary>
namespace WebService.Models
{
    public class OrderRepository : IOrderRepository<Order>
    {
        /// <summary>
        /// Path to folder with in-memory database
        /// </summary>
        public string databasePath { get; set ; }
        /// <summary>
        /// Name of Table of Orders
        /// </summary>
        public string tableName { get; set; }

        /// <summary>
        /// Setup of configuration
        /// </summary>
        /// <param name="configuration">defined in appsettings.json</param>
        public OrderRepository(IConfiguration configuration)
        {
            databasePath = configuration["database:connection"];
            tableName = configuration["database:ordersTable"];
        }

        /// <summary>
        /// Create order in database
        /// </summary>
        /// <param name="order">Will be inserted to database</param>
        public void Create(Order order)
        {
            //Open or Create Database at set path
            using (var database = new LiteDatabase(@databasePath))
            {
                //Get Table of Orders
                LiteCollection<Order> collection = database.GetCollection<Order>(tableName);
                //Insert new Order
                collection.Insert(order);
            }
        }

        /// <summary>
        /// Delete existing order from database
        /// </summary>
        /// <param name="orderId">Id of Order, which will be deleted</param>
        /// <returns>
        /// True if successfuly deleted
        /// False if couldn't be found
        /// </returns>
        public bool Delete(Guid orderId)
        {

            //Open or Create Database at set path
            using (var database = new LiteDatabase(@databasePath))
            {

                //Get Table of Orders
                LiteCollection<Order> collection = database.GetCollection<Order>(tableName);
                //Execute delete method on database
                return collection.Delete(orderId);
            }
        }

        /// <summary>
        /// Select All orders from database
        /// </summary>
        /// <returns>IEnumerable List of all Orders</returns>
        public IEnumerable<Order> FindAll()
        {
            //Open or Create Database at set path
            using (var database = new LiteDatabase(@databasePath))
            {
                //Get Table of Orders
                LiteCollection<Order> collection = database.GetCollection<Order>(tableName);
                //Get all orders from database
                return collection.FindAll();
            }
        }

        /// <summary>
        /// Orders are Selected from database by Condition
        /// </summary>
        /// <param name="expression">Condition specifying which orders will be selected</param>
        /// <returns>IEnumerable array od Orders</returns>
        public IEnumerable<Order> FindByCondition(Expression<Func<Order, bool>> expression)
        {
            //Open or Create Database at set path
            using (var database = new LiteDatabase(@databasePath))
            {
                //Get Table of Orders
                LiteCollection<Order> collection = database.GetCollection<Order>(tableName);
                //Find Orders if condition is met
                return collection.Find(expression);
            }
        }
        
        /// <summary>
        /// Update existing Order
        /// </summary>
        /// <param name="id">Id of order, which will be updated</param>
        /// <param name="order">copy of order, with new data</param>
        /// <returns>True if found and updated successfuly, false if not found</returns>
        public bool Update(Guid id, Order order)
        {
            order.Id = id;

            //Open or Create Database at set path
            using (var database = new LiteDatabase(@databasePath))
            {

                //Get Table of Orders
                LiteCollection<Order> collection = database.GetCollection<Order>(tableName);
                //check if order with id exists 
                var storedOrder = collection.FindOne(a => a.Id.Equals(id));

                //Order with id doesnt exist and Cannot be modified
                if (storedOrder == null)
                {
                    //Modify failed on Non existing Order
                    return false;
                }

                //Modify Order
                collection.Update(id, order);
                //Modify was successful
                return true;
            }
        }
    }
}
